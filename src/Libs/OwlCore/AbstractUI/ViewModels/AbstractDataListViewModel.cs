using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel wrapper for an <see cref="AbstractDataList"/>.
    /// </summary>
    public class AbstractDataListViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractDataList _model;
        private readonly AbstractDataListItemViewModel _requestAddMetadataItemVm;
        private bool _isUserEditingEnabled;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractDataListViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractDataListViewModel(AbstractDataList model)
            : base(model)
        {
            _model = model;

            using (Threading.PrimaryContext)
            {
                var requestAddMetadataItem = new AbstractUIMetadata("requestAddItem")
                {
                    Title = "Add new",
                };

                _requestAddMetadataItemVm = new AbstractDataListItemViewModel(requestAddMetadataItem, this);

                var itemViewModels = _model.Items.Select(item => new AbstractDataListItemViewModel(item, this));
                Items = new ObservableCollection<AbstractDataListItemViewModel>(itemViewModels);

                if (model.IsUserEditingEnabled)
                    Items.Add(_requestAddMetadataItemVm);
            }

            ItemTappedCommand = new RelayCommand<AbstractDataListItemViewModel>(TapItem);

            AttachEvents();
        }

        /// <summary>
        /// Fires when <see cref="RequestNewItem()"/> is called.
        /// </summary>
        public event EventHandler AddRequested
        {
            add => _model.AddRequested += value;
            remove => _model.AddRequested -= value;
        }


        /// <summary>
        /// Raised when an item is tapped.
        /// </summary>
        public event EventHandler<AbstractUIMetadata> ItemTapped
        {
            add => _model.ItemTapped += value;
            remove => _model.ItemTapped -= value;
        }

        private void AttachEvents()
        {
            _model.ItemsChanged += Model_ItemsChanged;
            _model.IsUserEditingEnabledChanged += ModelOnIsUserEditingEnabledChanged;

            foreach (var item in Items)
            {
                item.ItemRemoved += Items_ItemRemoved;
                item.ItemAddRequested += Items_ItemAddRequested;

            }
        }

        private void ModelOnIsUserEditingEnabledChanged(object sender, bool e)
        {
            if (!e)
            {
                if (Items.Contains(_requestAddMetadataItemVm))
                    Items.Remove(_requestAddMetadataItemVm);
            }
            else
            {
                if (!Items.Contains(_requestAddMetadataItemVm))
                    Items.Add(_requestAddMetadataItemVm);
            }

            IsUserEditingEnabled = e;
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
            if (!(sender is AbstractDataListItemViewModel viewModel))
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
                    var newViewModel = new AbstractDataListItemViewModel(item.Data, this);
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
        public AbstractDataListItemViewModel this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this collection.
        /// </summary>
        public ObservableCollection<AbstractDataListItemViewModel> Items { get; }

        /// <summary>
        /// If true, the user is able to add or remove items from the list.
        /// </summary>
        public bool IsUserEditingEnabled
        {
            get => _isUserEditingEnabled;
            set { 
                _isUserEditingEnabled = value;
                OnPropertyChanged(nameof(IsUserEditingEnabled));
            }
        }

        /// <inheritdoc cref="AbstractDataListPreferredDisplayMode"/>
        public AbstractDataListPreferredDisplayMode PreferredDisplayMode => _model.PreferredDisplayMode;

        /// <summary>
        /// Called when the user wants to add a new item in the list.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        public void RequestNewItem() => _model.RequestNewItem();

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
        public void RemoveItem(AbstractDataListItemViewModel item)
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

        private void TapItem(AbstractDataListItemViewModel? viewModel)
        {
            Guard.IsNotNull(viewModel, nameof(viewModel));

            if (viewModel.Id == "requestAddItem")
                RequestNewItem();

            _model.TapItem((AbstractUIMetadata)viewModel.Model);
        }

        /// <summary>
        /// Fires when a new item has been requested.
        /// </summary>
        public IRelayCommand<AbstractDataListItemViewModel> ItemTappedCommand { get; set; }

        /// <inheritdoc />
        public override void Dispose()
        {
            DetachEvents();
            base.Dispose();
        }
    }
}