using System.Collections.Generic;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    public partial class RemoteCorePlayableCollectionGroupBase
    {
        private void AttachEvents(ICorePlayableCollectionGroup collection)
        {
            collection.NameChanged += OnNameChanged;
            collection.DescriptionChanged += OnDescriptionChanged;
            collection.PlaybackStateChanged += OnPlaybackStateChanged;
            collection.DurationChanged += OnDurationChanged;
            collection.LastPlayedChanged += OnLastPlayedChanged;

            collection.TracksCountChanged += OnTracksCountChanged;
            collection.ArtistItemsCountChanged += OnAlbumItemsCountChanged;
            collection.AlbumItemsCountChanged += OnAlbumItemsCountChanged;
            collection.PlaylistItemsCountChanged += OnPlaylistItemsCountChanged;
            collection.ChildrenCountChanged += OnChildrenCountChanged;
            collection.ImagesCountChanged += OnImagesCountChanged;
            collection.UrlsCountChanged += OnUrlsCountChanged;

            collection.IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
            collection.IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
            collection.IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            collection.IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
            collection.IsPlayPlaylistCollectionAsyncAvailableChanged += OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            collection.IsPausePlaylistCollectionAsyncAvailableChanged += OnIsPausePlaylistCollectionAsyncAvailableChanged;
            collection.IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            collection.IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;

            collection.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            collection.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
            collection.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        }

        private void DetachEvents(ICorePlayableCollectionGroup collection)
        {
            collection.NameChanged -= OnNameChanged;
            collection.DescriptionChanged -= OnDescriptionChanged;
            collection.PlaybackStateChanged -= OnPlaybackStateChanged;
            collection.DurationChanged -= OnDurationChanged;
            collection.LastPlayedChanged -= OnLastPlayedChanged;

            collection.TracksCountChanged -= OnTracksCountChanged;
            collection.ArtistItemsCountChanged -= OnAlbumItemsCountChanged;
            collection.AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
            collection.PlaylistItemsCountChanged -= OnPlaylistItemsCountChanged;
            collection.ChildrenCountChanged -= OnChildrenCountChanged;
            collection.ImagesCountChanged -= OnImagesCountChanged;
            collection.UrlsCountChanged -= OnUrlsCountChanged;

            collection.IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
            collection.IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
            collection.IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            collection.IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
            collection.IsPlayPlaylistCollectionAsyncAvailableChanged -= OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            collection.IsPausePlaylistCollectionAsyncAvailableChanged -= OnIsPausePlaylistCollectionAsyncAvailableChanged;
            collection.IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            collection.IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;

            collection.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            collection.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
            collection.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        }

        private void OnNameChanged(object sender, string e) => Name = e;

        private void OnDescriptionChanged(object sender, string? e) => Description = e;

        private void OnLastPlayedChanged(object sender, System.DateTime? e) => LastPlayed = e;

        private void OnDurationChanged(object sender, System.TimeSpan e) => Duration = e;

        private void OnPlaybackStateChanged(object sender, MediaPlayback.PlaybackState e) => PlaybackState = e;

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => IsChangeNameAsyncAvailable = e;

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => IsChangeDescriptionAsyncAvailable = e;

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => IsChangeDurationAsyncAvailable = e;

        private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayAlbumCollectionAsyncAvailable = e;

        private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseAlbumCollectionAsyncAvailable = e;

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayArtistCollectionAsyncAvailable = e;

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseArtistCollectionAsyncAvailable = e;

        private void OnIsPlayPlaylistCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayPlaylistCollectionAsyncAvailable = e;

        private void OnIsPausePlaylistCollectionAsyncAvailableChanged(object sender, bool e) => IsPausePlaylistCollectionAsyncAvailable = e;

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayTrackCollectionAsyncAvailable = e;

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseTrackCollectionAsyncAvailable = e;

        private void OnTracksCountChanged(object sender, int e) => TotalTrackCount = e;

        private void OnArtistItemsCountChanged(object sender, int e) => TotalArtistItemsCount = e;

        private void OnAlbumItemsCountChanged(object sender, int e) => TotalAlbumItemsCount = e;

        private void OnPlaylistItemsCountChanged(object sender, int e) => TotalPlaylistItemsCount = e;

        private void OnChildrenCountChanged(object sender, int e) => TotalChildrenCount = e;

        private void OnImagesCountChanged(object sender, int e) => TotalImageCount = e;

        private void OnUrlsCountChanged(object sender, int e) => TotalUrlCount = e;

        [RemoteMethod]
        private void RaiseTracksChanged(IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems) => TracksChanged?.Invoke(this, addedItems, removedItems);

        [RemoteMethod]
        private void RaisePlaylistItemsChanged(IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> removedItems) => PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);

        [RemoteMethod]
        private void RaiseAlbumItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> removedItems) => AlbumItemsChanged?.Invoke(this, addedItems, removedItems);

        [RemoteMethod]
        private void RaiseArtistItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> removedItems) => ArtistItemsChanged?.Invoke(this, addedItems, removedItems);

        [RemoteMethod]
        private void RaiseChildItemsChanged(IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> removedItems) => ChildItemsChanged?.Invoke(this, addedItems, removedItems);

        [RemoteMethod]
        private void RaiseImagesChanged(IReadOnlyList<CollectionChangedItem<ICoreImage>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreImage>> removedItems) => ImagesChanged?.Invoke(this, addedItems, removedItems);

        [RemoteMethod]
        private void RaiseUrlsChanged(IReadOnlyList<CollectionChangedItem<ICoreUrl>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreUrl>> removedItems) => UrlsChanged?.Invoke(this, addedItems, removedItems);
    }
}
