using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Cores.Files.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Cores.Files.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public sealed class FilesCoreLibrary : FilesCorePlayableCollectionGroupBase, ICoreLibrary
    {
        private IFileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public FilesCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
        }

        /// <inheritdoc/>
        public override async Task InitAsync()
        {
            IsInitialized = true;

            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();

            TotalAlbumItemsCount = await _fileMetadataManager.Albums.GetItemCount();
            TotalArtistItemsCount = await _fileMetadataManager.Artists.GetItemCount();
            TotalPlaylistItemsCount = await _fileMetadataManager.Playlists.GetItemCount();
            TotalTrackCount = await _fileMetadataManager.Tracks.GetItemCount();

            await base.InitAsync();

            AttachEvents();
        }

        private void AttachEvents()
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            _fileMetadataManager.Tracks.MetadataAdded += Tracks_MetadataAdded;
            _fileMetadataManager.Albums.MetadataAdded += Albums_MetadataAdded;
            _fileMetadataManager.Artists.MetadataAdded += Artists_MetadataAdded;
            _fileMetadataManager.Playlists.MetadataAdded += Playlists_MetadataAdded;

            _fileMetadataManager.Tracks.MetadataRemoved += Tracks_MetadataRemoved;
            _fileMetadataManager.Albums.MetadataRemoved += Albums_MetadataRemoved;
            _fileMetadataManager.Artists.MetadataRemoved += Artists_MetadataRemoved;
            _fileMetadataManager.Playlists.MetadataRemoved += Playlists_MetadataRemoved;
        }

        private void DetachEvents()
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            _fileMetadataManager.Tracks.MetadataAdded -= Tracks_MetadataAdded;
            _fileMetadataManager.Albums.MetadataAdded -= Albums_MetadataAdded;
            _fileMetadataManager.Artists.MetadataAdded -= Artists_MetadataAdded;
            _fileMetadataManager.Playlists.MetadataAdded -= Playlists_MetadataAdded;

            _fileMetadataManager.Tracks.MetadataRemoved -= Tracks_MetadataRemoved;
            _fileMetadataManager.Albums.MetadataRemoved -= Albums_MetadataRemoved;
            _fileMetadataManager.Artists.MetadataRemoved -= Artists_MetadataRemoved;
            _fileMetadataManager.Playlists.MetadataRemoved -= Playlists_MetadataRemoved;
        }

        private void Playlists_MetadataAdded(object sender, IEnumerable<PlaylistMetadata> e)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICorePlaylistCollectionItem>>();
            var addedItems = new List<CollectionChangedItem<ICorePlaylistCollectionItem>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                addedItems.Add(new CollectionChangedItem<ICorePlaylistCollectionItem>(InstanceCache.Playlists.GetOrCreate(item.Id, SourceCore, item), addedItems.Count));
            }

            TotalPlaylistItemsCount += addedItems.Count - removedItems.Count;
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
            PlaylistItemsCountChanged?.Invoke(this, TotalPlaylistItemsCount);
        }

        private void Tracks_MetadataAdded(object sender, IEnumerable<TrackMetadata> e)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICoreTrack>>();
            var addedItems = new List<CollectionChangedItem<ICoreTrack>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                addedItems.Add(new CollectionChangedItem<ICoreTrack>(InstanceCache.Tracks.GetOrCreate(item.Id, SourceCore, item), addedItems.Count));
            }

            TotalTrackCount += addedItems.Count - removedItems.Count;
            TracksChanged?.Invoke(this, addedItems, removedItems);
            TracksCountChanged?.Invoke(this, TotalTrackCount);
        }

        private void Artists_MetadataAdded(object sender, IEnumerable<ArtistMetadata> e)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();
            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                addedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(InstanceCache.Artists.GetOrCreate(item.Id, SourceCore, item), addedItems.Count));
            }

            TotalArtistItemsCount += addedItems.Count - removedItems.Count;
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
            ArtistItemsCountChanged?.Invoke(this, TotalAlbumItemsCount);
        }

        private void Albums_MetadataAdded(object sender, IEnumerable<AlbumMetadata> e)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var removedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();
            var addedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));
                addedItems.Add(new CollectionChangedItem<ICoreAlbumCollectionItem>(InstanceCache.Albums.GetOrCreate(item.Id, SourceCore, item), addedItems.Count));
            }

            TotalAlbumItemsCount += addedItems.Count - removedItems.Count;
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
            AlbumItemsCountChanged?.Invoke(this, TotalAlbumItemsCount);
        }

        private void Tracks_MetadataRemoved(object sender, IEnumerable<TrackMetadata> e)
        {
            // TODO. Need to get the index of each item being removed.
            // Remember to remove from instance cache and dispose the objects being removed after emitted.
        }

        private void Artists_MetadataRemoved(object sender, IEnumerable<ArtistMetadata> e)
        {
            // TODO. Need to get the index of each item being removed.
            // Remember to remove from instance cache and dispose the objects being removed after emitted.
        }

        private void Albums_MetadataRemoved(object sender, IEnumerable<AlbumMetadata> e)
        {
            // TODO. Need to get the index of each item being removed.
            // Remember to remove from instance cache and dispose the objects being removed after emitted.
        }

        private void Playlists_MetadataRemoved(object sender, IEnumerable<PlaylistMetadata> e)
        {
            // TODO. Need to get the index of each item being removed.
            // Remember to remove from instance cache and dispose the objects being removed after emitted.
        }

        /// <summary>
        /// Determines if collection base is initialized or not.
        /// </summary>
        public override bool IsInitialized { get; protected set; }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "library";

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Library";

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />?
        public override event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc />
        public override event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public override event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public override event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public override event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public override event EventHandler<int>? ChildrenCountChanged;

        /// <inheritdoc />
        public override event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var playlistsMetadata = await _fileMetadataManager.Playlists.GetItemsAsync(offset, limit);

            foreach (var playList in playlistsMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(playList.Id, nameof(playList.Id));

                yield return InstanceCache.Playlists.GetOrCreate(playList.Id, SourceCore, playList);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var albumMetadata = await _fileMetadataManager.Albums.GetItemsAsync(offset, limit);

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
            var artistMetadata = await _fileMetadataManager.Artists.GetItemsAsync(offset, limit);

            foreach (var artist in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));

                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var artistMetadata = await _fileMetadataManager.Tracks.GetItemsAsync(offset, limit);

            foreach (var track in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(track.Id, nameof(track.Id));
                yield return InstanceCache.Tracks.GetOrCreate(track.Id, SourceCore, track);
            }
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICoreUrl>();
        }

        /// <inheritdoc/>
        public override ValueTask DisposeAsync()
        {
            DetachEvents();
            return base.DisposeAsync();
        }
    }
}
