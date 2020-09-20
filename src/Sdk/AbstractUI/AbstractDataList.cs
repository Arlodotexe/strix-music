using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a list of metadata in a Grid or List.
    /// </summary>
    public abstract class AbstractDataList : AbstractUIElement
    {
        /// <summary>
        /// Constructs a new instance of a <see cref="AbstractDataList"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        protected AbstractDataList(string id)
            : base(id)
        {
            Items = new ObservableCollection<AbstractUIMetadata>();
        }

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public ObservableCollection<AbstractUIMetadata> Items { get; protected set; }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode { get; protected set; }
    }
}
