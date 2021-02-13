using System.Collections.Generic;

namespace OwlCore.Events
{
    /// <summary>
    /// Delegates changes to a collection
    /// </summary>
    /// <param name="sender">The source that fired this event.</param>
    /// <param name="addedItems">The items that were added to the collection.</param>
    /// <param name="removedItems">The items that were removed from the collection.</param>
    public delegate void CollectionChangedEventHandler<T>(object sender, IReadOnlyList<CollectionChangedItem<T>> addedItems, IReadOnlyList<CollectionChangedItem<T>> removedItems);
}