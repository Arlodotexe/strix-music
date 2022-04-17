// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the track number.
    /// </summary>
    public sealed class TrackNumberComparer<TTrack> : InversableComparer<TTrack>
        where TTrack : ITrackBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackNumberComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public TrackNumberComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TTrack x, TTrack y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            int value = Nullable.Compare(x.TrackNumber, y.TrackNumber);
            return IsDescending ? -value : value;
        }
    }
}
