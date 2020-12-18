using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for an <see cref="AbstractMultiChoiceUIElement"/>.
    /// </summary>
    public class AbstractMultiChoiceUIElementViewModel : AbstractUIViewModelBase
    {
        /// <inheritdoc />
        public AbstractMultiChoiceUIElementViewModel(AbstractMultiChoiceUIElement model)
            : base(model)
        {
            ItemSelectedCommand = new RelayCommand<AbstractUIMetadata>(OnItemSelected);
            Items = model.Items.Select(x => x.Title).PruneNull();
        }

        private void OnItemSelected(AbstractUIMetadata selectedItem)
        {
            ItemSelected?.Invoke(this, selectedItem);
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public IEnumerable<string> Items { get; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode => ((AbstractMultiChoiceUIElement)Model).PreferredDisplayMode;

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        public AbstractUIMetadata SelectedItem => ((AbstractMultiChoiceUIElement)Model).SelectedItem;

        /// <summary>
        /// Fires when the <see cref="SelectedItem"/> is changed.
        /// </summary>
        public event EventHandler<AbstractUIMetadata>? ItemSelected;

        /// <summary>
        /// RelayCommand that raises when the user chooses an item.
        /// </summary>
        public IRelayCommand<AbstractUIMetadata> ItemSelectedCommand;
    }
}