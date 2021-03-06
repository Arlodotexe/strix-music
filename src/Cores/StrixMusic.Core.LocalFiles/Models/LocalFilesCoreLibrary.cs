using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public class LocalFilesCoreLibrary : LocalFilesCorePlayableCollectionGroupBase, ICoreLibrary
    {
        private readonly SemaphoreSlim _batchMutex;
        private readonly List<(bool IsAdded, TrackMetadata Data)> _batchTracksToUpdate;
        private readonly List<(bool IsAdded, ArtistMetadata Data)> _batchArtistsToUpdate;
        private readonly List<(bool IsAdded, AlbumMetadata Data)> _batchAlbumsToUpdate;
        private IFileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public LocalFilesCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            _batchTracksToUpdate = new List<(bool IsAdded, TrackMetadata Data)>();
            _batchArtistsToUpdate = new List<(bool IsAdded, ArtistMetadata Data)>();
            _batchAlbumsToUpdate = new List<(bool IsAdded, AlbumMetadata Data)>();

            _batchMutex = new SemaphoreSlim(1, 1);
        }

        /// <inheritdoc/>
        public override async Task InitAsync()
        {
            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();

            IsInitialized = true;

            await base.InitAsync();

            AttachEvents();
        }

        private void AttachEvents()
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            _fileMetadataManager.Tracks.MetadataAdded += Tracks_MetadataAdded;
            _fileMetadataManager.Albums.MetadataAdded += Albums_MetadataAdded;
            _fileMetadataManager.Artists.MetadataAdded += Artists_MetadataAdded;

            _fileMetadataManager.Tracks.MetadataRemoved += Tracks_MetadataRemoved;
            _fileMetadataManager.Albums.MetadataRemoved += Albums_MetadataRemoved;
            _fileMetadataManager.Artists.MetadataRemoved += Artists_MetadataRemoved;
        }

        private void DetachEvents()
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            _fileMetadataManager.Tracks.MetadataAdded -= Tracks_MetadataAdded;
            _fileMetadataManager.Albums.MetadataAdded -= Albums_MetadataAdded;
            _fileMetadataManager.Artists.MetadataAdded -= Artists_MetadataAdded;

            _fileMetadataManager.Tracks.MetadataRemoved -= Tracks_MetadataRemoved;
            _fileMetadataManager.Albums.MetadataRemoved -= Albums_MetadataRemoved;
            _fileMetadataManager.Artists.MetadataRemoved -= Artists_MetadataRemoved;
        }

        private async void Tracks_MetadataAdded(object sender, IEnumerable<TrackMetadata> e)
        {
            await _batchMutex.WaitAsync();
            foreach (var metadata in e)
                _batchTracksToUpdate.Add((true, metadata));

            _batchMutex.Release();

            await CommitItemsChanged();
        }

        private async void Artists_MetadataAdded(object sender, IEnumerable<ArtistMetadata> e)
        {
            await _batchMutex.WaitAsync();
            foreach (var metadata in e)
                _batchArtistsToUpdate.Add((true, metadata));

            _batchMutex.Release();

            await CommitItemsChanged();
        }

        private async void Albums_MetadataAdded(object sender, IEnumerable<AlbumMetadata> e)
        {
            await _batchMutex.WaitAsync();
            foreach (var metadata in e)
                _batchAlbumsToUpdate.Add((true, metadata));

            _batchMutex.Release();

            await CommitItemsChanged();
        }

        private async void Tracks_MetadataRemoved(object sender, IEnumerable<TrackMetadata> e)
        {
            await _batchMutex.WaitAsync();
            foreach (var metadata in e)
                _batchTracksToUpdate.Add((false, metadata));

            _batchMutex.Release();

            await CommitItemsChanged();
        }

        private async void Artists_MetadataRemoved(object sender, IEnumerable<ArtistMetadata> e)
        {
            await _batchMutex.WaitAsync();
            foreach (var metadata in e)
                _batchArtistsToUpdate.Add((false, metadata));

            _batchMutex.Release();
            await CommitItemsChanged();
        }

        private async void Albums_MetadataRemoved(object sender, IEnumerable<AlbumMetadata> e)
        {
            await _batchMutex.WaitAsync();
            foreach (var metadata in e)
                _batchAlbumsToUpdate.Add((false, metadata));

            _batchMutex.Release();

            await CommitItemsChanged();
        }

        private async Task CommitItemsChanged()
        {
            if (_batchAlbumsToUpdate.Count + _batchArtistsToUpdate.Count + _batchTracksToUpdate.Count < 100 && !await Flow.Debounce($"{SourceCore.InstanceId}.{Id}", TimeSpan.FromSeconds(2)))
                return;

            await _batchMutex.WaitAsync();

            HandleChangedTracks();
            HandleChangedArtists();
            HandleChangedAlbums();

            _batchMutex.Release();
        }

        private void HandleChangedTracks()
        {
            if (_batchTracksToUpdate.Count == 0)
                return;

            var addedItems = new List<CollectionChangedItem<ICoreTrack>>();
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICoreTrack>>();

            foreach (var item in _batchTracksToUpdate)
            {
                Guard.IsNotNullOrWhiteSpace(item.Data.Id, nameof(item.Data.Id));

                if (item.IsAdded)
                {
                    addedItems.Add(new CollectionChangedItem<ICoreTrack>(InstanceCache.Tracks.GetOrCreate(item.Data.Id, SourceCore, item.Data), addedItems.Count));
                }
                else
                {
                    // TODO. Need to get the index of each item being removed.
                    // Remember to remove from instance cache and dispose the objects being removed after emitted.
                }
            }

            _batchTracksToUpdate.Clear();

            TotalTracksCount += addedItems.Count - removedItems.Count;
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void HandleChangedArtists()
        {
            if (_batchArtistsToUpdate.Count == 0)
                return;

            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();

            foreach (var item in _batchArtistsToUpdate)
            {
                Guard.IsNotNullOrWhiteSpace(item.Data.Id, nameof(item.Data.Id));

                if (item.IsAdded)
                {
                    addedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(InstanceCache.Artists.GetOrCreate(item.Data.Id, SourceCore, item.Data), addedItems.Count));
                }
                else
                {
                    // TODO. Need to get the index of each item being removed.
                    // Remember to remove from instance cache and dispose the objects being removed after emitted.
                }
            }

            _batchArtistsToUpdate.Clear();

            TotalArtistItemsCount += addedItems.Count - removedItems.Count;
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void HandleChangedAlbums()
        {
            if (_batchAlbumsToUpdate.Count == 0)
                return;

            var addedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();

            foreach (var item in _batchAlbumsToUpdate)
            {
                Guard.IsNotNullOrWhiteSpace(item.Data.Id, nameof(item.Data.Id));

                if (item.IsAdded)
                {
                    addedItems.Add(new CollectionChangedItem<ICoreAlbumCollectionItem>(InstanceCache.Albums.GetOrCreate(item.Data.Id, SourceCore, item.Data), addedItems.Count));
                }
                else
                {
                    // TODO. Need to get the index of each item being removed.
                    // Remember to remove from instance cache and dispose the objects being removed after emitted.
                }
            }

            _batchAlbumsToUpdate.Clear();

            TotalAlbumItemsCount += addedItems.Count - removedItems.Count;
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <summary>
        /// Determines if collection base is initialized or not.
        /// </summary>
        public override bool IsInitialized { get; protected set; }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "library";

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Library";

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />?
        public override event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var playlistsMetadata = await _fileMetadataManager.Playlists.GetPlaylistsMetadata(offset, limit);

            foreach (var playList in playlistsMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(playList.Id, nameof(playList.Id));

                yield return InstanceCache.PlayLists.GetOrCreate(playList.Id, SourceCore, playList);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var albumMetadata = await _fileMetadataManager.Albums.GetAlbums(offset, limit);

            foreach (var album in albumMetadata)
            {
                Guard.IsNotNull(album.Id, nameof(album.Id));

                var tracks = await _fileMetadataManager.Tracks.GetTracksByAlbumId(album.Id, 0, 1);
                var track = tracks.FirstOrDefault();

                Guard.IsNotNullOrWhiteSpace(track?.Id, nameof(track.Id));

                yield return InstanceCache.Albums.GetOrCreate(album.Id, SourceCore, album);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var artistMetadata = await _fileMetadataManager.Artists.GetArtists(offset, limit);

            foreach (var artist in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));

                if (artist.ImagePath != null)
                    yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist);

                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var artistMetadata = await _fileMetadataManager.Tracks.GetTracks(offset, limit);

            foreach (var track in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(track.Id, nameof(track.Id));
                yield return InstanceCache.Tracks.GetOrCreate(track.Id, SourceCore, track);
            }
        }
    }
}
