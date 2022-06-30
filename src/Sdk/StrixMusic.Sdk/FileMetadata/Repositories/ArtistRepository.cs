// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// The service that helps in interacting with artist information.
    /// </summary>
    public sealed class ArtistRepository : IArtistRepository
    {
        private readonly string _dataFileName;

        private readonly ConcurrentDictionary<string, ArtistMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly string _debouncerId;
        private readonly string _id;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistRepository"/>.
        /// </summary>
        internal ArtistRepository(string id)
        {
            _inMemoryMetadata = new ConcurrentDictionary<string, ArtistMetadata>();
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
            _debouncerId = Guid.NewGuid().ToString();
            _id = id;

            _dataFileName = $"Artists.{id}.bin";
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

            try
            {
                await LoadDataFromDiskAsync(cancellationToken);
            }
            catch (JsonException)
            {
                // ignored
            }

            IsInitialized = true;
            _initMutex.Release();
        }

        /// <inheritdoc />
        public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataRemoved;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to work in.</param>
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
        public async Task AddOrUpdateAsync(params ArtistMetadata[] metadata)
        {
            await _storageMutex.WaitAsync();
            var addedArtists = new List<ArtistMetadata>();
            var updatedArtists = new List<ArtistMetadata>();

            foreach (var item in metadata)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                var artistExists = true;
                var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
                {
                    artistExists = false;
                    return item;
                });

                workingMetadata.AlbumIds ??= new HashSet<string>();
                workingMetadata.TrackIds ??= new HashSet<string>();
                workingMetadata.ImageIds ??= new HashSet<string>();
                item.AlbumIds ??= new HashSet<string>();
                item.TrackIds ??= new HashSet<string>();
                item.ImageIds ??= new HashSet<string>();

                Combine(workingMetadata.AlbumIds, item.AlbumIds);
                Combine(workingMetadata.TrackIds, item.TrackIds);
                Combine(workingMetadata.ImageIds, item.ImageIds);

                if (artistExists)
                    updatedArtists.Add(workingMetadata);
                else
                    addedArtists.Add(workingMetadata);
            }

            if (updatedArtists.Count > 0)
                MetadataUpdated?.Invoke(this, updatedArtists);

            if (addedArtists.Count > 0)
                MetadataAdded?.Invoke(this, addedArtists);

            _storageMutex.Release();
            _ = CommitChangesAsync();

            void Combine(HashSet<string> originalData, HashSet<string> newIds)
            {
                foreach (var newId in newIds.ToArray())
                    originalData.Add(newId);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(ArtistMetadata metadata)
        {
            Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(metadata.Id, out _);
            _storageMutex.Release();

            if (removed)
            {
                _ = CommitChangesAsync();
                MetadataRemoved?.Invoke(this, metadata.IntoList());
            }
        }

        /// <inheritdoc />
        public Task<ArtistMetadata?> GetByIdAsync(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var metadata);
            return Task.FromResult<ArtistMetadata?>(metadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ArtistMetadata>> GetItemsAsync(int offset, int limit)
        {
            var allArtists = _inMemoryMetadata.Values.ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<ArtistMetadata>>(allArtists);

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= allArtists.Count)
                return Task.FromResult<IReadOnlyList<ArtistMetadata>>(new List<ArtistMetadata>());

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > allArtists.Count)
                limit = allArtists.Count - offset;

            return Task.FromResult<IReadOnlyList<ArtistMetadata>>(allArtists.GetRange(offset, limit));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByAlbumId(string albumId, int offset, int limit)
        {
            var allArtists = await GetItemsAsync(offset, -1);
            var results = new List<ArtistMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.AlbumIds, nameof(item.AlbumIds));
                Guard.IsGreaterThan(item.AlbumIds.Count, 0, nameof(item.AlbumIds.Count));

                if (item.AlbumIds.Contains(albumId))
                    results.Add(item);
            }

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= results.Count)
                return new List<ArtistMetadata>();

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > results.Count)
                limit = results.Count - offset;

            return results.GetRange(offset, limit).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByTrackId(string trackId, int offset, int limit)
        {
            var allArtists = await GetItemsAsync(0, -1);
            var results = new List<ArtistMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.TrackIds, nameof(item.TrackIds));
                Guard.IsGreaterThan(item.TrackIds.Count, 0, nameof(item.TrackIds.Count));

                if (item.TrackIds.Contains(trackId))
                    results.Add(item);
            }

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= results.Count)
                return new List<ArtistMetadata>();

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > results.Count)
                limit = results.Count - offset;

            return results.GetRange(offset, limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDiskAsync(CancellationToken cancellationToken)
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, ArtistMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(_dataFileName, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            cancellationToken.ThrowIfCancellationRequested();

            var data = await FileMetadataRepoSerializer.Singleton.DeserializeAsync<List<ArtistMetadata>>(stream);
            cancellationToken.ThrowIfCancellationRequested();

            using var mutexReleaseOnCancelRegistration = cancellationToken.Register(() => _storageMutex.Release());
            await _storageMutex.WaitAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var item in data ?? Enumerable.Empty<ArtistMetadata>())
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
            using var serializedStream = await FileMetadataRepoSerializer.Singleton.SerializeAsync(_inMemoryMetadata.Values.DistinctBy(x => x.Id).ToList());

            var fileData = await _folderData.CreateFileAsync(_dataFileName, CreationCollisionOption.OpenIfExists);
            using var fileStream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            fileStream.Position = 0;
            serializedStream.Position = 0;

            await serializedStream.CopyToAsync(fileStream);

            _storageMutex.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _initMutex.Dispose();
            _storageMutex.Dispose();
        }
    }
}
