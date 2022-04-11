// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.FileMetadata.Models;
using StrixMusic.Sdk.FileMetadata.Scanners;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// The service that helps in interacting with playlist information.
    /// </summary>
    public sealed class PlaylistRepository : IPlaylistRepository
    {
        private const string PLAYLISTS_DATA_FILENAME = "Playlists.bin";

        private readonly ConcurrentDictionary<string, PlaylistMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="PlaylistRepository"/>.
        /// </summary>
        /// <param name="playlistMetadataScanner">The file scanner instance to source metadata from.</param>
        internal PlaylistRepository(PlaylistMetadataScanner playlistMetadataScanner)
        {
            _playlistMetadataScanner = playlistMetadataScanner;

            Guard.IsNotNull(_playlistMetadataScanner, nameof(_playlistMetadataScanner));

            _inMemoryMetadata = new ConcurrentDictionary<string, PlaylistMetadata>();
            _playlistMetadataScanner = playlistMetadataScanner;
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
            _debouncerId = Guid.NewGuid().ToString();

            AttachEvents();
        }

        /// <inheritdoc />
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            using var initMutexReleaseRegistration = cancellationToken.Register(() => _initMutex.Release());
            await _initMutex.WaitAsync(cancellationToken);
            if (IsInitialized)
            {
                _initMutex.Release();
                return;
            }

            await LoadDataFromDisk(cancellationToken);

            IsInitialized = true;
            _initMutex.Release();
        }

        /// <inheritdoc />
        public event EventHandler<IEnumerable<PlaylistMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<PlaylistMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<PlaylistMetadata>>? MetadataRemoved;

        private void AttachEvents()
        {
            _playlistMetadataScanner.PlaylistMetadataAdded += PlaylistMetadataScanner_PlaylistMetadataAdded;
            _playlistMetadataScanner.PlaylistMetadataRemoved += PlaylistMetadataScannerPlaylistMetadataRemoved;
        }

        private void DetachEvents()
        {
            _playlistMetadataScanner.PlaylistMetadataAdded -= PlaylistMetadataScanner_PlaylistMetadataAdded;
            _playlistMetadataScanner.PlaylistMetadataRemoved -= PlaylistMetadataScannerPlaylistMetadataRemoved;
        }

        private async void PlaylistMetadataScannerPlaylistMetadataRemoved(object sender, IEnumerable<PlaylistMetadata> e)
        {
            var removedPlaylists = new List<PlaylistMetadata>();

            foreach (var metadata in e)
            {
                if (metadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));

                    await _storageMutex.WaitAsync();

                    if (!_inMemoryMetadata.TryAdd(metadata.Id, metadata))
                        ThrowHelper.ThrowInvalidOperationException($"Tried adding an item that already exists in {nameof(_inMemoryMetadata)}");

                    _storageMutex.Release();
                    removedPlaylists.Add(metadata);
                }
            }

            if (removedPlaylists.Count > 0)
            {
                _ = CommitChangesAsync();
                MetadataRemoved?.Invoke(this, removedPlaylists);
            }
        }

        private async void PlaylistMetadataScanner_PlaylistMetadataAdded(object sender, IEnumerable<PlaylistMetadata> e)
        {
            var addedPlaylists = new List<PlaylistMetadata>();
            var updatedPlaylists = new List<PlaylistMetadata>();

            await _storageMutex.WaitAsync();

            foreach (var metadata in e)
            {
                if (metadata == null)
                    continue;

                Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));

                var playlistExists = true;
                _inMemoryMetadata.GetOrAdd(metadata.Id, key =>
                {
                    playlistExists = false;
                    return metadata;
                });

                if (playlistExists)
                    updatedPlaylists.Add(metadata);
                else
                    addedPlaylists.Add(metadata);

                if (addedPlaylists.Count > 0)
                    MetadataAdded?.Invoke(this, addedPlaylists);

                if (updatedPlaylists.Count > 0)
                    MetadataUpdated?.Invoke(this, updatedPlaylists);
            }

            _ = CommitChangesAsync();
            _storageMutex.Release();
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to save data in.</param>
        public void SetDataFolder(IFolderData rootFolder)
        {
            _folderData = rootFolder;
        }

        /// <inheritdoc />
        public Task<int> GetItemCount()
        {
            return Task.FromResult(_inMemoryMetadata.Count);
        }

        /// <inheritdoc />
        public async Task AddOrUpdateAsync(params PlaylistMetadata[] playlistMetadata)
        {
            await _storageMutex.WaitAsync();
            var updatedItems = new List<PlaylistMetadata>();
            var newItems = new List<PlaylistMetadata>();

            foreach (var item in playlistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                var isUpdate = false;

                _inMemoryMetadata.AddOrUpdate(
                    item.Id,
                    addValueFactory: id =>
                    {
                        isUpdate = false;
                        return item;
                    },
                    updateValueFactory: (id, existing) =>
                    {
                        isUpdate = true;
                        return item;
                    });

                if (isUpdate)
                    updatedItems.Add(item);
                else
                    newItems.Add(item);
            }

            _storageMutex.Release();

            _ = CommitChangesAsync();

            if (newItems.Count > 0)
                MetadataAdded?.Invoke(this, newItems);

            if (updatedItems.Count > 0)
                MetadataUpdated?.Invoke(this, updatedItems);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(PlaylistMetadata playlistMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(playlistMetadata.Id, nameof(playlistMetadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(playlistMetadata.Id, out _);
            _storageMutex.Release();

            if (removed)
            {
                _ = CommitChangesAsync();
                MetadataRemoved?.Invoke(this, playlistMetadata.IntoList());
            }
        }

        /// <inheritdoc />
        public Task<PlaylistMetadata?> GetByIdAsync(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var metadata);

            return Task.FromResult<PlaylistMetadata?>(metadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<PlaylistMetadata>> GetItemsAsync(int offset, int limit)
        {
            var allPlaylists = _inMemoryMetadata.Values.ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(allPlaylists);

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= allPlaylists.Count)
                return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(new List<PlaylistMetadata>());

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > allPlaylists.Count)
                limit = allPlaylists.Count - offset;

            return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(allPlaylists.GetRange(offset, limit));
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="PlaylistMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk(CancellationToken cancellationToken)
        {
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(PLAYLISTS_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            cancellationToken.ThrowIfCancellationRequested();

            var bytes = await stream.ToBytesAsync();
            cancellationToken.ThrowIfCancellationRequested();

            if (bytes.Length == 0)
                return;

            var str = System.Text.Encoding.UTF8.GetString(bytes);
            var data = JsonConvert.DeserializeObject<List<PlaylistMetadata>>(str);
            cancellationToken.ThrowIfCancellationRequested();

            using var mutexReleaseOnCancelRegistration = cancellationToken.Register(() => _storageMutex.Release());
            await _storageMutex.WaitAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var item in data ?? Enumerable.Empty<PlaylistMetadata>())
            {
                Guard.IsNotNullOrWhiteSpace(item?.Id, nameof(item.Id));

                if (!_inMemoryMetadata.TryAdd(item.Id, item))
                    ThrowHelper.ThrowInvalidOperationException($"Item already added to {nameof(_inMemoryMetadata)}");
            }

            _storageMutex.Release();
        }

        private async Task CommitChangesAsync()
        {
            if (!await Flow.Debounce(_debouncerId, TimeSpan.FromSeconds(5)) || _inMemoryMetadata.IsEmpty)
                return;

            await _storageMutex.WaitAsync();

            Guard.IsNotNull(_folderData, nameof(_folderData));
            var json = JsonConvert.SerializeObject(_inMemoryMetadata.Values.DistinctBy(x => x.Id).ToList());

            var fileData = await _folderData.CreateFileAsync(PLAYLISTS_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
            await fileData.WriteAllBytesAsync(System.Text.Encoding.UTF8.GetBytes(json));

            _storageMutex.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!IsInitialized)
                return;

            // Dispose any objects you created here
            _inMemoryMetadata.Clear();
            _storageMutex.Dispose();
            DetachEvents();

            IsInitialized = false;
        }
    }
}
