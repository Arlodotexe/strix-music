using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a list of multiple choices to the user for selection.
    /// </summary>
    public abstract class AbstractMultiChoiceUIElement : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of a <see cref="AbstractMultiChoiceUIElement"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="defaultSelectedItem"><inheritdoc cref="SelectedItem"/></param>
        /// <param name="preferredDisplayMode"><inheritdoc cref="PreferredDisplayMode"/></param>
        protected AbstractMultiChoiceUIElement(string id, AbstractUIMetadata defaultSelectedItem, AbstractMultiChoicePreferredDisplayMode preferredDisplayMode)
            : base(id)
        {
            Items = Array.Empty<AbstractUIMetadata>();
            PreferredDisplayMode = preferredDisplayMode;
            SelectedItem = defaultSelectedItem;
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public IEnumerable<AbstractUIMetadata> Items { get; protected set; }

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        public AbstractUIMetadata SelectedItem { get; protected set; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode { get; }

        /// <summary>
        /// Called to tell the core that the selected item has been changed.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task ItemSelected(AbstractUIMetadata selectedItem);
    }
}
