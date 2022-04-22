using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore;
using OwlCore.Events;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.Tests.Mock.AppModels;

namespace StrixMusic.Sdk.Tests;

public static class VerifyModelObjectTree
{
    public static void VerifyReturns<T>(IStrixDataRoot item)
    {
        Assert.IsInstanceOfType(item.Discoverables, typeof(T));
        Assert.IsInstanceOfType(item.RecentlyPlayed, typeof(T));
        Assert.IsInstanceOfType(item.Pins, typeof(T));
        Assert.IsInstanceOfType(item.Search, typeof(T));
        Assert.IsInstanceOfType(item.Library, typeof(T));
    }

    public async static Task VerifyReturnsAsync<T>(IPlayableCollectionGroup item)
    {
        await VerifyReturnsAsync<T>((IAlbumCollection)item);
        await VerifyReturnsAsync<T>((IArtistCollection)item);
        await VerifyReturnsAsync<T>((IPlaylistCollection)item);
        await VerifyReturnsAsync<T>((ITrackCollection)item);
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);

        item.ChildItemsChanged += OnChildItemsChanged;

        IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>>? added = null;
        IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>>? removed = null;

        await item.AddChildAsync(new MockPlayableCollectionGroup(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IPlayableCollectionGroup));
        added = null;
        
