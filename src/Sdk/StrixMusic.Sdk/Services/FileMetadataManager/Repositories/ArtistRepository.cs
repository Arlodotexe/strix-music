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
    /// Service to retrieve artist records.
    /// </summary>
    public class ArtistRepository : IArtistRepository
    {
        private const string ARTIST_DATA_FILENAME = "ArtistMeta.bin";

        private readonly ConcurrentDictionary<string, ArtistMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly TrackRepository _trackRepository;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance for <see cref="TrackRepository"/>.
        /// </summary>
        ///  <param name="fileMetadataScanner">The file scanner instance to source metadata from.</param>
        /// <param name="trackRepository">A <see cref="TrackRepository"/> that references the same set of data as this <see cref="ArtistMetadata"/>.</param>
        internal ArtistRepository(FileMetadataScanner fileMetadataScanner, TrackRepository trackRepository)
        {
            Guard.IsNotNull(fileMetadataScanner, nameof(fileMetadataScanner));

            _fileMetadataScanner = fileMetadataScanner;
            _trackRepository = trackRepository;

            _inMemoryMetadata = new ConcurrentDictionary<string, ArtistMetadata>();
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
            // Remove artist
            if (e.ArtistMetadata != null)
            {
                Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata.Id, nameof(e.ArtistMetadata.Id));

                // If all tracks with this artist have been removed, we can safely remove this artist.
                var tracksWithArtist = await _trackRepository.GetTracksByArtistId(e.ArtistMetadata.Id, 0, -1);
                if (tracksWithArtist.Count == 0)
                {
                    await _storageMutex.WaitAsync();

                    if (!_inMemoryMetadata.TryRemove(e.ArtistMetadata.Id, out _))
                        ThrowHelper.ThrowInvalidOperationException($"Unable to remove metadata from {nameof(_inMemoryMetadata)}");

                    _storageMutex.Release();

                    MetadataRemoved?.Invoke(this, e.ArtistMetadata);
                    _ = CommitChangesAsync();
                }
            }
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, FileMetadata e)
        {
            await _storageMutex.WaitAsync();

            // Add new or update existing artist, with the given album and track ID.
            if (e.ArtistMetadata != null)
            {
                Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata.Id, nameof(e.ArtistMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(e.TrackMetadata?.Id, nameof(e.TrackMetadata.Id));
                Guard.IsNotNullOrWhiteSpace(e.AlbumMetadata?.Id, nameof(e.AlbumMetadata.Id));

                var exists = true;
                var workingMetadata = _inMemoryMetadata.GetOrAdd(e.ArtistMetadata.Id, key =>
                {
                    exists = false;
                    return e.ArtistMetadata;
                });

                workingMetadata.AlbumIds ??= new List<string>();
                workingMetadata.TrackIds ??= new List<string>();

                workingMetadata.AlbumIds.Add(e.AlbumMetadata.Id);
                workingMetadata.TrackIds.Add(e.TrackMetadata.Id);

                if (exists)
                {
                    var addedAlbumItem = new CollectionChangedItem<(ArtistMetadata Artist, AlbumMetadata Album)>((workingMetadata, e.AlbumMetadata), 0);
                    var removedAlbumItems = Array.Empty<CollectionChangedItem<(ArtistMetadata Artist, AlbumMetadata Album)>>();

                    var addedTrackItem = new CollectionChangedItem<(ArtistMetadata Artist, TrackMetadata Track)>((workingMetadata, e.TrackMetadata), 0);
                    var removedTrackItems = Array.Empty<CollectionChangedItem<(ArtistMetadata Artist, TrackMetadata Track)>>();

                    AlbumItemsChanged?.Invoke(this, addedAlbumItem.IntoList(), removedAlbumItems);
                    TracksChanged?.Invoke(this, addedTrackItem.IntoList(), removedTrackItems);
                    MetadataUpdated?.Invoke(this, workingMetadata);
                }
                else
                {
                    MetadataAdded?.Invoke(this, workingMetadata);
                }
            }

            _storageMutex.Release();
            _ = CommitChangesAsync();
        }

        /// <inheritdoc />
        public event EventHandler<ArtistMetadata>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<ArtistMetadata>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<ArtistMetadata>? MetadataRemoved;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(ArtistMetadata Artist, AlbumMetadata Album)>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(ArtistMetadata Artist, TrackMetadata Track)>? TracksChanged;

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
        public async Task AddOrUpdateArtist(ArtistMetadata artistMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(artistMetadata.Id, nameof(artistMetadata.Id));

            var isUpdate = false;

            await _storageMutex.WaitAsync();

            _inMemoryMetadata.AddOrUpdate(
                artistMetadata.Id,
                addValueFactory: id =>
                {
                    isUpdate = false;
                    return artistMetadata;
                },
                updateValueFactory: (id, existing) =>
                {
                    isUpdate = true;
                    return artistMetadata;
                });

            _storageMutex.Release();

            await CommitChangesAsync();

            if (isUpdate)
                MetadataUpdated?.Invoke(this, artistMetadata);
            else
                MetadataAdded?.Invoke(this, artistMetadata);
        }

        /// <inheritdoc />
        public async Task RemoveArtist(ArtistMetadata artistMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(artistMetadata.Id, nameof(artistMetadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(artistMetadata.Id, out _);
            _storageMutex.Release();

            if (removed)
                _ = CommitChangesAsync();
        }

        /// <inheritdoc />
        public Task<ArtistMetadata?> GetArtistById(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var metadata);
            return Task.FromResult<ArtistMetadata?>(metadata);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ArtistMetadata>> GetArtists(int offset, int limit)
        {
            var allArtists = _inMemoryMetadata.Values.ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<ArtistMetadata>>(allArtists.GetRange(offset, allArtists.Count - offset));

            return Task.FromResult<IReadOnlyList<ArtistMetadata>>(allArtists.GetRange(offset, limit));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByAlbumId(string albumId, int offset, int limit)
        {
            var allArtists = await GetArtists(offset, -1);
            var results = new List<ArtistMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.AlbumIds, nameof(item.AlbumIds));
                Guard.HasSizeGreaterThan(item.AlbumIds, 0, nameof(ArtistMetadata.AlbumIds));

                if (item.AlbumIds.Contains(albumId))
                    results.Add(item);
            }

            return results.GetRange(offset, limit).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByTrackId(string trackId, int offset, int limit)
        {
            var filteredArtists = new List<ArtistMetadata>();

            var artists = await GetArtists(0, -1);

            foreach (var item in artists)
            {
                Guard.IsNotNull(item.TrackIds, nameof(item.TrackIds));
                Guard.HasSizeGreaterThan(item.TrackIds, 0, nameof(ArtistMetadata.TrackIds));

                if (item.TrackIds.Contains(trackId))
                    filteredArtists.Add(item);
            }

            return filteredArtists.GetRange(offset, limit).ToList();
        }

        /// <summary>
        /// Gets the existing repo data stored on disk.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task LoadDataFromDisk()
        {
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(ARTIST_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            var bytes = await stream.ToBytesAsync();

            if (bytes.Length == 0)
                return;

            var data = MessagePackSerializer.Deserialize<List<ArtistMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

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
            if (!await Flow.Debounce(_debouncerId, TimeSpan.FromSeconds(4)) || _inMemoryMetadata.IsEmpty)
                return;

            await _storageMutex.WaitAsync();

            Guard.IsNotNull(_folderData, nameof(_folderData));
            var bytes = MessagePackSerializer.Serialize(_inMemoryMetadata.ToList(), MessagePack.Resolvers.ContractlessStandardResolver.Options);

            var fileData = await _folderData.CreateFileAsync(ARTIST_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
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
        ~ArtistRepository()
        {
            Dispose(false);
        }
    }
}
