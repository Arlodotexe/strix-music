using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for <see cref="AbstractUIElementGroup"/>.
    /// </summary>
    [Bindable(true)]
    public class AbstractUIElementGroupViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractUIElementGroup _model;

        /// <inheritdoc />
        public AbstractUIElementGroupViewModel(AbstractUIElementGroup model) : base(model)
        {
            _model = model;
            Items = SetupItemViewModels();
        }

        private IEnumerable<AbstractUIViewModelBase> SetupItemViewModels()
        {
            foreach (var item in _model.Items)
            {
                yield return item switch
                {
                    AbstractTextBox textBox => new AbstractTextBoxViewModel(textBox),
                    AbstractMutableDataList mutableDataList => new AbstractMutableDataListViewModel(mutableDataList),
                    AbstractDataList dataList => new AbstractDataListViewModel(dataList),
                    AbstractButton button => new AbstractButtonViewModel(button),
                    AbstractBooleanUIElement boolean => new AbstractBooleanViewModel(boolean),
                    AbstractMultiChoiceUIElement multiChoiceUIElement => new AbstractMultiChoiceUIElementViewModel(multiChoiceUIElement),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractUIElementGroup"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIViewModelBase this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this group.
        /// </summary>
        public IEnumerable<AbstractUIViewModelBase> Items { get; }

        /// <inheritdoc cref="Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation => _model.PreferredOrientation;
    }
}