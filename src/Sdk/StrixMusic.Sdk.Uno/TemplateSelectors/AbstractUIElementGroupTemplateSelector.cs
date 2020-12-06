using OwlCore.AbstractUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Uno.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the elements in a <see cref="AbstractUIElementGroup"/>.
    /// </summary>
    public class AbstractUIElementGroupTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The <see cref="DataTemplate"/> for <see cref="AbstractBooleanUIElement"/>.
        /// </summary>
        public DataTemplate? BooleanUIElementTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for <see cref="AbstractRichTextBlock"/>.
        /// </summary>
        public DataTemplate? RichTextBlockUIElementTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for <see cref="AbstractTextBox"/>.
        /// </summary>
        public DataTemplate? TextBoxUIElementTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for any <see cref="AbstractUIElement"/>.
        /// </summary>
        public DataTemplate? AbstractUIElementTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case AbstractBooleanUIElement _:
                    return BooleanUIElementTemplate!;
                case AbstractRichTextBlock _:
                    return RichTextBlockUIElementTemplate!;
                case AbstractTextBox _:
                    return TextBoxUIElementTemplate!;
                default:
                    return AbstractUIElementTemplate!;
            }
        }
    }
}
