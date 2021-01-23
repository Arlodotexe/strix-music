using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
    public static partial class Cores
    {
        /// <summary>
        /// Checks a collection for support for adding an item at a specific index.
        /// </summary>
        /// <typeparam name="TCollection">The type of collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="index">The index to check.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public static Task<bool> IsRemoveAvailable<TCollection>(this TCollection source, int index)
            where TCollection : class, ICollectionBase, ICoreMember
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return typeof(TCollection) switch
            {
                IPlayableCollectionGroupBase _ => ((ICorePlayableCollectionGroup)source).IsRemoveChildAvailable(index),
                IAlbumCollectionBase _ => ((ICoreAlbumCollection)source).IsRemoveAlbumItemAvailable(index),
                IArtistCollectionBase _ => ((ICoreArtistCollection)source).IsRemoveArtistItemAvailable(index),
                IPlaylistCollectionBase _ => ((ICorePlaylistCollection)source).IsRemovePlaylistItemAvailable(index),
                ITrackCollectionBase _ => ((ICoreTrackCollection)source).IsRemoveTrackAvailable(index),
                IImageCollectionBase _ => ((ICoreImageCollection)source).IsRemoveImageAvailable(index),
                _ => throw new NotSupportedException("Collection type not handled"),
            };
        }
    }
}