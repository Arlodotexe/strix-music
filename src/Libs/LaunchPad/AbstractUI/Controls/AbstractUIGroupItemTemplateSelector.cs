using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LaunchPad.AbstractUI.ViewModels;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.Controls
{
    /// <summary>
    /// The template selector used to display Abstract UI elements. Use this to define your own custom styles for each control. You may specify the existing, default styles for those you don't want to override.
    /// </summary>
    public class AbstractUIGroupItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIGroupItemTemplateSelector"/>.
        /// </summary>
        public AbstractUIGroupItemTemplateSelector()
        {
            // ReSharper disable CollectionNeverUpdated.Local
            if (!new Themes.AbstractTextBoxTemplate().TryGetValue("DefaultAbstractTextBoxTemplate", out var textBoxTemplate))
            {
                TextBoxTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(textBoxTemplate));
            }

            if (!new Themes.AbstractDataListTemplate().TryGetValue("DefaultAbstractDataListTemplate", out var dataListTemplate))
            {
                DataListTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(dataListTemplate));
            }

            if (!new Themes.AbstractMutableDataListTemplate().TryGetValue("DefaultAbstractMutableDataListTemplate", out var mutableDataListTemplate))
            {
                MutableDataListTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(mutableDataListTemplate));
            }

            if (!new Themes.AbstractButtonTemplate().TryGetValue("DefaultAbstractButtonTemplate", out var buttonTemplate))
            {
                ButtonTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(buttonTemplate));
            }

            if (!new Themes.AbstractMultiChoiceUIElementTemplate().TryGetValue("DefaultAbstractMultipleChoiceTemplate", out var multiChoiceTemplate))
            {
                MultiChoiceTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(multiChoiceTemplate));
            }

            TextBoxTemplate = (DataTemplate)textBoxTemplate;
            DataListTemplate = (DataTemplate)dataListTemplate;
            ButtonTemplate = (DataTemplate)buttonTemplate;
            MutableDataListTemplate = (DataTemplate)mutableDataListTemplate;
            MultiChoiceTemplate = (DataTemplate)multiChoiceTemplate;
        }

        /// <summary>
        /// The data template used to display an <see cref="AbstractTextBox"/>.
        /// </summary>
        public DataTemplate TextBoxTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractDataList"/>.
        /// </summary>
        public DataTemplate DataListTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractMutableDataList"/>.
        /// </summary>
        public DataTemplate MutableDataListTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractButton"/>.
        /// </summary>
        public DataTemplate ButtonTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractButton"/>.
        /// </summary>
        public DataTemplate MultiChoiceTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => item switch
            {
                AbstractTextBoxViewModel _ => TextBoxTemplate,
                AbstractDataListViewModel _ => DataListTemplate,
                AbstractButtonViewModel _ => ButtonTemplate,
                AbstractMutableDataListViewModel _ => MutableDataListTemplate,
                AbstractMultiChoiceUIElementViewModel _ => MultiChoiceTemplate,
                _ => base.SelectTemplateCore(item, container)
            };
    }
}