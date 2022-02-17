// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A <see cref="Comparer{T}"/> that can be either ascending or descending.
    /// </summary>
    /// <typeparam name="T">The type of items compared.</typeparam>
    public abstract class InversableComparer<T> : Comparer<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InversableComparer{T}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public InversableComparer(bool isDescending = false)
        {
            IsDescending = isDescending;
        }

        /// <summary>
        /// Gets or sets if items are compared in descending order.
        /// </summary>
        public bool IsDescending { get; set; }
    }
}
