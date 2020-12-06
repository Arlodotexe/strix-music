using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Presents a list of multiple choices to the user for selection.
    /// </summary>
    public class AbstractMultiChoiceUIElement : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of a <see cref="AbstractMultiChoiceUIElement"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="defaultSelectedItem"><inheritdoc cref="SelectedItem"/></param>
        /// <param name="preferredDisplayMode"><inheritdoc cref="PreferredDisplayMode"/></param>
        /// <param name="items"><inheritdoc cref="Items"/></param>
        public AbstractMultiChoiceUIElement(string id, AbstractUIMetadata defaultSelectedItem, AbstractMultiChoicePreferredDisplayMode preferredDisplayMode, IEnumerable<AbstractUIMetadata> items)
            : base(id)
        {
            Items = items;
            PreferredDisplayMode = preferredDisplayMode;
            SelectedItem = defaultSelectedItem;
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
        public AbstractUIMetadata SelectedItem { get; private set; }

        /// <summary>
        /// Called to change the <see cref="SelectedItem"/>.
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public void SelectItem(AbstractUIMetadata newValue)
        {
            SelectedItem = newValue;
            ItemSelected?.Invoke(this, newValue);
        }

        /// <summary>
        /// Fires when the <see cref="SelectedItem"/> is changed.
        /// </summary>
        public event EventHandler<AbstractUIMetadata>? ItemSelected;
    }
}
