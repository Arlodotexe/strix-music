using System;
using System.Collections.Generic;
using System.Text;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
    public static partial class General
    {

        /// <summary>
        /// Gets the total items count from an <see cref="ICoreMember"/> by casting it to the specified collection type.
        /// </summary>
        /// <typeparam name="TCollection">The collection type to check the item count.</typeparam>
        /// <returns>The number of items for the given collection.</returns>
        public static void SubItemsChanged<TCollection>(this TCollection source, CollectionChangedEventHandler<TCollection> callback)
            where TCollection : IPlayableCollectionBase
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            switch (typeof(TCollection))
            {
                case ICorePlayableCollectionGroup _:
                    ((ICorePlayableCollectionGroup)source).ChildItemsChanged += (CollectionChangedEventHandler<ICorePlayableCollectionGroup>)callback;
                    break;
                case ICoreAlbumCollection _:
                    ((ICoreAlbumCollection)source).AlbumItemsChanged += (CollectionChangedEventHandler<ICoreAlbumCollection>)callback;
                    break;
                case ICoreArtistCollection _:
                    ((ICoreArtistCollection)source).ArtistItemsChanged += (CollectionChangedEventHandler<ICoreArtistCollection>)callback;
                    break;
                case ICorePlaylistCollection _:
                    ((ICorePlaylistCollection)source).PlaylistItemsChanged += (CollectionChangedEventHandler<ICorePlaylistCollection>)callback;
                    break;
                case ICoreTrackCollection _:
                    ((ICoreTrackCollection)source).TrackItemsChanged += (CollectionChangedEventHandler<ICoreTrackCollection>)callback;
                    break;
                case IPlayableCollectionGroup _:
                    ((IPlayableCollectionGroup)source).ChildItemsChanged += (CollectionChangedEventHandler<IPlayableCollectionGroup>)callback;
                    break;
                case IAlbumCollection _:
                    ((IAlbumCollection)source).AlbumItemsChanged += (CollectionChangedEventHandler<IAlbumCollection>)callback;
                    break;
                case IArtistCollection _:
                    ((IArtistCollection)source).ArtistItemsChanged += (CollectionChangedEventHandler<IArtistCollection>)callback;
                    break;
                case IPlaylistCollection _:
                    ((IPlaylistCollection)source).PlaylistItemsChanged += (CollectionChangedEventHandler<IPlaylistCollection>)callback;
                    break;
                case ITrackCollection _:
                    ((ITrackCollection)source).TrackItemsChanged += (CollectionChangedEventHandler<ITrackCollection>)callback;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }
        }
    }
}
