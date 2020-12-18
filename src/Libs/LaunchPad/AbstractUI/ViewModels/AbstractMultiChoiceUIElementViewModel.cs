using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for an <see cref="AbstractMultiChoiceUIElement"/>.
    /// </summary>
    public class AbstractMultiChoiceUIElementViewModel : AbstractUIViewModelBase
    {
        private AbstractUIMetadataViewModel _selectedItem;

        /// <inheritdoc />
        public AbstractMultiChoiceUIElementViewModel(AbstractMultiChoiceUIElement model)
            : base(model)
        {
            ItemSelectedCommand = new RelayCommand<SelectionChangedEventArgs>(OnItemSelected);
            Items = model.Items.Select(x => new AbstractUIMetadataViewModel(x));
            _selectedItem = new AbstractUIMetadataViewModel(model.SelectedItem);
        }

        private void AttachEvents(AbstractMultiChoiceUIElement model)
        {
            model.ItemSelected += Model_ItemSelected;   
        }

        private void DetachEvents(AbstractMultiChoiceUIElement model)
        {
            model.ItemSelected -= Model_ItemSelected;
        }

        private void Model_ItemSelected(object sender, AbstractUIMetadata e)
        {
            SelectedItem = new AbstractUIMetadataViewModel(e);
            ItemSelected?.Invoke(this, SelectedItem);
        }

        private void OnItemSelected(SelectionChangedEventArgs args)
        {
            SelectedItem = (AbstractUIMetadataViewModel)args.AddedItems[0];
            ItemSelected?.Invoke(this, SelectedItem);

            ((AbstractMultiChoiceUIElement) Model).SelectItem((AbstractUIMetadata)SelectedItem.Model);
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public IEnumerable<AbstractUIMetadataViewModel> Items { get; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode => ((AbstractMultiChoiceUIElement)Model).PreferredDisplayMode;

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        public AbstractUIMetadataViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value, nameof(SelectedItem));
        }

        /// <summary>
        /// Fires when the <see cref="SelectedItem"/> is changed.
        /// </summary>
        public event EventHandler<AbstractUIMetadataViewModel>? ItemSelected;

        /// <summary>
        /// RelayCommand that raises when the user chooses an item.
        /// </summary>
        public IRelayCommand<SelectionChangedEventArgs> ItemSelectedCommand;
    }
}
