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
    /// The service that helps in interacting with album information.
    /// </summary>
    public sealed class AlbumRepository : IAlbumRepository
    {
        private const string ALBUM_DATA_FILENAME = "AlbumData.bin";

        private readonly ConcurrentDictionary<string, AlbumMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumRepository"/>.
        /// </summary>
        internal AlbumRepository()
        {
            _inMemoryMetadata = new ConcurrentDictionary<string, AlbumMetadata>();
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
            _debouncerId = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            await _initMutex.WaitAsync();
            if (IsInitialized)
            {
                _initMutex.Release();
                return;
            }

            await LoadDataFromDisk();

            IsInitialized = true;
            _initMutex.Release();
        }

        /// <inheritdoc />
        public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataRemoved;

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
        public async Task AddOrUpdateAsync(params AlbumMetadata[] metadata)
        {
            var addedAlbums = new List<AlbumMetadata>();
            var updatedAlbums = new List<AlbumMetadata>();

            foreach (var item in metadata)
            {
                Guard.IsNotNull(item.Id, nameof(item.Id));

                var albumExists = true;
                await _storageMutex.WaitAsync();

                var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
                {
                    albumExists = false;
                    return item;
                });

                _storageMutex.Release();

                workingMetadata.ArtistIds ??= new HashSet<string>();
                workingMetadata.TrackIds ??= new HashSet<string>();
                workingMetadata.ImageIds ??= new HashSet<string>();
                item.ArtistIds ??= new HashSet<string>();
                item.TrackIds ??= new HashSet<string>();
                item.ImageIds ??= new HashSet<string>();
                item.ArtistIds ??= new HashSet<string>();
                item.TrackIds ??= new HashSet<string>();
                item.ImageIds ??= new HashSet<string>();

                Combine(workingMetadata.ArtistIds, item.ArtistIds);
                Combine(workingMetadata.TrackIds, item.TrackIds);
                Combine(workingMetadata.ImageIds, item.ImageIds);

                if (albumExists)
                    updatedAlbums.Add(workingMetadata);
                else
                    addedAlbums.Add(workingMetadata);
            }

            if (updatedAlbums.Count > 0)
                MetadataUpdated?.Invoke(this, updatedAlbums);

            if (addedAlbums.Count > 0)
                MetadataAdded?.Invoke(this, addedAlbums);

            _ = CommitChangesAsync();

            void Combine(HashSet<string> originalData, HashSet<string> newIds)
            {
                foreach (var newId in newIds.ToArray())
                    originalData.Add(newId);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(AlbumMetadata metadata)
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
        public Task<AlbumMetadata?> GetByIdAsync(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var metadata);
            return Task.FromResult<AlbumMetadata?>(metadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<AlbumMetadata>> GetItemsAsync(int offset, int limit)
        {
            var allAlbums = _inMemoryMetadata.Values.ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<AlbumMetadata>>(allAlbums);

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= allAlbums.Count)
                return Task.FromResult<IReadOnlyList<AlbumMetadata>>(new List<AlbumMetadata>());

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > allAlbums.Count)
                limit = allAlbums.Count - offset;

            return Task.FromResult<IReadOnlyList<AlbumMetadata>>(allAlbums.GetRange(offset, limit));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<AlbumMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit)
        {
            var allArtists = await GetItemsAsync(offset, -1);
            var results = new List<AlbumMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
                Guard.IsGreaterThan(item.ArtistIds.Count, 0, nameof(item.ArtistIds.Count));

                if (item.ArtistIds.Contains(artistId))
                    results.Add(item);
            }

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= results.Count)
                return new List<AlbumMetadata>();

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > results.Count)
                limit = results.Count - offset;

            return results.GetRange(offset, limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk()
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, AlbumMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(ALBUM_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            var bytes = await stream.ToBytesAsync();

            if (bytes.Length == 0)
                return;

            var str = System.Text.Encoding.UTF8.GetString(bytes);
            var data = JsonConvert.DeserializeObject<List<AlbumMetadata>>(str);

            await _storageMutex.WaitAsync();

            foreach (var item in data ?? Enumerable.Empty<AlbumMetadata>())
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

            Guard.IsNotNull(_folderData, nameof(_folderData)); ;
            var json = JsonConvert.SerializeObject(_inMemoryMetadata.Values.DistinctBy(x => x.Id).ToList());

            var fileData = await _folderData.CreateFileAsync(ALBUM_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
            await fileData.WriteAllBytesAsync(System.Text.Encoding.UTF8.GetBytes(json));

            _storageMutex.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
