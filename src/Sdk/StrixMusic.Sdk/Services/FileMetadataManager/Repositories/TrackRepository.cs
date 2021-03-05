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
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance for <see cref="TrackRepository"/>.
        /// </summary>
        ///  <param name="fileMetadataScanner">The file scanner instance to source metadata from.</param>
        public TrackRepository(FileMetadataScanner fileMetadataScanner)
        {
            Guard.IsNotNull(fileMetadataScanner, nameof(fileMetadataScanner));

            _inMemoryMetadata = new ConcurrentDictionary<string, TrackMetadata>();
            _fileMetadataScanner = fileMetadataScanner;
            _storageMutex = new SemaphoreSlim(1, 1);
            _debouncerId = Guid.NewGuid().ToString();

            AttachEvents();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            Guard.IsFalse(IsInitialized, nameof(IsInitialized));
            IsInitialized = true;

            //await LoadDataFromDisk();
        }

        private void AttachEvents()
        {
            _fileMetadataScanner.FileMetadataAdded += FileMetadataScanner_FileMetadataAdded;
            _fileMetadataScanner.FileMetadataRemoved += FileMetadataScanner_FileMetadataRemoved;
        }

        private void DetachEvents()
        {
            _fileMetadataScanner.FileMetadataAdded -= FileMetadataScanner_FileMetadataAdded;
            _fileMetadataScanner.FileMetadataRemoved -= FileMetadataScanner_FileMetadataRemoved;
        }

        private async void FileMetadataScanner_FileMetadataRemoved(object sender, FileMetadata e)
        {
            // Remove tracks
            if (e.TrackMetadata != null)
            {
                Guard.IsNotNullOrWhiteSpace(e.TrackMetadata.Id, nameof(e.TrackMetadata.Id));

                await _storageMutex.WaitAsync();
                var removed = _inMemoryMetadata.TryRemove(e.TrackMetadata.Id, out _);
                _storageMutex.Release();

                if (removed)
                {
                    MetadataRemoved?.Invoke(this, e.TrackMetadata);
                    _ = CommitChangesAsync();
                }
            }

            // No need to update tracks with removed album IDs.
            // The album exists as long as any tracks from the album still exist, so it's handled in the AlbumRepository.

            // Update tracks with removed artist IDs 
            foreach (var data in _inMemoryMetadata.Values)
            {
                if (e.ArtistMetadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata.Id, nameof(e.ArtistMetadata.Id));

                    if (data?.ArtistIds?.Contains(e.ArtistMetadata.Id) ?? false)
                    {
                        data.ArtistIds.Remove(e.ArtistMetadata.Id);

                        MetadataUpdated?.Invoke(this, data);
                        _ = CommitChangesAsync();
                    }
                }
            }
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, FileMetadata e)
        {
            // Add track metadata
            if (e.TrackMetadata != null)
            {
                Guard.IsNotNullOrWhiteSpace(e.TrackMetadata.Id, nameof(e.TrackMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(e.AlbumMetadata?.Id, nameof(e.AlbumMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata?.Id, nameof(e.ArtistMetadata.Id));

                e.TrackMetadata.AlbumId = e.AlbumMetadata.Id;
                e.TrackMetadata.ArtistIds = e.ArtistMetadata.Id.IntoList();

                await _storageMutex.WaitAsync();

                if (!_inMemoryMetadata.TryAdd(e.TrackMetadata.Id, e.TrackMetadata))
                    ThrowHelper.ThrowInvalidOperationException($"Tried adding an item that already exists in {nameof(_inMemoryMetadata)}");

                _storageMutex.Release();

                MetadataAdded?.Invoke(this, e.TrackMetadata);
            }

            _ = CommitChangesAsync();
        }

        /// <inheritdoc />
        public event EventHandler<TrackMetadata>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<TrackMetadata>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<TrackMetadata>? MetadataRemoved;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(TrackMetadata Track, ArtistMetadata Artist)>? ArtistItemsChanged;

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
                MetadataUpdated?.Invoke(this, trackMetadata);
            else
                MetadataAdded?.Invoke(this, trackMetadata);
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
                MetadataRemoved?.Invoke(this, trackMetadata);
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
            var tracks = await GetTracks(offset, -1);
            var filteredTracks = new List<TrackMetadata>();

            foreach (var item in tracks)
            {
                Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
                Guard.HasSizeGreaterThan(item.ArtistIds, 0, nameof(TrackMetadata.ArtistIds));

                if (item.ArtistIds.Contains(artistId))
                    filteredTracks.Add(item);
            }

            return filteredTracks.GetRange(offset, limit).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string albumId, int offset, int limit)
        {
            var filteredTracks = new List<TrackMetadata>();

            var tracks = await GetTracks(offset, -1);

            foreach (var item in tracks)
            {
                Guard.IsNotNull(item.AlbumId, nameof(item.AlbumId));

                if (item.AlbumId == albumId)
                    filteredTracks.Add(item);
            }

            return filteredTracks.Skip(offset).Take(limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk()
        {
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
            var bytes = MessagePackSerializer.Serialize(_inMemoryMetadata.ToList(), MessagePack.Resolvers.ContractlessStandardResolver.Options);

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
