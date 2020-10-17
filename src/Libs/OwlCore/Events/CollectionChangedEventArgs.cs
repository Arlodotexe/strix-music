using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Events
{
    /// <summary>
    /// Event args containing info about a changed collection.
    /// </summary>
    /// <remarks>
    /// Note:
    /// If we're shuffling a collection around in a handler, we have to retains the original position of each item during alteration. A single event (vs one event per action) makes that easier.
    /// </remarks>
    public class CollectionChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs{T}"/> class.
        /// </summary>
        public CollectionChangedEventArgs(IReadOnlyList<CollectionChangedEventArgsItem<T>>? addedItems, IReadOnlyList<CollectionChangedEventArgsItem<T>>? removedItems)
        {
            AddedItems = addedItems ?? Array.Empty<CollectionChangedEventArgsItem<T>>();
            RemovedItems = removedItems ?? Array.Empty<CollectionChangedEventArgsItem<T>>();
        }

        /// <summary>
        /// The items that were added to the collection.
        /// </summary>
        public IReadOnlyList<CollectionChangedEventArgsItem<T>> AddedItems { get; }

        /// <summary>
        /// The items that were removed from the collection.
        /// </summary>
        public IReadOnlyList<CollectionChangedEventArgsItem<T>> RemovedItems { get; }
    }
}