using System;
using System.Collections.Generic;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for an <see cref="AbstractMultiChoiceUIElement"/>.
    /// </summary>
    public class AbstractMultiChoiceUIElementViewModel : AbstractUIViewModelBase<AbstractMultiChoiceUIElement>
    {

        /// <inheritdoc />
        public AbstractMultiChoiceUIElementViewModel(AbstractMultiChoiceUIElement model)
            : base(model)
        {
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public IEnumerable<AbstractUIMetadata> Items { get; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode { get; }

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        public AbstractUIMetadata SelectedItem => Model.SelectedItem;

        /// <summary>
        /// Fires when the <see cref="SelectedItem"/> is changed.
        /// </summary>
        public event EventHandler<AbstractUIMetadata>? ItemSelected;
    }
}