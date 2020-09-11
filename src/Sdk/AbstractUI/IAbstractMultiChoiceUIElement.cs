using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a list of multiple choices to the user for selection.
    /// </summary>
    public interface IAbstractMultiChoiceUIElement : IAbstractUIElement
    {
        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public IEnumerable<IAbstractUIMetadata> Items { get; }

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        public IAbstractUIMetadata SelectedItem { get; }

        /// <inheritdoc cref="AbstractMultiChoicePrefferedDisplayMode"/>
        public AbstractMultiChoicePrefferedDisplayMode PrefferedDisplayMode { get; }

        /// <summary>
        /// Called to tell the core that the selected item has been changed.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ItemSelected(IAbstractUIMetadata selectedItem);
    }
}
