using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;
using OwlCore.Helpers;

namespace LaunchPad.AbstractUI.ViewModels
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

            using (Threading.UIThread)
            {
                RequestNewItemCommand = new RelayCommand<ItemClickEventArgs>(RequestNewItem);

                var itemViewModels = _model.Items.Select(item => new AbstractMutableDataListItemViewModel(item));
                Items = new ObservableCollection<AbstractMutableDataListItemViewModel>(itemViewModels);

                var requestAddMetadataItem = new AbstractUIMetadata("requestAddItem")
                {
                    IconCode = "\uE710",
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
            }
        }

        private void DetachEvents()
        {
            _model.ItemsChanged -= Model_ItemsChanged;

            foreach (var item in Items)
            {
                item.ItemRemoved -= Items_ItemRemoved;
            }
        }

        private void Items_ItemRemoved(object sender, EventArgs e)
        {
            if (!(sender is AbstractMutableDataListItemViewModel viewModel))
                return;

            RemoveItem(viewModel);
        }

        private void Model_ItemsChanged(object sender, System.Collections.Generic.IReadOnlyList<CollectionChangedEventItem<AbstractUIMetadata>> addedItems, System.Collections.Generic.IReadOnlyList<CollectionChangedEventItem<AbstractUIMetadata>> removedItems)
        {
            using (Threading.UIThread)
            {
                foreach (var item in addedItems)
                {
                    var newViewModel = new AbstractMutableDataListItemViewModel(item.Data);
                    newViewModel.ItemRemoved += Items_ItemRemoved;

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
        /// Checks the current display mode by name.
        /// </summary>
        /// <param name="name">The string representation of the <see cref="AbstractDataListPreferredDisplayMode"/> value.</param>
        /// <returns>A boolean indicating if the given string matches the current <see cref="PreferredDisplayMode"/>.</returns>
        public Visibility IsDisplayMode(string name)
        {
            if (!Enum.TryParse<AbstractDataListPreferredDisplayMode>(name, out var result))
                return Visibility.Collapsed;

            return result == PreferredDisplayMode ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Called when the user wants to add a new item in the list.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        public void RequestNewItem() => _model.RequestNewItem();

        private void RequestNewItem(ItemClickEventArgs args)
        {
            if (!(args.ClickedItem is AbstractMutableDataListItemViewModel viewModel))
                return;

            var index = Items.IndexOf(viewModel);

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
        public IRelayCommand<ItemClickEventArgs> RequestNewItemCommand;
    }
}