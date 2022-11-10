using StrixMusic.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls
{
    /// <summary>
    /// A template selector for the navigation view in a <see cref="Settings"/> control.
    /// </summary>
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The data template for shell settings.
        /// </summary>
        public DataTemplate? ShellSettingsTemplate { get; set; }

        /// <summary>
        /// The data template for core settings.
        /// </summary>
        public DataTemplate? CoreSettingsTemplate { get; set; }

        /// <summary>
        /// The data template for core settings.
        /// </summary>
        public DataTemplate? AppSettingsTemplate { get; set; }

        /// <summary>
        /// The data template to use when no other valid template are found.
        /// </summary>
        public DataTemplate? FallbackTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => item switch
        {
            ShellSettings _ => ShellSettingsTemplate ?? base.SelectTemplateCore(item, container),
            _ => FallbackTemplate ?? base.SelectTemplateCore(item, container)
        };
    }
}
