using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LaunchPad.AbstractUI.ViewModels;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.Controls
{
    /// <summary>
    /// The template selector used to display Abstract UI elements. Use this to define your own custom styles for each control. You may specify the existing, default styles for those you don't want to override.
    /// </summary>
    public class AbstractUIGroupPresentationTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIGroupPresentationTemplateSelector"/>.
        /// </summary>
        public AbstractUIGroupPresentationTemplateSelector()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var textBoxResource = new Themes.AbstractTextBoxTemplate();
            TextBoxTemplate = (DataTemplate)textBoxResource["DefaultAbstractTextBoxTemplate"];
        }

        /// <summary>
        /// The data template used to display a <see cref="AbstractTextBox"/>.
        /// </summary>
        public DataTemplate TextBoxTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => item switch
            {
                AbstractTextBoxViewModel _ => TextBoxTemplate,
                _ => base.SelectTemplateCore(item, container)
            };
    }
}