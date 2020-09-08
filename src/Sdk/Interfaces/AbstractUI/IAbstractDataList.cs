using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// A list of items to be shown to the user in a Grid or List
    /// </summary>
    public interface IAbstractDataList : IAbstractUIMetadata
    {
        /// <summary>
        /// The items in this collection.
        /// </summary>
        public IEnumerable<IAbstractUIMetadata> Items { get; }

        /// <inheritdoc cref="AbstractDataListPrefferedDisplayMode"/>
        public AbstractDataListPrefferedDisplayMode PrefferedDisplayMode { get; }

        /// <summary>
        /// Fires when the <see cref="Items"/> are changed.
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<IAbstractUIMetadata>> ItemsChanged;
    }
}
