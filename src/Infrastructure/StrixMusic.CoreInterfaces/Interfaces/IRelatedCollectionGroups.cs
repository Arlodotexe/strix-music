using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Contains information about related items in a collection.
    /// </summary>
    public interface IRelatedCollectionGroups
    {
        /// <summary>
        /// The related <see cref="IPlayableCollectionGroup"/>s in this collection group.
        /// </summary>
        IReadOnlyList<IPlayableCollectionGroup> RelatedItems { get; }

        /// <summary>
        /// The total number of available <see cref="RelatedItems"/>.
        /// </summary>
        int TotalRelatedItemsCount { get; }

        /// <summary>
        /// Populates a set of <see cref="RelatedItems"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateRelatedItemsAsync(int limit, int offset = 0);

        /// <summary>
        /// Fires when <see cref="RelatedItems"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;
    }
}
