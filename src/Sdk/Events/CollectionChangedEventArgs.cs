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
        public CollectionChangedEventArgs(IReadOnlyList<T>? addedItems, IReadOnlyList<T>? removedItems)
        {
            AddedItems = addedItems ?? Array.Empty<T>();
            RemovedItems = removedItems ?? Array.Empty<T>();
        }

        /// <summary>
        /// The items that were added to the collection.
        /// </summary>
        public IReadOnlyList<T> AddedItems { get; }

        /// <summary>
        /// The items that were removed from the collection.
        /// </summary>
        public IReadOnlyList<T> RemovedItems { get; }
    }
}
