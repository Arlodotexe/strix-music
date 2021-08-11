using System;

namespace StrixMusic.Sdk.Data.Base
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