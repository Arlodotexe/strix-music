// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the DatePublished />.
    /// </summary>
    /// <typeparam name="TAlbum">The <inheritdoc cref="IAlbumBase"/> to sort.</typeparam>
    public sealed class DatePublishedComparer<TAlbum> : InversableComparer<TAlbum>
        where TAlbum : IAlbumBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatePublishedComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public DatePublishedComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TAlbum x, TAlbum y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            int value = Nullable.Compare(x.DatePublished, y.DatePublished);
            return IsDescending ? -value : value;
        }
    }
}
