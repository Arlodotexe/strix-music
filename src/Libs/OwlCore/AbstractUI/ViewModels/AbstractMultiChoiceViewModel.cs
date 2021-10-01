using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel wrapper for an <see cref="AbstractMultiChoice"/>.
    /// </summary>
    public class AbstractMultiChoiceViewModel : AbstractUIViewModelBase
    {
        private int _selectedIndex;

        /// <inheritdoc />
        public AbstractMultiChoiceViewModel(AbstractMultiChoice model)
            : base(model)
        {
            ItemSelectedCommand = new RelayCommand<AbstractUIMetadataViewModel>(OnItemSelected);
            Items = new ObservableCollection<AbstractMultiChoiceItemViewModel>(model.Items.Select(x => CreateItemViewModel(x, model)));

            SelectedIndex = model.Items.ToOrAsList().IndexOf(model.SelectedItem);

            AttachEvents(model);
        }

        private AbstractMultiChoiceItemViewModel CreateItemViewModel(AbstractUIMetadata itemModel, AbstractMultiChoice model)
        {
            var newItem = new AbstractMultiChoiceItemViewModel(itemModel, Id)
            {
                IsSelected = itemModel == model.SelectedItem
            };

            newItem.ItemSelected += ViewModelItem_OnSelected;

            return newItem;
        }

        private void AttachEvents(AbstractMultiChoice model)
        {
            model.ItemSelected += Model_ItemSelected;
        }

        private void DetachEvents(AbstractMultiChoice model)
        {
            model.ItemSelected -= Model_ItemSelected;
        }

        private void ViewModelItem_OnSelected(object sender, EventArgs e)
        {
            if (sender is AbstractMultiChoiceItemViewModel selectedItem)
            {
                foreach (var item in Items)
                {
                    item.IsSelected = item == selectedItem;
                }

                var index = Items.IndexOf(selectedItem);

                SelectedIndex = index;

                ItemSelected?.Invoke(this, selectedItem);

                ((AbstractMultiChoice)Model).SelectedItem = (AbstractUIMetadata)selectedItem.Model;
            }
        }

        private void Model_ItemSelected(object sender, AbstractUIMetadata e)
        {
            var selectedItem = new AbstractMultiChoiceItemViewModel(e, Id);
            var index = Items.IndexOf(selectedItem);

            SelectedIndex = index;

            ItemSelected?.Invoke(this, selectedItem);
        }

        private void OnItemSelected(AbstractUIMetadataViewModel? selectedItem)
        {
            Guard.IsNotNull(selectedItem, nameof(selectedItem));

            ItemSelected?.Invoke(this, selectedItem);

            Model.Cast<AbstractMultiChoice>().SelectedItem = (AbstractUIMetadata)selectedItem.Model;
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public ObservableCollection<AbstractMultiChoiceItemViewModel> Items { get; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode => ((AbstractMultiChoice)Model).PreferredDisplayMode;

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        /// <summary>
        /// Fires when the <see cref="SelectedIndex"/> is changed.
        /// </summary>
        public event EventHandler<AbstractUIMetadataViewModel>? ItemSelected;

        /// <summary>
        /// RelayCommand that raises when the user chooses an item.
        /// </summary>
        public IRelayCommand<AbstractUIMetadataViewModel> ItemSelectedCommand;

        /// <inheritdoc/>
        public override void Dispose()
        {
            DetachEvents((AbstractMultiChoice)Model);

            base.Dispose();
        }
    }
}
