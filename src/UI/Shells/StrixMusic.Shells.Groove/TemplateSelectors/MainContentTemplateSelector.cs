using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.TemplateSelectors
{
    public class MainContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AlbumPageTemplate { get; set; }

        public DataTemplate? ArtistPageTemplate { get; set; }

        public DataTemplate? HomePageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case GrooveAlbumPageViewModel _:
                    Guard.IsNotNull(AlbumPageTemplate, nameof(AlbumPageTemplate));
                    return AlbumPageTemplate;
                case GrooveArtistPageViewModel _:
                    Guard.IsNotNull(ArtistPageTemplate, nameof(ArtistPageTemplate));
                    return ArtistPageTemplate;
                case GrooveHomePageViewModel _:
                    Guard.IsNotNull(HomePageTemplate, nameof(HomePageTemplate));
                    return HomePageTemplate;
                default:
                    return base.SelectTemplateCore(item);
            }
        }
    }
}
