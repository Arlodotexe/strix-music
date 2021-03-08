using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel for the <see cref="AbstractDataList"/>.
    /// </summary>
    public class AbstractMutableDataListViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractMutableDataList _model;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractDataListViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractMutableDataListViewModel(AbstractMutableDataList model)
            : base(model)
        {
            _model = model;

            using (Threading.PrimaryContext)
            {
                RequestNewItemCommand = new RelayCommand<AbstractMutableDataListItemViewModel>(RequestNewItem);

                var itemViewModels = _model.Items.Select(item => new AbstractMutableDataListItemViewModel(item));
                Items = new ObservableCollection<AbstractMutableDataListItemViewModel>(itemViewModels);

                var requestAddMetadataItem = new AbstractUIMetadata("requestAddItem")
                {
                    Title = "Add new",
                };

                Items.Add(new AbstractMutableDataListItemViewModel(requestAddMetadataItem));
            }

            AttachEvents();
        }

        private void AttachEvents()
        {
            _model.ItemsChanged += Model_ItemsChanged;

            foreach (var item in Items)
            {
                item.ItemRemoved += Items_ItemRemoved;
                item.ItemAddRequested += Items_ItemAddRequested;

            }
        }

        private void DetachEvents()
        {
            _model.ItemsChanged -= Model_ItemsChanged;

            foreach (var item in Items)
            {
                item.ItemRemoved -= Items_ItemRemoved;
                item.ItemAddRequested -= Items_ItemAddRequested;
            }
        }

        private void Items_ItemRemoved(object sender, EventArgs e)
        {
            if (!(sender is AbstractMutableDataListItemViewModel viewModel))
                return;

            RemoveItem(viewModel);
        }

        private void Items_ItemAddRequested(object sender, EventArgs e) => RequestNewItem();

        private void Model_ItemsChanged(object sender, System.Collections.Generic.IReadOnlyList<CollectionChangedItem<AbstractUIMetadata>> addedItems, System.Collections.Generic.IReadOnlyList<CollectionChangedItem<AbstractUIMetadata>> removedItems)
        {
            using (Threading.PrimaryContext)
            {
                foreach (var item in addedItems)
                {
                    var newViewModel = new AbstractMutableDataListItemViewModel(item.Data);
                    newViewModel.ItemRemoved += Items_ItemRemoved;
                    newViewModel.ItemAddRequested += Items_ItemAddRequested;

                    if (item.Index == Items.Count)
                        Items.Add(newViewModel);
                    else
                        Items.Insert(item.Index, newViewModel);
                }

                foreach (var item in removedItems)
                {
                    // Last item is the "add" button, not allowed to remove.
                    if (item.Index == Items.Count - 1)
                        return;

                    Items.RemoveAt(item.Index);
                }
            }
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractDataListViewModel"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractMutableDataListItemViewModel this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public ObservableCollection<AbstractMutableDataListItemViewModel> Items { get; }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode => _model.PreferredDisplayMode;

        /// <summary>
        /// Called when the user wants to add a new item in the list.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        public void RequestNewItem() => _model.RequestNewItem();

        private void RequestNewItem(AbstractMutableDataListItemViewModel selectedItem)
        {
            var index = Items.IndexOf(selectedItem);

            if (index == Items.Count - 1)
                RequestNewItem();
        }

        /// <summary>
        /// Adds an item from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(AbstractUIMetadata item)
        {
            _model.AddItem(item);
        }

        /// <summary>
        /// Inserts an item into <see cref="AbstractDataList.Items"/> at a specific index.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to add.</param>
        public void InsertItem(AbstractUIMetadata item, int index)
        {
            _model.InsertItem(item, index);
        }

        /// <summary>
        /// Removes an item from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="item">The item being removed</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public void RemoveItem(AbstractMutableDataListItemViewModel item)
        {
            var index = _model.Items.IndexOf((AbstractUIMetadata)item.Model);

            if (index == -1)
                return;

            RemoveItemAt(index);
        }

        /// <summary>
        /// Removes an item at a specific index from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public void RemoveItemAt(int index) => _model.RemoveItemAt(index);

        /// <summary>
        /// Fires when <see cref="RequestNewItem"/> is called.
        /// </summary>
        public event EventHandler AddRequested
        {
            add => _model.AddRequested += value;
            remove => _model.AddRequested -= value;
        }

        /// <inheritdoc cref="RequestNewItem" />
        public IRelayCommand<AbstractMutableDataListItemViewModel> RequestNewItemCommand;
    }
}