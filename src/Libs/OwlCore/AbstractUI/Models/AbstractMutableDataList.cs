using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Events;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// A <see cref="AbstractDataList"/> that can be changed by the user.
    /// </summary>
    public class AbstractMutableDataList : AbstractDataList
    {
        private readonly CollectionChangedEventItem<AbstractUIMetadata>[] _emptyAbstractUIArray = Array.Empty<CollectionChangedEventItem<AbstractUIMetadata>>();

        /// <summary>
        /// Creates a new instance of <see cref="AbstractMutableDataList"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this item.</param>
        /// <param name="items">The initial items for this collection.</param>
        public AbstractMutableDataList(string id, List<AbstractUIMetadata> items)
            : base(id, items)
        {
        }

        /// <summary>
        /// Called when the user wants to add a new item in the list.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        public void RequestNewItem()
        {
            AddRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires when <see cref="RequestNewItem"/> is called.
        /// </summary>
        public event EventHandler? AddRequested;

        /// <summary>
        /// Adds an item from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(AbstractUIMetadata item)
        {
            InsertItem(item, Items.Count);
        }

        /// <summary>
        /// Inserts an item into <see cref="AbstractDataList.Items"/> at a specific index.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to add.</param>
        public void InsertItem(AbstractUIMetadata item, int index)
        {
            Items.Add(item);

            var addedItems = new List<CollectionChangedEventItem<AbstractUIMetadata>>
            {
                new CollectionChangedEventItem<AbstractUIMetadata>(item, index)
            };

            ItemsChanged?.Invoke(this, addedItems, _emptyAbstractUIArray);
        }

        /// <summary>
        /// Removes an item from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="item">The item being removed</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public void RemoveItem(AbstractUIMetadata item)
        {
            var index = Items.IndexOf(item);
            RemoveItemAt(index);
        }

        /// <summary>
        /// Removes an item at a specific index from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public void RemoveItemAt(int index)
        {
            var item = Items.ElementAt(index);
            Items.RemoveAt(index);

            var removedItems = new List<CollectionChangedEventItem<AbstractUIMetadata>>
            {
                new CollectionChangedEventItem<AbstractUIMetadata>(item, index)
            };

            ItemsChanged?.Invoke(this, _emptyAbstractUIArray, removedItems);
        }

        /// <summary>
        /// Fires when <see cref="RemoveItem"/> is called.
        /// </summary>
        public event CollectionChangedEventHandler<AbstractUIMetadata>? ItemsChanged;
    }
}
