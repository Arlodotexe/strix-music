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
        DateTime? AddedAt { get; }
    }
}