// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// An <see cref="IPlayableBase"/> that belongs to a playable collection.
    /// </summary>
    public interface IPlayableCollectionItem : IPlayableBase, ICollectionItemBase, IAsyncDisposable
    {
        /// <summary>
        /// The date this item was added to a collection. If unknown, value is null.
        /// </summary>
        /// <remarks>
        /// This property has no counterpart "changed" events or supporting properties.
        /// Since the item must be added to a collection for the data to change, a new instance with updated data would be used.
        /// </remarks>
        DateTime? AddedAt { get; }
    }
}