        await item.RemoveChildAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IPlayableCollectionGroup));
        removed = null;

        item.ChildItemsChanged -= OnChildItemsChanged;

        void OnChildItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(ILibrary item)
    {
        await VerifyReturnsAsync<T>((IPlayableCollectionGroup)item);
    }

    public async static Task VerifyReturnsAsync<T>(IDiscoverables item)
    {
        await VerifyReturnsAsync<T>((IPlayableCollectionGroup)item);
    }

    public async static Task VerifyReturnsAsync<T>(IRecentlyPlayed item)
    {
        await VerifyReturnsAsync<T>((IPlayableCollectionGroup)item);
    }

    public async static Task VerifyReturnsAsync<T>(ISearch item)
    {
        var results = await item.GetSearchResultsAsync(string.Empty);
        await VerifyReturnsAsync<T>(results);
        
        Assert.IsNotNull(item.SearchHistory);
        await VerifyReturnsAsync<T>(item.SearchHistory);
    }

    public async static Task VerifyReturnsAsync<T>(ISearchHistory item)
    {
        await VerifyReturnsAsync<T>((IPlayableCollectionGroup)item);
    }

    public async static Task VerifyReturnsAsync<T>(ISearchResults item)
    {
        await VerifyReturnsAsync<T>((IPlayableCollectionGroup)item);
    }

    public async static Task VerifyReturnsAsync<T>(IArtist item)
    {
        await VerifyReturnsAsync<T>((IGenreCollection)item);
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);
        await VerifyReturnsAsync<T>((ITrackCollection)item);
        await VerifyReturnsAsync<T>((IAlbumCollection)item);
        
        Assert.IsNotNull(item.RelatedItems);
        Assert.IsInstanceOfType(item.RelatedItems, typeof(T));
    }

    public async static Task VerifyReturnsAsync<T>(IAlbum item)
    {
        await VerifyReturnsAsync<T>((IGenreCollection)item);
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);
        await VerifyReturnsAsync<T>((ITrackCollection)item);
        await VerifyReturnsAsync<T>((IArtistCollection)item);
        
        Assert.IsNotNull(item.RelatedItems);
        Assert.IsInstanceOfType(item.RelatedItems, typeof(T));
    }

    public async static Task VerifyReturnsAsync<T>(IPlaylist item)
    {
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);
        await VerifyReturnsAsync<T>((ITrackCollection)item);
        
        Assert.IsNotNull(item.RelatedItems);
        Assert.IsInstanceOfType(item.RelatedItems, typeof(T));
    }

    public async static Task VerifyReturnsAsync<T>(ITrack item)
    {
        await VerifyReturnsAsync<T>((IGenreCollection)item);
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);
        await VerifyReturnsAsync<T>((IArtistCollection)item);
        
        Assert.IsNotNull(item.RelatedItems);
        Assert.IsInstanceOfType(item.RelatedItems, typeof(T));
    }

    public async static Task VerifyReturnsAsync<T>(IAlbumCollection item)
    {
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);

        item.AlbumItemsChanged += OnAlbumItemsChanged;

        IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>>? added = null;
        IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>>? removed = null;

        await item.AddAlbumItemAsync(new MockAlbum(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IAlbum));
        added = null;

        var albumItems = await item.GetAlbumItemsAsync(1, 0);
        Assert.IsInstanceOfType(albumItems[0], typeof(T));
        Assert.IsInstanceOfType(albumItems[0], typeof(IAlbum));

        await item.RemoveAlbumItemAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IAlbum));
        removed = null;

        await item.AddAlbumItemAsync(new MockAlbumCollection(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IAlbumCollection));
        added = null;

        var albumCollection = await item.GetAlbumItemsAsync(1, 0);
        Assert.IsInstanceOfType(albumCollection[0], typeof(T));
        Assert.IsInstanceOfType(albumCollection[0], typeof(IAlbumCollection));

        await item.RemoveAlbumItemAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IAlbumCollection));
        removed = null;

        item.AlbumItemsChanged -= OnAlbumItemsChanged;

        void OnAlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(IArtistCollection item)
    {
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);

        item.ArtistItemsChanged += OnArtistItemsChanged;

        IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>>? added = null;
        IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>>? removed = null;

        await item.AddArtistItemAsync(new MockArtist(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IArtist));
        added = null;

        var artistItems = await item.GetArtistItemsAsync(1, 0);
        Assert.IsInstanceOfType(artistItems[0], typeof(T));
        Assert.IsInstanceOfType(artistItems[0], typeof(IArtist));

        await item.RemoveArtistItemAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IArtist));
        removed = null;

        await item.AddArtistItemAsync(new MockArtistCollection(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IArtistCollection));
        added = null;

        var artistCollection = await item.GetArtistItemsAsync(1, 0);
        Assert.IsInstanceOfType(artistCollection[0], typeof(T));
        Assert.IsInstanceOfType(artistCollection[0], typeof(IArtistCollection));

        await item.RemoveArtistItemAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IArtistCollection));
        removed = null;

        item.ArtistItemsChanged -= OnArtistItemsChanged;

        void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(ITrackCollection item)
    {
        item.TracksChanged += OnChanged;

        IReadOnlyList<CollectionChangedItem<ITrack>>? added = null;
        IReadOnlyList<CollectionChangedItem<ITrack>>? removed = null;

        await item.AddTrackAsync(new MockTrack(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(ITrack));
        added = null;
        
        await item.RemoveTrackAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(ITrack));
        removed = null;

        item.TracksChanged -= OnChanged;

        void OnChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(IPlaylistCollection item)
    {
        await VerifyReturnsAsync<T>((IImageCollection)item);
        await VerifyReturnsAsync<T>((IUrlCollection)item);

        item.PlaylistItemsChanged += OnPlaylistItemsChanged;

        IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>>? added = null;
        IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>>? removed = null;

        await item.AddPlaylistItemAsync(new MockPlaylist(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IPlaylist));
        added = null;

        var playlistItems = await item.GetPlaylistItemsAsync(1, 0);
        Assert.IsInstanceOfType(playlistItems[0], typeof(T));
        Assert.IsInstanceOfType(playlistItems[0], typeof(IPlaylist));

        await item.RemovePlaylistItemAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IPlaylist));
        removed = null;

        await item.AddPlaylistItemAsync(new MockPlaylistCollection(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IPlaylistCollection));
        added = null;

        var playlistCollection = await item.GetPlaylistItemsAsync(1, 0);
        Assert.IsInstanceOfType(playlistCollection[0], typeof(T));
        Assert.IsInstanceOfType(playlistCollection[0], typeof(IPlaylistCollection));

        await item.RemovePlaylistItemAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IPlaylistCollection));
        removed = null;

        item.PlaylistItemsChanged -= OnPlaylistItemsChanged;

        void OnPlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(IUrlCollection item)
    {
        item.UrlsChanged += OnChanged;

        IReadOnlyList<CollectionChangedItem<IUrl>>? added = null;
        IReadOnlyList<CollectionChangedItem<IUrl>>? removed = null;

        await item.AddUrlAsync(new MockUrl(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IUrl));
        added = null;
        
        await item.RemoveUrlAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IUrl));
        removed = null;

        item.UrlsChanged -= OnChanged;

        void OnChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(IImageCollection item)
    {
        item.ImagesChanged += OnChanged;

        IReadOnlyList<CollectionChangedItem<IImage>>? added = null;
        IReadOnlyList<CollectionChangedItem<IImage>>? removed = null;

        await item.AddImageAsync(new MockImage(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IImage));
        added = null;
        
        await item.RemoveImageAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IImage));
        removed = null;

        item.ImagesChanged -= OnChanged;

        void OnChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }

    public async static Task VerifyReturnsAsync<T>(IGenreCollection item)
    {
        item.GenresChanged += OnChanged;

        IReadOnlyList<CollectionChangedItem<IGenre>>? added = null;
        IReadOnlyList<CollectionChangedItem<IGenre>>? removed = null;

        await item.AddGenreAsync(new MockGenre(), 0);
        Assert.IsNotNull(added);
        Assert.AreEqual(1, added.Count);
        Assert.IsInstanceOfType(added[0].Data, typeof(T));
        Assert.IsInstanceOfType(added[0].Data, typeof(IGenre));
        added = null;
        
        await item.RemoveGenreAsync(0);
        Assert.IsNotNull(removed);
        Assert.AreEqual(1, removed.Count);
        Assert.IsInstanceOfType(removed[0].Data, typeof(T));
        Assert.IsInstanceOfType(removed[0].Data, typeof(IGenre));
        removed = null;

        item.GenresChanged -= OnChanged;

        void OnChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
        {
            if (addedItems.Count > 0)
                added = addedItems;

            if (removedItems.Count > 0)
                removed = removedItems;
        }
    }
}
