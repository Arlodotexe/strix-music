using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.TemplateSelectors
{
    public class MainContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AlbumPageTemplate { get; set; }

        public DataTemplate? HomePageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case AlbumViewModel _:
                    Guard.IsNotNull(AlbumPageTemplate, nameof(AlbumPageTemplate));
                    return AlbumPageTemplate;
                case LibraryViewModel _:
                    Guard.IsNotNull(HomePageTemplate, nameof(HomePageTemplate));
                    return HomePageTemplate;
                default:
                    return base.SelectTemplateCore(item);
            }
        }
    }
}
