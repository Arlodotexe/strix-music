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
        private readonly List<(bool IsAdded, (AlbumMetadata Album, TrackMetadata Track) Data)> _batchTracksToUpdate;
        private readonly List<(bool IsAdded, (AlbumMetadata Album, ArtistMetadata Artist) Data)> _batchArtistsToUpdate;
        private readonly SemaphoreSlim _storageMutex;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly TrackRepository _trackRepository;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumRepository"/>.
        /// </summary>
        ///  <param name="fileMetadataScanner">The file scanner instance to source metadata from.</param>
        /// <param name="trackRepository">A <see cref="TrackRepository"/> that references the same set of data as this <see cref="AlbumMetadata"/>.</param>
        internal AlbumRepository(FileMetadataScanner fileMetadataScanner, TrackRepository trackRepository)
        {
            Guard.IsNotNull(fileMetadataScanner, nameof(fileMetadataScanner));

            _fileMetadataScanner = fileMetadataScanner;
            _trackRepository = trackRepository;

            _inMemoryMetadata = new ConcurrentDictionary<string, AlbumMetadata>();
            _batchTracksToUpdate = new List<(bool IsAdded, (AlbumMetadata Album, TrackMetadata Track) Data)>();
            _batchArtistsToUpdate = new List<(bool IsAdded, (AlbumMetadata Album, ArtistMetadata Artist) Data)>();
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
            // Remove album
            if (e.AlbumMetadata != null)
            {
                Guard.IsNotNullOrWhiteSpace(e.AlbumMetadata.Id, nameof(e.AlbumMetadata.Id));

                // If all tracks with this artist have been removed, we can safely remove this artist.
                var tracksWithAlbum = await _trackRepository.GetTracksByAlbumId(e.AlbumMetadata.Id, 0, -1);
                if (tracksWithAlbum.Count == 0)
                {
                    await _storageMutex.WaitAsync();
                    var removed = _inMemoryMetadata.TryRemove(e.AlbumMetadata.Id, out _);
                    _storageMutex.Release();

                    if (!removed)
                        ThrowHelper.ThrowInvalidOperationException($"Unable to remove metadata from {nameof(_inMemoryMetadata)}");

                    MetadataRemoved?.Invoke(this, e.AlbumMetadata);
                    _ = CommitChangesAsync();
                }
            }
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, FileMetadata e)
        {
            // Add new or update existing album, with the given artist and track ID.
            if (e.AlbumMetadata != null)
            {
                Guard.IsNotNullOrWhiteSpace(e.AlbumMetadata.Id, nameof(e.AlbumMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(e.TrackMetadata?.Id, nameof(e.TrackMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata?.Id, nameof(e.ArtistMetadata.Id));

                var exists = true;
                await _storageMutex.WaitAsync();

                var workingMetadata = _inMemoryMetadata.GetOrAdd(e.AlbumMetadata.Id, (key) =>
                {
                    exists = false;
                    return e.AlbumMetadata;
                });

                _storageMutex.Release();

                workingMetadata.ArtistIds ??= new List<string>();
                workingMetadata.TrackIds ??= new List<string>();

                var newArtist = !workingMetadata.ArtistIds.Contains(e.ArtistMetadata.Id);

                workingMetadata.ArtistIds.Add(e.ArtistMetadata.Id);
                workingMetadata.TrackIds.Add(e.TrackMetadata.Id);

                if (exists)
                    MetadataUpdated?.Invoke(this, workingMetadata);
                else
                    MetadataAdded?.Invoke(this, workingMetadata);

                if (newArtist)
                    _batchArtistsToUpdate.Add((true, (workingMetadata, e.ArtistMetadata)));

                _batchTracksToUpdate.Add((true, (workingMetadata, e.TrackMetadata)));

                _ = HandleChanged();
            }
        }

        private async Task HandleChanged()
        {
            await _storageMutex.WaitAsync();
            if (_batchTracksToUpdate.Count + _batchArtistsToUpdate.Count < 50 && !await Flow.Debounce(_debouncerId, TimeSpan.FromSeconds(2)))
                return;

            var addedArtistItems = new List<CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>>();
            var removedArtistItems = new List<CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>>();

            var addedTrackItems = new List<CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>>();
            var removedTrackItems = new List<CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>>();

            foreach (var item in _batchTracksToUpdate)
            {
                if (item.IsAdded)
                    addedTrackItems.Add(new CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>(item.Data, addedTrackItems.Count));
                else
                    removedTrackItems.Add(new CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>(item.Data, addedTrackItems.Count));
            }

            foreach (var item in _batchArtistsToUpdate)
            {
                if (item.IsAdded)
                    addedArtistItems.Add(new CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>(item.Data, addedTrackItems.Count));
                else
                    removedArtistItems.Add(new CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>(item.Data, addedTrackItems.Count));
            }

            // If the album already exists, modify the artists / tracks for this album.

            if (addedArtistItems.Count + removedArtistItems.Count > 0)
                ArtistItemsChanged?.Invoke(this, addedArtistItems, removedArtistItems);

            if (addedTrackItems.Count + removedTrackItems.Count > 0)
                TracksChanged?.Invoke(this, addedTrackItems, removedTrackItems);

            _storageMutex.Release();
            await CommitChangesAsync();
        }

        /// <inheritdoc />
        public event EventHandler<AlbumMetadata>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<AlbumMetadata>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<AlbumMetadata>? MetadataRemoved;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(AlbumMetadata Album, ArtistMetadata Artist)>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(AlbumMetadata Album, TrackMetadata Track)>? TracksChanged;

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
                MetadataUpdated?.Invoke(this, albumMetadata);
            else
                MetadataAdded?.Invoke(this, albumMetadata);

        }

        /// <inheritdoc />
        public async Task RemoveAlbum(AlbumMetadata albumMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(albumMetadata.Id, nameof(albumMetadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(albumMetadata.Id, out _);
            _storageMutex.Release();

            if (removed)
                _ = CommitChangesAsync();
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
                return Task.FromResult<IReadOnlyList<AlbumMetadata>>(allAlbums.GetRange(offset, allAlbums.Count - offset));

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

            return results.GetRange(offset, limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk()
        {
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(ALBUM_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            var bytes = await stream.ToBytesAsync();

            if (bytes.Length == 0)
                return;

            var data = MessagePackSerializer.Deserialize<List<AlbumMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            foreach (var item in data)
            {
                Guard.IsNotNullOrWhiteSpace(item?.Id, nameof(item.Id));

                if (!_inMemoryMetadata.TryAdd(item.Id, item))
                    ThrowHelper.ThrowInvalidOperationException($"Item already added to {nameof(_inMemoryMetadata)}");
            }
        }

        private async Task CommitChangesAsync()
        {
            if (!await Flow.Debounce(_debouncerId, TimeSpan.FromSeconds(5)) || _inMemoryMetadata.IsEmpty)
                return;

            await _storageMutex.WaitAsync();

            Guard.IsNotNull(_folderData, nameof(_folderData));
            var bytes = MessagePackSerializer.Serialize(_inMemoryMetadata.ToList(), MessagePack.Resolvers.ContractlessStandardResolver.Options);

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
