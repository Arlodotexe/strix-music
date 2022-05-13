// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Extensions
{
    /// <summary>
    /// Extension methods for generic operations on core members.
    /// </summary>
    public static partial class Cores
    {
        /// <summary>
        /// Gets items from a specific collection type in an object that derives multiple collection types.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection we're handling.</typeparam>
        /// <typeparam name="TResult">The type of the returned items.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of the requested items.</returns>
        public static IAsyncEnumerable<TResult> GetItems<TCollection, TResult>(this ICoreCollection source, int limit, int offset)
            where TCollection : ICoreMember, ICollectionBase
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (typeof(TCollection) == typeof(ICorePlayableCollectionGroup))
                return (IAsyncEnumerable<TResult>)((ICorePlayableCollectionGroup)source).GetChildrenAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICoreAlbumCollection))
                return (IAsyncEnumerable<TResult>)((ICoreAlbumCollection)source).GetAlbumItemsAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICoreArtistCollection))
                return (IAsyncEnumerable<TResult>)((ICoreArtistCollection)source).GetArtistItemsAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICorePlaylistCollection))
                return (IAsyncEnumerable<TResult>)((ICorePlaylistCollection)source).GetPlaylistItemsAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICoreTrackCollection))
                return (IAsyncEnumerable<TResult>)((ICoreTrackCollection)source).GetTracksAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICoreImageCollection))
                return (IAsyncEnumerable<TResult>)((ICoreImageCollection)source).GetImagesAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICoreGenreCollection))
                return (IAsyncEnumerable<TResult>)((ICoreGenreCollection)source).GetGenresAsync(limit, offset);

            if (typeof(TCollection) == typeof(ICoreUrlCollection))
                return (IAsyncEnumerable<TResult>)((ICoreUrlCollection)source).GetUrlsAsync(limit, offset);

            throw new NotSupportedException("Collection type not handled");
        }
    }
}
