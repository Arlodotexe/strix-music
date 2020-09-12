using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Events;

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
            Items = Array.Empty<AbstractUIMetadata>();
        }

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public IEnumerable<AbstractUIMetadata> Items { get; protected set; }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode { get; protected set; }

        /// <summary>
        /// Fires when the <see cref="Items"/> are changed.
        /// </summary>
        public abstract event EventHandler<CollectionChangedEventArgs<AbstractUIMetadata>>? ItemsChanged;
    }
}
