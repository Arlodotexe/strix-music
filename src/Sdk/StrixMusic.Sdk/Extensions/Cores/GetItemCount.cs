using System;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
    public static partial class Cores
    {
        /// <summary>
        /// Gets the total items count from an <see cref="ICoreMember"/> by casting it to the specified collection type.
        /// </summary>
        /// <typeparam name="TCollection">The collection type to check the item count.</typeparam>
        /// <returns>The number of items for the given collection.</returns>
        public static int GetItemsCount<TCollection>(this ICoreMember source)
            where TCollection : ICollectionBase
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (typeof(TCollection) == typeof(IPlayableCollectionGroup))
                return ((IPlayableCollectionGroupBase)source).TotalChildrenCount;

            if (typeof(TCollection) == typeof(IAlbumCollection))
                return ((IAlbumCollectionBase)source).TotalAlbumItemsCount;

            if (typeof(TCollection) == typeof(IArtistCollection))
                return ((IArtistCollectionBase)source).TotalArtistItemsCount;

            if (typeof(TCollection) == typeof(IPlaylistCollection))
                return ((IPlaylistCollectionBase)source).TotalPlaylistItemsCount;

            if (typeof(TCollection) == typeof(ITrackCollection))
                return ((ITrackCollectionBase)source).TotalTrackCount;

            if (typeof(TCollection) == typeof(IImageCollection))
                return ((IImageCollectionBase)source).TotalImageCount;

            if (typeof(TCollection) == typeof(IGenreCollection))
                return ((IGenreCollectionBase)source).TotalGenreCount;

            throw new ArgumentOutOfRangeException(nameof(source));
        }
    }
}
