using System.Collections.Generic;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    public partial class RemoteCorePlayableCollectionGroupBase
    {

        private void AttachEvents()
        {
            if (_corePlayableCollection is null)
                return;

            _corePlayableCollection.NameChanged += OnNameChanged;
            _corePlayableCollection.DescriptionChanged += OnDescriptionChanged;
        }

        private void DetachEvents()
        {

        }

        private void OnNameChanged(object sender, string e) => RaiseNameChanged(e);

        [RemoteMethod]
        private void RaiseNameChanged(string name) => NameChanged?.Invoke(this, name);

        private void OnDescriptionChanged(object sender, string? e) => RaiseDescriptionChanged(e);

        [RemoteMethod]
        private void RaiseDescriptionChanged(string? description) => DescriptionChanged?.Invoke(this, description);

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
