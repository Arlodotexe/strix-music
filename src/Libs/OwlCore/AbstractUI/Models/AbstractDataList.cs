using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a sequence of metadata that is presented to the user, such as a List or Grid.
    /// </summary>
    public class AbstractDataList : AbstractUIElement
    {
        private readonly CollectionChangedItem<AbstractUIMetadata>[] _emptyAbstractUiArray = Array.Empty<CollectionChangedItem<AbstractUIMetadata>>();
        private bool _isUserEditingEnabled;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractDataList"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this item.</param>
        /// <param name="items">The initial items for this collection.</param>
        public AbstractDataList(string id, List<AbstractUIMetadata> items)
            : base(id)
        {
            Items = items;
        }

        /// <summary>
        /// Fires when <see cref="RequestNewItem"/> is called.
        /// </summary>
        public event EventHandler? AddRequested;

        /// <summary>
        /// Fires when <see cref="RemoveItem"/> is called.
        /// </summary>
        public event CollectionChangedEventHandler<AbstractUIMetadata>? ItemsChanged;

        /// <summary>
        /// Raised when an item is tapped.
        /// </summary>
        public event EventHandler<AbstractUIMetadata>? ItemTapped; 

        /// <summary>
        /// Raised when <see cref="IsUserEditingEnabled"/> changes.
        /// </summary>
        public event EventHandler<bool>? IsUserEditingEnabledChanged;

        /// <summary>
        /// Get an item from this <see cref="AbstractDataList"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIMetadata this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public List<AbstractUIMetadata> Items { get; }

        /// <summary>
        /// If true, the user is able to add or remove items from the list.
        /// </summary>
        [RemoteProperty]
        public bool IsUserEditingEnabled
        {
            get => _isUserEditingEnabled;
            set
            {
                _isUserEditingEnabled = value;
                IsUserEditingEnabledChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        [RemoteProperty]
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode { get; set; }

        /// <summary>
        /// Called when the user wants to add a new item in the list.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        [RemoteMethod]
        public void RequestNewItem()
        {
            AddRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Simulates the tapping of a specific item in <see cref="Items"/>.
        /// </summary>
        /// <param name="item">The item to relay as tapped.</param>
        [RemoteMethod]
        public void TapItem(AbstractUIMetadata item)
        {
            ItemTapped?.Invoke(this, item);
        }

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
        [RemoteMethod]
        public void InsertItem(AbstractUIMetadata item, int index)
        {
            Items.Add(item);

            var addedItems = new List<CollectionChangedItem<AbstractUIMetadata>>
            {
                new CollectionChangedItem<AbstractUIMetadata>(item, index)
            };

            ItemsChanged?.Invoke(this, addedItems, _emptyAbstractUiArray);
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
        [RemoteMethod]
        public void RemoveItemAt(int index)
        {
            var item = Items.ElementAt(index);
            Items.RemoveAt(index);

            var removedItems = new List<CollectionChangedItem<AbstractUIMetadata>>
            {
                new CollectionChangedItem<AbstractUIMetadata>(item, index)
            };

            ItemsChanged?.Invoke(this, _emptyAbstractUiArray, removedItems);
        }
    }
}
