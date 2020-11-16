using System;
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
            where TCollection : IPlayableCollectionBase
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            return typeof(TCollection) switch
            {
                IPlayableCollectionGroupBase _ => ((IPlayableCollectionGroupBase)source).TotalChildrenCount,
                IAlbumCollectionBase _ => ((IAlbumCollectionBase)source).TotalAlbumItemsCount,
                IArtistCollectionBase _ => ((IArtistCollectionBase)source).TotalArtistItemsCount,
                IPlaylistCollectionBase _ => ((IPlaylistCollectionBase)source).TotalPlaylistItemsCount,
                ITrackCollectionBase _ => ((ITrackCollectionBase)source).TotalTracksCount,
                _ => throw new ArgumentOutOfRangeException(nameof(source))
            };
        }
    }
}
