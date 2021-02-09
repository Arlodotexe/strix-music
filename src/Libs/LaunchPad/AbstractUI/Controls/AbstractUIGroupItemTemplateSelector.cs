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
            if (!new Themes.AbstractTextBoxStyle().TryGetValue("DefaultAbstractTextBoxTemplate", out var textBoxTemplate))
            {
                TextBoxTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(textBoxTemplate));
            }

            if (!new Themes.AbstractDataListStyle().TryGetValue("DefaultAbstractDataListTemplate", out var dataListTemplate))
            {
                DataListTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(dataListTemplate));
            }

            if (!new Themes.AbstractMutableDataListStyle().TryGetValue("DefaultAbstractMutableDataListTemplate", out var mutableDataListTemplate))
            {
                MutableDataListTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(mutableDataListTemplate));
            }

            if (!new Themes.AbstractButtonStyle().TryGetValue("DefaultAbstractButtonTemplate", out var buttonTemplate))
            {
                ButtonTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(buttonTemplate));
            }

            if (!new Themes.AbstractMultiChoiceUIElementStyle().TryGetValue("DefaultAbstractMultipleChoiceTemplate", out var multiChoiceTemplate))
            {
                MultiChoiceTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(multiChoiceTemplate));
            }

            if (!new Themes.AbstractBooleanUIElementStyle().TryGetValue("DefaultAbstractBooleanUIElementTemplate", out var booleanTemplate))
            {
                BooleanTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(booleanTemplate));
            }

            if (!new Themes.AbstractProgressUIElementStyle().TryGetValue("DefaultAbstractProgressUIElementTemplate", out var progressTemplate))
            {
                ProgressTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(progressTemplate));
            }
            
            TextBoxTemplate = (DataTemplate)textBoxTemplate;
            DataListTemplate = (DataTemplate)dataListTemplate;
            ButtonTemplate = (DataTemplate)buttonTemplate;
            MutableDataListTemplate = (DataTemplate)mutableDataListTemplate;
            MultiChoiceTemplate = (DataTemplate)multiChoiceTemplate;
            BooleanTemplate = (DataTemplate)booleanTemplate;
            ProgressTemplate = (DataTemplate)progressTemplate;
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
        /// The data template used to display an <see cref="AbstractBooleanUIElement"/>.
        /// </summary>
        public DataTemplate BooleanTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractProgressUIElement"/>.
        /// </summary>
        public DataTemplate ProgressTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractMultiChoiceUIElement"/>.
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
                AbstractBooleanViewModel _ => BooleanTemplate,
                AbstractProgressUIElement _ => ProgressTemplate,
                _ => base.SelectTemplateCore(item, container)
            };
    }
}