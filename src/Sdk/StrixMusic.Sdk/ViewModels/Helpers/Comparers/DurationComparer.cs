// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration/>.
    /// </summary>
    public sealed class DurationComparer<TPlayableBase> : InversableComparer<TPlayableBase>
        where TPlayableBase : IPlayableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DurationComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public DurationComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TPlayableBase? x, TPlayableBase? y)
        {
            int value = TimeSpan.Compare(x?.Duration ?? TimeSpan.Zero, y?.Duration ?? TimeSpan.Zero);
            return IsDescending ? -value : value;
        }
    }
}
