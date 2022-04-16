// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;

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
