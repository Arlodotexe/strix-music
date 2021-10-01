using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel wrapper for an <see cref="AbstractUICollection"/>.
    /// </summary>
    public class AbstractUICollectionViewModel : AbstractUIViewModelBase, IEnumerable<AbstractUIViewModelBase>
    {
        private readonly AbstractUICollection _model;

        /// <inheritdoc />
        public AbstractUICollectionViewModel(AbstractUICollection model) : base(model)
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
                    AbstractDataList dataList => new AbstractDataListViewModel(dataList),
                    AbstractButton button => new AbstractButtonViewModel(button),
                    AbstractBoolean boolean => new AbstractBooleanViewModel(boolean),
                    AbstractRichTextBlock richText => new AbstractRichTextBlockViewModel(richText),
                    AbstractMultiChoice multiChoiceUIElement => new AbstractMultiChoiceViewModel(multiChoiceUIElement),
                    AbstractUICollection elementGroup => new AbstractUICollectionViewModel(elementGroup),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractUICollection"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIViewModelBase this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this group.
        /// </summary>
        public IEnumerable<AbstractUIViewModelBase> Items { get; }

        /// <inheritdoc cref="Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation => _model.PreferredOrientation;

        /// <inheritdoc />
        public IEnumerator<AbstractUIViewModelBase> GetEnumerator() => Items.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}