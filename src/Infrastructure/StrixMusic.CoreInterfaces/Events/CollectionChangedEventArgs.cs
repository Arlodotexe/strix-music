using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces
{
    /// <summary>
    /// Event args containing info about a changed collection.
    /// </summary>
    public class CollectionChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The items that were added to the collection.
        /// </summary>
        public IReadOnlyList<T>? AddedItems { get; }

        /// <summary>
        /// The items that were removed from the collection.
        /// </summary>
        public IReadOnlyList<T>? RemovedItems { get; }
    }
}
