using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Events
{
    /// <summary>
    /// Event args containing info about a changed collection.
    /// </summary>
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
