using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for <see cref="AbstractUIElementGroup"/>.
    /// </summary>
    public class AbstractUIElementGroupViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractUIElementGroup _model;

        /// <inheritdoc />
        public AbstractUIElementGroupViewModel(AbstractUIElementGroup model, DataTemplateSelector templateSelector) : base(model)
        {
            TemplateSelector = templateSelector;
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

        /// <inheritdoc cref="OwlCore.AbstractUI.Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation => _model.PreferredOrientation;

        /// <summary>
        /// The template selector used to decide the template for each item in <see cref="Items"/>.
        /// </summary>
        public DataTemplateSelector TemplateSelector { get; }
    }
}