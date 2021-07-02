using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class TrackRepository : ITrackRepository
    {
        private const string TRACK_DATA_FILENAME = "TrackData.bin";

        private readonly ConcurrentDictionary<string, TrackMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="TrackRepository"/>.
        /// </summary>
        /// <param name="audioMetadataScanner">The file scanner instance to source metadata from.</param>
        public TrackRepository(AudioMetadataScanner audioMetadataScanner)
        {
            Guard.IsNotNull(audioMetadataScanner, nameof(audioMetadataScanner));

            _inMemoryMetadata = new ConcurrentDictionary<string, TrackMetadata>();
            _audioMetadataScanner = audioMetadataScanner;
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
            _debouncerId = Guid.NewGuid().ToString();

            AttachEvents();
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
        public event EventHandler<IEnumerable<TrackMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<TrackMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<TrackMetadata>>? MetadataRemoved;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(TrackMetadata Track, ArtistMetadata Artist)>? ArtistItemsChanged;

        private void AttachEvents()
        {
            _audioMetadataScanner.FileMetadataAdded += FileMetadataScanner_FileMetadataAdded;
            _audioMetadataScanner.FileMetadataRemoved += FileMetadataScanner_FileMetadataRemoved;
        }

        private void DetachEvents()
        {
            _audioMetadataScanner.FileMetadataAdded -= FileMetadataScanner_FileMetadataAdded;
            _audioMetadataScanner.FileMetadataRemoved -= FileMetadataScanner_FileMetadataRemoved;
        }

        private async void FileMetadataScanner_FileMetadataRemoved(object sender, IEnumerable<FileMetadata> e)
        {
            var removedTracks = new List<TrackMetadata>();

            // Remove tracks
            foreach (var metadata in e)
            {
                if (metadata.TrackMetadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata.Id, nameof(metadata.TrackMetadata.Id));

                    await _storageMutex.WaitAsync();
                    var removed = _inMemoryMetadata.TryRemove(metadata.TrackMetadata.Id, out _);
                    _storageMutex.Release();

                    if (removed)
                        removedTracks.Add(metadata.TrackMetadata);
                }

                // No need to update tracks with removed album IDs.
                // The album exists as long as any tracks from the album still exist, so it's handled in the AlbumRepository.

                // Update tracks with removed artist IDs 
                foreach (var data in _inMemoryMetadata.Values)
                {
                    if (metadata.ArtistMetadata != null)
                    {
                        Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata.Id, nameof(metadata.ArtistMetadata.Id));

                        if (data?.ArtistIds?.Contains(metadata.ArtistMetadata.Id) ?? false)
                        {
                            // TODO: emit artist items changed for this track.
                            data.ArtistIds.Remove(metadata.ArtistMetadata.Id);
                        }
                    }
                }
            }

            MetadataRemoved?.Invoke(this, removedTracks);
            _ = CommitChangesAsync();
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, IEnumerable<FileMetadata> e)
        {
            var addedTracks = new List<TrackMetadata>();
            var updatedTracks = new List<TrackMetadata>();

            // Iterate through FileMetadata and store in memory.
            // Updates and additions are tracked separately and emitted as events after all metadata has been processed.
            foreach (var metadata in e)
            {
                Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata?.Id, nameof(metadata.TrackMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata?.Id, nameof(metadata.AlbumMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata?.Id, nameof(metadata.ArtistMetadata.Id));

                var trackMetadata = metadata.TrackMetadata;

                trackMetadata.AlbumId = metadata.AlbumMetadata.Id;
                trackMetadata.ArtistIds = metadata.ArtistMetadata.Id.IntoList();

                var isUpdate = false;

                await _storageMutex.WaitAsync();

                _inMemoryMetadata.AddOrUpdate(
                    trackMetadata.Id,
                    addValueFactory: id =>
                    {
                        isUpdate = false;
                        return trackMetadata;
                    },
                    updateValueFactory: (id, existing) =>
                    {
                        isUpdate = true;
                        return trackMetadata;
                    });

                _storageMutex.Release();

                if (isUpdate)
                    updatedTracks.Add(metadata.TrackMetadata);
                else
                    addedTracks.Add(metadata.TrackMetadata);
            }

            if (addedTracks.Count > 0 || updatedTracks.Count > 0)
            {
                _ = CommitChangesAsync();

                if (addedTracks.Count > 0)
                    MetadataAdded?.Invoke(this, addedTracks);

                if (updatedTracks.Count > 0)
                    MetadataUpdated?.Invoke(this, updatedTracks);
            }
        }

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
        public async Task AddOrUpdateTrack(TrackMetadata trackMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(trackMetadata.Id, nameof(trackMetadata.Id));

            var isUpdate = false;

            await _storageMutex.WaitAsync();

            _inMemoryMetadata.AddOrUpdate(
                trackMetadata.Id,
                addValueFactory: id =>
                {
                    isUpdate = false;
                    return trackMetadata;
                },
                updateValueFactory: (id, existing) =>
                {
                    isUpdate = true;
                    return trackMetadata;
                });

            _storageMutex.Release();

            await CommitChangesAsync();

            if (isUpdate)
                MetadataUpdated?.Invoke(this, trackMetadata.IntoList());
            else
                MetadataAdded?.Invoke(this, trackMetadata.IntoList());
        }

        /// <inheritdoc />
        public async Task RemoveTrack(TrackMetadata trackMetadata)
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
        public Task<TrackMetadata?> GetTrackById(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var trackMetadata);

            return Task.FromResult<TrackMetadata?>(trackMetadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TrackMetadata>> GetTracks(int offset, int limit)
        {
            var allTracks = _inMemoryMetadata.Values.OrderBy(c => c.TrackNumber).ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<TrackMetadata>>(allTracks.GetRange(offset, allTracks.Count - offset));

            return Task.FromResult<IReadOnlyList<TrackMetadata>>(allTracks.GetRange(offset, limit));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByArtistId(string artistId, int offset, int limit)
        {
            var allTracks = await GetTracks(offset, -1);
            var filteredTracks = new List<TrackMetadata>();

            foreach (var item in allTracks)
            {
                Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
                Guard.HasSizeGreaterThan(item.ArtistIds, 0, nameof(TrackMetadata.ArtistIds));

                if (item.ArtistIds.Contains(artistId))
                    filteredTracks.Add(item);
            }

            if (offset + limit > allTracks.Count)
                return new List<TrackMetadata>();

            return filteredTracks.GetRange(offset, limit).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string albumId, int offset, int limit)
        {
            var filteredTracks = new List<TrackMetadata>();
            var allTracks = await GetTracks(offset, -1);

            foreach (var item in allTracks)
            {
                Guard.IsNotNull(item.AlbumId, nameof(item.AlbumId));

                if (item.AlbumId == albumId)
                    filteredTracks.Add(item);
            }

            if (offset + limit > allTracks.Count)
                return new List<TrackMetadata>();

            return filteredTracks.Skip(offset).Take(limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk()
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, TrackMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(TRACK_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            var bytes = await stream.ToBytesAsync();

            if (bytes.Length == 0)
                return;

            var data = MessagePackSerializer.Deserialize<List<TrackMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            await _storageMutex.WaitAsync();

            foreach (var item in data)
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
            var bytes = MessagePackSerializer.Serialize(_inMemoryMetadata.Values.ToList(), MessagePack.Resolvers.ContractlessStandardResolver.Options);

            var fileData = await _folderData.CreateFileAsync(TRACK_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
            await fileData.WriteAllBytesAsync(bytes);

            _storageMutex.Release();
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));

            ReleaseUnmanagedResources();
            if (disposing)
            {
                _inMemoryMetadata.Clear();
                _storageMutex.Dispose();
            }

            IsInitialized = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~TrackRepository()
        {
            Dispose(false);
        }
    }
}
