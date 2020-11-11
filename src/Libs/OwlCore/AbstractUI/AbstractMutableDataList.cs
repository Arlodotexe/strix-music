using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Events;

namespace OwlCore.AbstractUI
{
    /// <summary>
    /// A <see cref="AbstractDataList"/> that can be changed by the user.
    /// </summary>
    public class AbstractMutableDataList : AbstractDataList
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractMutableDataList"/>.
        /// </summary>
        /// <param name="id"></param>
        protected AbstractMutableDataList(string id)
            : base(id, new ObservableCollection<AbstractUIMetadata>())
        {
        }

        /// <summary>
        /// Called when the user wants to add a new item in the list. Behavior is defined by the core.
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
            var index = Items.IndexOf(item);
            InsertItem(item, index);
        }

        /// <summary>
        /// Inserts an item into <see cref="AbstractDataList.Items"/> at a specific index..
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to add.</param>
        public void InsertItem(AbstractUIMetadata item, int index)
        {
            Items.Add(item);

            var removedItems = Array.Empty<CollectionChangedEventItem<AbstractUIMetadata>>();

            var addedItems = new List<CollectionChangedEventItem<AbstractUIMetadata>>()
            {
                new CollectionChangedEventItem<AbstractUIMetadata>(item, index)
            };

            ItemAdded?.Invoke(this, addedItems, removedItems);
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

            var removedItems = new List<CollectionChangedEventItem<AbstractUIMetadata>>()
            {
                new CollectionChangedEventItem<AbstractUIMetadata>(item, index)
            };

            var addedItems = Array.Empty<CollectionChangedEventItem<AbstractUIMetadata>>();

            ItemRemoved?.Invoke(this,addedItems, removedItems);
        }

        /// <summary>
        /// Fires when <see cref="RemoveItem"/> is called.
        /// </summary>
        public event CollectionChangedEventHandler<AbstractUIMetadata>? ItemRemoved;

        /// <summary>
        /// Fired when a new item is added to the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        public event CollectionChangedEventHandler<AbstractUIMetadata>? ItemAdded;
    }
}
