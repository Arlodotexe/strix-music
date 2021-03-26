using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for an <see cref="AbstractMultiChoiceUIElement"/>.
    /// </summary>
    [Bindable(true)]
    public class AbstractMultiChoiceUIElementViewModel : AbstractUIViewModelBase
    {
        private int _selectedIndex;

        /// <inheritdoc />
        public AbstractMultiChoiceUIElementViewModel(AbstractMultiChoiceUIElement model)
            : base(model)
        {
            ItemSelectedCommand = new RelayCommand<AbstractUIMetadataViewModel>(OnItemSelected);
            Items = new ObservableCollection<AbstractMultiChoiceItemViewModel>(model.Items.Select(x => CreateItemViewModel(x, model)));

            SelectedIndex = model.Items.ToOrAsList().IndexOf(model.SelectedItem);
        }

        private AbstractMultiChoiceItemViewModel CreateItemViewModel(AbstractUIMetadata itemModel, AbstractMultiChoiceUIElement model)
        {
            var newItem = new AbstractMultiChoiceItemViewModel(itemModel, Id)
            {
                IsSelected = itemModel == model.SelectedItem
            };

            AttachEvents(model);

            newItem.ItemSelected += ViewModelItem_OnSelected;

            return newItem;
        }

        private void AttachEvents(AbstractMultiChoiceUIElement model)
        {
            model.ItemSelected += Model_ItemSelected;
        }

        private void DetachEvents(AbstractMultiChoiceUIElement model)
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

                ((AbstractMultiChoiceUIElement)Model).SelectItem((AbstractUIMetadata)selectedItem.Model);
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

            Model.Cast<AbstractMultiChoiceUIElement>().SelectItem((AbstractUIMetadata)selectedItem.Model);
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public ObservableCollection<AbstractMultiChoiceItemViewModel> Items { get; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode => ((AbstractMultiChoiceUIElement)Model).PreferredDisplayMode;

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
    }
}
