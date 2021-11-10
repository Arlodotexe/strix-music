using OwlCore.Remoting;
using System;
using System.Collections.Generic;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Presents a list of multiple choices to the user for selection, such as radio buttons or a combobox.
    /// </summary>
    public class AbstractMultiChoice : AbstractUIElement
    {
        private AbstractUIMetadata _selectedItem;

        /// <summary>
        /// Creates a new instance of a <see cref="AbstractMultiChoice"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="defaultSelectedItem"><inheritdoc cref="SelectedItem"/></param>
        /// <param name="items"><inheritdoc cref="Items"/></param>

        public AbstractMultiChoice(string id, AbstractUIMetadata defaultSelectedItem, IEnumerable<AbstractUIMetadata> items)
            : base(id)
        {
            _selectedItem = defaultSelectedItem;
            Items = items;
        }

        /// <summary>
        /// The list of items to be displayed in the UI.
        /// </summary>
        public IEnumerable<AbstractUIMetadata> Items { get; }

        /// <inheritdoc cref="AbstractMultiChoicePreferredDisplayMode"/>
        [RemoteProperty]
        public AbstractMultiChoicePreferredDisplayMode PreferredDisplayMode { get; set; }

        /// <summary>
        /// The current selected item.
        /// </summary>
        /// <remarks>Must be specified on object creation, even if the item is just a prompt to choose something.</remarks>
        [RemoteProperty]
        public AbstractUIMetadata SelectedItem
        {
            get => _selectedItem; 
            set
            {
                _selectedItem = value;
                ItemSelected?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Fires when the <see cref="SelectedItem"/> is changed.
        /// </summary>
        public event EventHandler<AbstractUIMetadata>? ItemSelected;
    }
}
