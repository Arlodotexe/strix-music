// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

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
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public static Task<bool> IsRemoveAvailable<TCollection>(this TCollection source, int index, CancellationToken cancellationToken = default)
            where TCollection : class, ICollectionBase, ICoreMember
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return typeof(TCollection) switch
            {
                IPlayableCollectionGroupBase _ => ((ICorePlayableCollectionGroup)source).IsRemoveChildAvailableAsync(index, cancellationToken),
                IAlbumCollectionBase _ => ((ICoreAlbumCollection)source).IsRemoveAlbumItemAvailableAsync(index, cancellationToken),
                IArtistCollectionBase _ => ((ICoreArtistCollection)source).IsRemoveArtistItemAvailableAsync(index, cancellationToken),
                IPlaylistCollectionBase _ => ((ICorePlaylistCollection)source).IsRemovePlaylistItemAvailableAsync(index, cancellationToken),
                ITrackCollectionBase _ => ((ICoreTrackCollection)source).IsRemoveTrackAvailableAsync(index, cancellationToken),
                IImageCollectionBase _ => ((ICoreImageCollection)source).IsRemoveImageAvailableAsync(index, cancellationToken),
                IGenreCollectionBase _ => ((ICoreGenreCollection)source).IsRemoveGenreAvailableAsync(index, cancellationToken),
                IUrlCollectionBase _ => ((ICoreUrlCollection)source).IsRemoveUrlAvailableAsync(index, cancellationToken),
                _ => throw new NotSupportedException("Collection type not handled"),
            };
        }
    }
}
