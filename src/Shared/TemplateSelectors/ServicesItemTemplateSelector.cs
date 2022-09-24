using CommunityToolkit.Diagnostics;
using StrixMusic.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.TemplateSelectors
{
    /// <summary>
    /// Selects the template for a service item.
    /// </summary>
    public class ServicesItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The template to display for an item that triggers creating a new core.
        /// </summary>
        public DataTemplate? NewItemTemplate { get; set; }

        /// <summary>
        /// The template to display for a loaded core.
        /// </summary>
        public DataTemplate? CoreItemTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!(item is LoadedServicesItemViewModel settingsVm))
                return base.SelectTemplateCore(item, container);
            
            Guard.IsNotNull(NewItemTemplate, nameof(NewItemTemplate));
            Guard.IsNotNull(CoreItemTemplate, nameof(CoreItemTemplate));

            return settingsVm.IsAddItem ? NewItemTemplate : CoreItemTemplate;
        }
    }
}
