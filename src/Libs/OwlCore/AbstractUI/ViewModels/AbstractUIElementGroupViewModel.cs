using System.Collections.Generic;
using System.Linq;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for <see cref="AbstractUIElementGroup"/>.
    /// </summary>
    public class AbstractUIElementGroupViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractUIElementGroup _model;

        /// <inheritdoc />
        public AbstractUIElementGroupViewModel(AbstractUIElementGroup model) : base(model)
        {
            _model = model;
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractUIElementGroup"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIElement this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this group.
        /// </summary>
        public IEnumerable<AbstractUIElement> Items
        {
            get => _model.Items;
            set => SetProperty(_model.Items, value, _model, (u, n) => _model.Items = n);
        }

        /// <inheritdoc cref="Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation => _model.PreferredOrientation;

        //public DataTemplateSelector 
    }
}