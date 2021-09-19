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
    /// The service that helps in interacting with artist information.
    /// </summary>
    public class ArtistRepository : IArtistRepository
    {
        private const string ARTIST_DATA_FILENAME = "ArtistMeta.bin";

        private readonly ConcurrentDictionary<string, ArtistMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly TrackRepository _trackRepository;
        private readonly string _debouncerId;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistRepository"/>.
        /// </summary>
        /// <param name="audioMetadataScanner">The file scanner instance to source metadata from.</param>
        /// <param name="trackRepository">A <see cref="TrackRepository"/> that references the same set of data as this <see cref="ArtistMetadata"/>.</param>
        internal ArtistRepository(AudioMetadataScanner audioMetadataScanner, TrackRepository trackRepository)
        {
            Guard.IsNotNull(audioMetadataScanner, nameof(audioMetadataScanner));

            _audioMetadataScanner = audioMetadataScanner;
            _trackRepository = trackRepository;

            _inMemoryMetadata = new ConcurrentDictionary<string, ArtistMetadata>();
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
        public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataUpdated;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataAdded;

        /// <inheritdoc />
        public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataRemoved;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(ArtistMetadata Artist, AlbumMetadata Album)>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<(ArtistMetadata Artist, TrackMetadata Track)>? TracksChanged;

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
            var removedArtists = new List<ArtistMetadata>();
            var removedTrackItems = Array.Empty<CollectionChangedItem<(ArtistMetadata Artist, TrackMetadata Track)>>();
            var removedAlbumItems = Array.Empty<CollectionChangedItem<(ArtistMetadata Artist, AlbumMetadata Album)>>();

            // Remove artists
            foreach (var metadata in e)
            {
                if (metadata.ArtistMetadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata.Id, nameof(metadata.ArtistMetadata.Id));

                    // If all tracks with this artist have been removed, we can safely remove this artist.
                    var tracksWithArtist = await _trackRepository.GetTracksByArtistId(metadata.ArtistMetadata.Id, 0, -1);
                    if (tracksWithArtist.Count == 0)
                    {
                        await _storageMutex.WaitAsync();

                        if (!_inMemoryMetadata.TryRemove(metadata.ArtistMetadata.Id, out _))
                            ThrowHelper.ThrowInvalidOperationException($"Unable to remove metadata from {nameof(_inMemoryMetadata)}");

                        _storageMutex.Release();

                        removedArtists.Add(metadata.ArtistMetadata);
                    }
                }
            }

            if (removedArtists.Count > 0)
            {
                MetadataRemoved?.Invoke(this, removedArtists);
                _ = CommitChangesAsync();
            }
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, IEnumerable<FileMetadata> e)
        {
            await _storageMutex.WaitAsync();
            var addedArtists = new List<ArtistMetadata>();
            var updatedArtists = new List<ArtistMetadata>();

            var addedAlbumItems = new List<CollectionChangedItem<(ArtistMetadata, AlbumMetadata)>>();
            var addedTrackItems = new List<CollectionChangedItem<(ArtistMetadata, TrackMetadata)>>();

            foreach (var metadata in e)
            {
                // Add new or update existing artist, with the given album and track ID.
                if (metadata.ArtistMetadata != null)
                {
                    Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata.Id, nameof(metadata.ArtistMetadata.Id));
                    Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata?.Id, nameof(metadata.TrackMetadata.Id));
                    Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata?.Id, nameof(metadata.AlbumMetadata.Id));

                    var artistExists = true;
                    var workingMetadata = _inMemoryMetadata.GetOrAdd(metadata.ArtistMetadata.Id, key =>
                    {
                        artistExists = false;
                        return metadata.ArtistMetadata;
                    });

                    workingMetadata.AlbumIds ??= new List<string>();
                    workingMetadata.TrackIds ??= new List<string>();
                    workingMetadata.ImageIds ??= new List<string>();

                    if (!workingMetadata.AlbumIds.Contains(metadata.AlbumMetadata.Id))
                    {
                        addedAlbumItems.Add(new CollectionChangedItem<(ArtistMetadata, AlbumMetadata)>((workingMetadata, metadata.AlbumMetadata), addedAlbumItems.Count));
                        workingMetadata.AlbumIds.Add(metadata.AlbumMetadata.Id);
                    }

                    if (!workingMetadata.TrackIds.Contains(metadata.TrackMetadata.Id))
                    {
                        addedTrackItems.Add(new CollectionChangedItem<(ArtistMetadata, TrackMetadata)>((workingMetadata, metadata.TrackMetadata), addedTrackItems.Count));
                        workingMetadata.TrackIds.Add(metadata.TrackMetadata.Id);
                    }

                    if (artistExists)
                        updatedArtists.Add(workingMetadata);
                    else
                        addedArtists.Add(workingMetadata);
                }
            }

            if (addedAlbumItems.Count > 0)
                AlbumItemsChanged?.Invoke(this, addedAlbumItems, Array.Empty<CollectionChangedItem<(ArtistMetadata, AlbumMetadata)>>());

            if (addedTrackItems.Count > 0)
                TracksChanged?.Invoke(this, addedTrackItems, Array.Empty<CollectionChangedItem<(ArtistMetadata, TrackMetadata)>>());

            if (updatedArtists.Count > 0)
                MetadataUpdated?.Invoke(this, updatedArtists);

            if (addedArtists.Count > 0)
                MetadataAdded?.Invoke(this, addedArtists);

            _storageMutex.Release();
            _ = CommitChangesAsync();
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
                MetadataUpdated?.Invoke(this, artistMetadata.IntoList());
            else
                MetadataAdded?.Invoke(this, artistMetadata.IntoList());
        }

        /// <inheritdoc />
        public async Task RemoveArtist(ArtistMetadata artistMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(artistMetadata.Id, nameof(artistMetadata.Id));

            await _storageMutex.WaitAsync();
            var removed = _inMemoryMetadata.TryRemove(artistMetadata.Id, out _);
            _storageMutex.Release();

            if (removed)
            {
                _ = CommitChangesAsync();
                MetadataRemoved?.Invoke(this, artistMetadata.IntoList());
            }
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
            var allArtists = await GetArtists(offset, -1);
            var results = new List<ArtistMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.AlbumIds, nameof(item.AlbumIds));
                Guard.HasSizeGreaterThan(item.AlbumIds, 0, nameof(ArtistMetadata.AlbumIds));

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
            var allArtists = await GetArtists(0, -1);
            var results = new List<ArtistMetadata>();

            foreach (var item in allArtists)
            {
                Guard.IsNotNull(item.TrackIds, nameof(item.TrackIds));
                Guard.HasSizeGreaterThan(item.TrackIds, 0, nameof(ArtistMetadata.TrackIds));

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
        private async Task LoadDataFromDisk()
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, ArtistMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
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
            var bytes = MessagePackSerializer.Serialize(_inMemoryMetadata.Values.ToList(), MessagePack.Resolvers.ContractlessStandardResolver.Options);

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
    }
}
