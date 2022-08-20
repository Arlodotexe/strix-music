using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Cores.Files.Services;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Cores.Files.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public sealed class FilesCoreLibrary : FilesCorePlayableCollectionGroupBase, ICoreLibrary
    {
        private readonly SemaphoreSlim _initSemaphore = new(1, 1);
        private IFileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public FilesCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            AttachEvents();
        }

        /// <inheritdoc/>
        public override async Task InitAsync(CancellationToken cancellationToken = default)
        {
            using var initReleaseRegistration = cancellationToken.Register(() => _initSemaphore.Release());
            await _initSemaphore.WaitAsync(cancellationToken);

            _fileMetadataManager = SourceCore.Cast<FilesCore>().FileMetadataManager;
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            AttachEvents(_fileMetadataManager);

            TotalAlbumItemsCount = await _fileMetadataManager.Albums.GetItemCount();
            TotalArtistItemsCount = await _fileMetadataManager.AlbumArtists.GetItemCount();
            TotalPlaylistItemsCount = await _fileMetadataManager.Playlists.GetItemCount();
            TotalTrackCount = await _fileMetadataManager.Tracks.GetItemCount();

            await base.InitAsync(cancellationToken);

            IsInitialized = true;

            _initSemaphore.Release();
        }

        private void AttachEvents()
        {
            base.TracksCountChanged += FilesCoreLibrary_TracksCountChanged;
            base.ArtistItemsCountChanged += FilesCoreLibrary_ArtistItemsCountChanged;
            base.AlbumItemsCountChanged += FilesCoreLibrary_AlbumItemsCountChanged;
            base.PlaylistItemsCountChanged += FilesCoreLibrary_PlaylistItemsCountChanged;
            base.ImagesCountChanged += FilesCoreLibrary_ImagesCountChanged;
        }

        private void DetachEvents()
        {
            base.TracksCountChanged -= FilesCoreLibrary_TracksCountChanged;
            base.ArtistItemsCountChanged -= FilesCoreLibrary_ArtistItemsCountChanged;
            base.AlbumItemsCountChanged -= FilesCoreLibrary_AlbumItemsCountChanged;
            base.PlaylistItemsCountChanged -= FilesCoreLibrary_PlaylistItemsCountChanged;
            base.ImagesCountChanged -= FilesCoreLibrary_ImagesCountChanged;
        }

        private void AttachEvents(IFileMetadataManager fileMetadataManager)
        {
            fileMetadataManager.Tracks.MetadataAdded += Tracks_MetadataAdded;
            fileMetadataManager.Albums.MetadataAdded += Albums_MetadataAdded;
            fileMetadataManager.AlbumArtists.MetadataAdded += Artists_MetadataAdded;
            fileMetadataManager.Playlists.MetadataAdded += Playlists_MetadataAdded;

            fileMetadataManager.Tracks.MetadataRemoved += Tracks_MetadataRemoved;
            fileMetadataManager.Albums.MetadataRemoved += Albums_MetadataRemoved;
            fileMetadataManager.AlbumArtists.MetadataRemoved += Artists_MetadataRemoved;
            fileMetadataManager.Playlists.MetadataRemoved += Playlists_MetadataRemoved;
        }

        private void DetachEvents(IFileMetadataManager fileMetadataManager)
        {
            fileMetadataManager.Tracks.MetadataAdded -= Tracks_MetadataAdded;
            fileMetadataManager.Albums.MetadataAdded -= Albums_MetadataAdded;
            fileMetadataManager.AlbumArtists.MetadataAdded -= Artists_MetadataAdded;
            fileMetadataManager.Playlists.MetadataAdded -= Playlists_MetadataAdded;

            fileMetadataManager.Tracks.MetadataRemoved -= Tracks_MetadataRemoved;
            fileMetadataManager.Albums.MetadataRemoved -= Albums_MetadataRemoved;
            fileMetadataManager.AlbumArtists.MetadataRemoved -= Artists_MetadataRemoved;
            fileMetadataManager.Playlists.MetadataRemoved -= Playlists_MetadataRemoved;
        }

        private void FilesCoreLibrary_PlaylistItemsCountChanged(object sender, int e)
        {
            PlaylistItemsCountChanged?.Invoke(sender, e);
        }

        private void FilesCoreLibrary_AlbumItemsCountChanged(object sender, int e)
        {
            AlbumItemsCountChanged?.Invoke(sender, e);
        }

        private void FilesCoreLibrary_ArtistItemsCountChanged(object sender, int e)
        {
            ArtistItemsCountChanged?.Invoke(sender, e);
        }

        private void FilesCoreLibrary_ImagesCountChanged(object sender, int e)
        {
            ImagesCountChanged?.Invoke(this, e);
        }

        private void FilesCoreLibrary_TracksCountChanged(object sender, int e)
        {
            TracksCountChanged?.Invoke(sender, e);
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
            var addedItems = new List<CollectionChangedItem<ICoreTrack>>();
            var removedItems = new List<CollectionChangedItem<ICoreTrack>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                TotalTrackCount--;
                TracksCountChanged?.Invoke(this, TotalTrackCount);

                if (!InstanceCache.Tracks.HasId(item.Id))
                    continue;

                removedItems.Add(new CollectionChangedItem<ICoreTrack>(InstanceCache.Tracks.GetOrCreate(item.Id, SourceCore, item), removedItems.Count));
                TracksChanged?.Invoke(this, addedItems, removedItems);

                InstanceCache.Tracks.Remove(item.Id);
            }
        }

        private void Artists_MetadataRemoved(object sender, IEnumerable<ArtistMetadata> e)
        {
            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();
            var removedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                TotalArtistItemsCount--;
                ArtistItemsCountChanged?.Invoke(this, TotalArtistItemsCount);

                if (!InstanceCache.Artists.HasId(item.Id))
                    continue;

                removedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(InstanceCache.Artists.GetOrCreate(item.Id, SourceCore, item), removedItems.Count));
                ArtistItemsChanged?.Invoke(this, addedItems, removedItems);

                InstanceCache.Artists.Remove(item.Id);
            }
        }

        private void Albums_MetadataRemoved(object sender, IEnumerable<AlbumMetadata> e)
        {
            var addedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();
            var removedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();

            foreach (var item in e)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                TotalAlbumItemsCount--;
                AlbumItemsCountChanged?.Invoke(this, TotalAlbumItemsCount);

                if (!InstanceCache.Albums.HasId(item.Id))
                    continue;

                removedItems.Add(new CollectionChangedItem<ICoreAlbumCollectionItem>(InstanceCache.Albums.GetOrCreate(item.Id, SourceCore, item), removedItems.Count));

                AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
                InstanceCache.Albums.Remove(item.Id);
            }
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
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var albumMetadata = await _fileMetadataManager.Albums.GetItemsAsync(offset, limit);

            foreach (var album in albumMetadata)
            {
                Guard.IsNotNull(album.Id, nameof(album.Id));

                yield return InstanceCache.Albums.GetOrCreate(album.Id, SourceCore, album);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var artistMetadata = await _fileMetadataManager.AlbumArtists.GetItemsAsync(offset, limit);

            foreach (var artist in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));

                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
        public override IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return AsyncEnumerable.Empty<ICoreUrl>();
        }
    }
}
