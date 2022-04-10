// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Newtonsoft.Json;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// The service that helps in interacting with track information.
    /// </summary>
    public sealed class TrackRepository : ITrackRepository
    {
        private const string TRACK_DATA_FILENAME = "TrackData.bin";

        private readonly ConcurrentDictionary<string, TrackMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="TrackRepository"/>.
        /// </summary>
        public TrackRepository()
        {
            _inMemoryMetadata = new ConcurrentDictionary<string, TrackMetadata>();
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
            _debouncerId = Guid.NewGuid().ToString();
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
        public event EventHandler<IEnumerable<TrackMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<TrackMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<TrackMetadata>>? MetadataRemoved;

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
        public async Task AddOrUpdateAsync(params TrackMetadata[] trackMetadata)
        {
            var addedTracks = new List<TrackMetadata>();
            var updatedTracks = new List<TrackMetadata>();

            // Iterate through FileMetadata and store in memory.
            // Updates and additions are tracked separately and emitted as events after all metadata has been processed.
            foreach (var item in trackMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                var trackExists = true;
                await _storageMutex.WaitAsync();

                var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
                {
                    trackExists = false;
                    return item;
                });

                workingMetadata.ArtistIds ??= new HashSet<string>();
                workingMetadata.ImageIds ??= new HashSet<string>();
                item.ArtistIds ??= new HashSet<string>();
                item.ImageIds ??= new HashSet<string>();

                Combine(workingMetadata.ArtistIds, item.ArtistIds);
                Combine(workingMetadata.ImageIds, item.ImageIds);

                _storageMutex.Release();

                if (trackExists)
                    updatedTracks.Add(workingMetadata);
                else
                    addedTracks.Add(workingMetadata);
            }

            if (addedTracks.Count > 0 || updatedTracks.Count > 0)
            {
                _ = CommitChangesAsync();

                if (addedTracks.Count > 0)
                    MetadataAdded?.Invoke(this, addedTracks);

                if (updatedTracks.Count > 0)
                    MetadataUpdated?.Invoke(this, updatedTracks);
            }

            void Combine(HashSet<string> originalData, HashSet<string> newIds)
            {
                foreach (var newId in newIds.ToArray())
                    originalData.Add(newId);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(TrackMetadata trackMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(trackMetadata.Id, nameof(trackMetadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(trackMetadata.Id, out _);
            _storageMutex.Release();

            if (removed)
            {
                await CommitChangesAsync();
                MetadataRemoved?.Invoke(this, trackMetadata.IntoList());
            }
        }

        /// <inheritdoc />
        public Task<TrackMetadata?> GetByIdAsync(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var trackMetadata);

            return Task.FromResult<TrackMetadata?>(trackMetadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TrackMetadata>> GetItemsAsync(int offset, int limit)
        {
            var allTracks = _inMemoryMetadata.Values.OrderBy(c => c.TrackNumber).ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<TrackMetadata>>(allTracks);

            return Task.FromResult<IReadOnlyList<TrackMetadata>>(allTracks.GetRange(offset, limit));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByArtistId(string artistId, int offset, int limit)
        {
            var allTracks = await GetItemsAsync(offset, -1);
            var results = new List<TrackMetadata>();

            foreach (var item in allTracks)
            {
                Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
                Guard.IsGreaterThan(item.ArtistIds.Count, 0, nameof(item.ArtistIds.Count));

                if (item.ArtistIds.Contains(artistId))
                    results.Add(item);
            }

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= results.Count)
                return new List<TrackMetadata>();

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > results.Count)
                limit = results.Count - offset;

            return results.GetRange(offset, limit).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string albumId, int offset, int limit)
        {
            var results = new List<TrackMetadata>();
            var allTracks = await GetItemsAsync(offset, -1);

            foreach (var item in allTracks)
            {
                Guard.IsNotNull(item.AlbumId, nameof(item.AlbumId));

                if (item.AlbumId == albumId)
                    results.Add(item);
            }

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= results.Count)
                return new List<TrackMetadata>();

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > results.Count)
                limit = results.Count - offset;

            return results.Skip(offset).Take(limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk(CancellationToken cancellationToken)
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, TrackMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(TRACK_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            cancellationToken.ThrowIfCancellationRequested();

            var bytes = await stream.ToBytesAsync();
            cancellationToken.ThrowIfCancellationRequested();

            if (bytes.Length == 0)
                return;

            var str = System.Text.Encoding.UTF8.GetString(bytes);
            var data = JsonConvert.DeserializeObject<List<TrackMetadata>>(str);
            cancellationToken.ThrowIfCancellationRequested();

            using var mutexReleaseOnCancelRegistration = cancellationToken.Register(() => _storageMutex.Release());
            await _storageMutex.WaitAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var item in data ?? Enumerable.Empty<TrackMetadata>())
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

            var fileData = await _folderData.CreateFileAsync(TRACK_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
            await fileData.WriteAllBytesAsync(System.Text.Encoding.UTF8.GetBytes(json));

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
