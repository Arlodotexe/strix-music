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
    /// <inheritdoc />
    public class AlbumRepository : IAlbumRepository
    {
        private const string ALBUM_DATA_FILENAME = "AlbumData.bin";

        private readonly ConcurrentDictionary<string, AlbumMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly TrackRepository _trackRepository;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumRepository"/>.
        /// </summary>
        ///  <param name="audioMetadataScanner">The file scanner instance to source metadata from.</param>
        /// <param name="trackRepository">A <see cref="TrackRepository"/> that references the same set of data as this <see cref="AlbumMetadata"/>.</param>
        internal AlbumRepository(AudioMetadataScanner audioMetadataScanner, TrackRepository trackRepository)
        {
            Guard.IsNotNull(audioMetadataScanner, nameof(audioMetadataScanner));

            _audioMetadataScanner = audioMetadataScanner;
            _trackRepository = trackRepository;

            _inMemoryMetadata = new ConcurrentDictionary<string, AlbumMetadata>();
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
        public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataRemoved;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(AlbumMetadata Album, ArtistMetadata Artist)>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(AlbumMetadata Album, TrackMetadata Track)>? TracksChanged;

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
            var removedAlbums = new List<AlbumMetadata>();

            var removedArtistItems = new List<CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>>();
            var removedTrackItems = new List<CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>>();

            // Remove album
            foreach (var metadata in e)
            {
                if (metadata.AlbumMetadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata.Id, nameof(metadata.AlbumMetadata.Id));

                    // If all tracks with this artist have been removed, we can safely remove this artist.
                    var tracksWithAlbum = await _trackRepository.GetTracksByAlbumId(metadata.AlbumMetadata.Id, 0, -1);
                    if (tracksWithAlbum.Count == 0)
                    {
                        await _storageMutex.WaitAsync();
                        var removed = _inMemoryMetadata.TryRemove(metadata.AlbumMetadata.Id, out _);
                        _storageMutex.Release();

                        removedAlbums.Add(metadata.AlbumMetadata);

                        if (!removed)
                            ThrowHelper.ThrowInvalidOperationException($"Unable to remove metadata from {nameof(_inMemoryMetadata)}");
                    }
                }
            }

            if (removedAlbums.Count > 0)
            {
                MetadataRemoved?.Invoke(this, removedAlbums);
                _ = CommitChangesAsync();
            }
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, IEnumerable<FileMetadata> e)
        {
            var addedAlbums = new List<AlbumMetadata>();
            var updatedAlbums = new List<AlbumMetadata>();

            var updatedArtistItems = new List<CollectionChangedItem<(AlbumMetadata, ArtistMetadata)>>();
            var addedTrackItems = new List<CollectionChangedItem<(AlbumMetadata, TrackMetadata)>>();

            foreach (var metadata in e)
            {
                // Add new or update existing album, with the given artist and track ID.
                if (metadata.AlbumMetadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata.Id, nameof(metadata.AlbumMetadata.Id));
                    Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata?.Id, nameof(metadata.TrackMetadata.Id));
                    Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata?.Id, nameof(metadata.ArtistMetadata.Id));

                    var albumExists = true;
                    await _storageMutex.WaitAsync();

                    var workingMetadata = _inMemoryMetadata.GetOrAdd(metadata.AlbumMetadata.Id, key =>
                    {
                        albumExists = false;
                        return metadata.AlbumMetadata;
                    });

                    _storageMutex.Release();

                    workingMetadata.ArtistIds ??= new List<string>();
                    workingMetadata.TrackIds ??= new List<string>();

                    if (!workingMetadata.ArtistIds.Contains(metadata.ArtistMetadata.Id))
                    {
                        workingMetadata.ArtistIds.Add(metadata.ArtistMetadata.Id);
                        updatedArtistItems.Add(new CollectionChangedItem<(AlbumMetadata, ArtistMetadata)>((workingMetadata, metadata.ArtistMetadata), updatedAlbums.Count));
                    }

                    if (!workingMetadata.TrackIds.Contains(metadata.TrackMetadata.Id))
                    {
                        workingMetadata.TrackIds.Add(metadata.TrackMetadata.Id);
                        addedTrackItems.Add(new CollectionChangedItem<(AlbumMetadata, TrackMetadata)>((workingMetadata, metadata.TrackMetadata), addedTrackItems.Count));
                    }

                    if (albumExists)
                        updatedAlbums.Add(workingMetadata);
                    else
                        addedAlbums.Add(workingMetadata);
                }
            }

            if (updatedArtistItems.Count > 0)
                ArtistItemsChanged?.Invoke(this, updatedArtistItems, Array.Empty<CollectionChangedItem<(AlbumMetadata, ArtistMetadata)>>());

            if (updatedAlbums.Count > 0)
                MetadataUpdated?.Invoke(this, updatedAlbums);

            if (addedAlbums.Count > 0)
                MetadataAdded?.Invoke(this, addedAlbums);

            if (addedTrackItems.Count > 0)
                TracksChanged?.Invoke(this, addedTrackItems, Array.Empty<CollectionChangedItem<(AlbumMetadata, TrackMetadata)>>());

            _ = CommitChangesAsync();
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
        public async Task AddOrUpdateAlbum(AlbumMetadata albumMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(albumMetadata.Id, nameof(albumMetadata.Id));

            var isUpdate = false;
            await _storageMutex.WaitAsync();

            _inMemoryMetadata.AddOrUpdate(
                albumMetadata.Id,
                addValueFactory: id =>
                {
                    isUpdate = false;
                    return albumMetadata;
                },
                updateValueFactory: (id, existing) =>
                {
                    isUpdate = true;
                    return albumMetadata;
                });

            _storageMutex.Release();

            _ = CommitChangesAsync();

            if (isUpdate)
                MetadataUpdated?.Invoke(this, albumMetadata.IntoList());
            else
                MetadataAdded?.Invoke(this, albumMetadata.IntoList());

        }

        /// <inheritdoc />
        public async Task RemoveAlbum(AlbumMetadata albumMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(albumMetadata.Id, nameof(albumMetadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(albumMetadata.Id, out _);
            _storageMutex.Release();

            if (removed)
            {
                _ = CommitChangesAsync();
                MetadataRemoved?.Invoke(this, albumMetadata.IntoList());
            }
        }

        /// <inheritdoc />
        public Task<AlbumMetadata?> GetAlbumById(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var metadata);
            return Task.FromResult<AlbumMetadata?>(metadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<AlbumMetadata>> GetAlbums(int offset, int limit)
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
            var allArtists = await GetAlbums(offset, -1);
            var results = new List<AlbumMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
                Guard.HasSizeGreaterThan(item.ArtistIds, 0, nameof(AlbumMetadata.ArtistIds));

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

            var data = MessagePackSerializer.Deserialize<List<AlbumMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

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

            var fileData = await _folderData.CreateFileAsync(ALBUM_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
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
            if (!IsInitialized)
                return;

            ReleaseUnmanagedResources();
            if (disposing)
            {
                // dispose any objects you created here
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
        ~AlbumRepository()
        {
            Dispose(false);
        }
    }
}
