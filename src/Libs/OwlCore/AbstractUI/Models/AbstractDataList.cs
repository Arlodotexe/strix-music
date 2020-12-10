﻿using System.Collections.Generic;
using System.Linq;

namespace OwlCore.AbstractUI.Models
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
        public AbstractDataList(string id, List<AbstractUIMetadata> items)
            : base(id)
        {
            Items = items;
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractDataList"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIMetadata this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public List<AbstractUIMetadata> Items { get; }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode { get; set; }
    }
}
