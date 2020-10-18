using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OwlCore.AbstractUI
{
    /// <summary>
    /// Presents a list of metadata in a Grid or List.
    /// </summary>
    public class AbstractDataList : AbstractUIElement
    {
        /// <summary>
        /// Constructs a new instance of a <see cref="AbstractDataList"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="items">The items in this collection.</param>
        public AbstractDataList(string id, ObservableCollection<AbstractUIMetadata> items)
            : base(id)
        {
            Items = items;
        }

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public ObservableCollection<AbstractUIMetadata> Items { get; protected set; }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode { get; protected set; }
    }
}
