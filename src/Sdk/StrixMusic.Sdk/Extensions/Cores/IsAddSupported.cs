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
        public static Task<bool> IsAddAvailable<TCollection>(this TCollection source, int index, CancellationToken cancellationToken = default)
            where TCollection : ICoreCollection
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return typeof(TCollection) switch
            {
                IPlayableCollectionGroupBase _ => ((ICorePlayableCollectionGroup)source).IsAddChildAvailableAsync(index, cancellationToken),
                IAlbumCollectionBase _ => ((ICoreAlbumCollection)source).IsAddAlbumItemAvailableAsync(index, cancellationToken),
                IArtistCollectionBase _ => ((ICoreArtistCollection)source).IsAddArtistItemAvailableAsync(index, cancellationToken),
                IPlaylistCollectionBase _ => ((ICorePlaylistCollection)source).IsAddPlaylistItemAvailableAsync(index, cancellationToken),
                ITrackCollectionBase _ => ((ICoreTrackCollection)source).IsAddTrackAvailableAsync(index, cancellationToken),
                IImageCollectionBase _ => ((ICoreImageCollection)source).IsAddImageAvailableAsync(index, cancellationToken),
                IGenreCollectionBase _ => ((ICoreGenreCollection)source).IsAddGenreAvailableAsync(index, cancellationToken),
                IUrlCollectionBase _ => ((ICoreUrlCollection)source).IsAddUrlAvailableAsync(index, cancellationToken),
                _ => throw new NotSupportedException("Collection type not handled"),
            };
        }
    }
}
