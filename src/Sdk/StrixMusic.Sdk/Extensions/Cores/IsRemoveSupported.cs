// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

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
                IPlayableCollectionGroupBase _ => ((ICorePlayableCollectionGroup)source).IsRemoveChildAvailableAsync(index),
                IAlbumCollectionBase _ => ((ICoreAlbumCollection)source).IsRemoveAlbumItemAvailableAsync(index),
                IArtistCollectionBase _ => ((ICoreArtistCollection)source).IsRemoveArtistItemAvailableAsync(index),
                IPlaylistCollectionBase _ => ((ICorePlaylistCollection)source).IsRemovePlaylistItemAvailableAsync(index),
                ITrackCollectionBase _ => ((ICoreTrackCollection)source).IsRemoveTrackAvailableAsync(index),
                IImageCollectionBase _ => ((ICoreImageCollection)source).IsRemoveImageAvailableAsync(index),
                IGenreCollectionBase _ => ((ICoreGenreCollection)source).IsRemoveGenreAvailableAsync(index),
                IUrlCollectionBase _ => ((ICoreUrlCollection)source).IsRemoveUrlAvailableAsync(index),
                _ => throw new NotSupportedException("Collection type not handled"),
            };
        }
    }
}