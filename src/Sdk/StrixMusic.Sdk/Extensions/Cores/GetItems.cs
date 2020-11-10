using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
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
        public static IAsyncEnumerable<TResult> GetItems<TCollection, TResult>(this ICorePlayableCollection source, int limit, int offset)
            where TCollection : ICoreMember, IPlayableCollectionBase
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return typeof(TCollection) switch
            {
                IPlayableCollectionGroupBase _ => (IAsyncEnumerable<TResult>)((ICorePlayableCollectionGroup)source).GetChildrenAsync(limit, offset),
                IAlbumCollectionBase _ => (IAsyncEnumerable<TResult>)((ICoreAlbumCollection)source).GetAlbumItemsAsync(limit, offset),
                IArtistCollectionBase _ => (IAsyncEnumerable<TResult>)((ICoreArtistCollection)source).GetArtistItemsAsync(limit, offset),
                IPlaylistCollectionBase _ => (IAsyncEnumerable<TResult>)((ICorePlaylistCollection)source).GetPlaylistItemsAsync(limit, offset),
                ITrackCollectionBase _ => (IAsyncEnumerable<TResult>)((ICoreTrackCollection)source).GetTracksAsync(limit, offset),
                _ => throw new NotSupportedException("Collection type not handled"),
            };
        }
    }
}