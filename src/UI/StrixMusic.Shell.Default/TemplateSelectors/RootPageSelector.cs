using StrixMusic.Shell.Default.Controls;
using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the root page of the shell.
    /// </summary>
    public class RootPageSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="HomeControl"/> template.
        /// </summary>
        public DataTemplate? HomeControlTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case ObservableCollectionGroup _:
                    return HomeControlTemplate!;
                default:
                    return HomeControlTemplate!;
            }
        }
    }
}
