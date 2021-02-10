using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace OwlCore.Uno.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for an <see cref="AbstractMultiChoiceUIElement"/>.
    /// </summary>
    public class AbstractMultiChoiceUIElementViewModel : AbstractUIViewModelBase
    {
        private int _selectedIndex;

        /// <inheritdoc />
        public AbstractMultiChoiceUIElementViewModel(AbstractMultiChoiceUIElement model)
            : base(model)
        {
            ItemSelectedCommand = new RelayCommand<SelectionChangedEventArgs>(OnItemSelected);
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

        private void OnItemSelected(SelectionChangedEventArgs args)
        {
            var selectedItem = (AbstractUIMetadataViewModel)args.AddedItems[0];

            ItemSelected?.Invoke(this, selectedItem);

            ((AbstractMultiChoiceUIElement)Model).SelectItem((AbstractUIMetadata)selectedItem.Model);
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
        public IRelayCommand<SelectionChangedEventArgs> ItemSelectedCommand;
    }
}
