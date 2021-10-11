using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the <see cref="GrooveShell"/> based on the ViewModel.
    /// </summary>
    public class MainContentTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="GrooveAlbumPageViewModel"/>.
        /// </summary>
        public DataTemplate? AlbumPageTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="GrooveArtistPageViewModel"/>.
        /// </summary>
        public DataTemplate? ArtistPageTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="GrooveHomePageViewModel"/>.
        /// </summary>
        public DataTemplate? HomePageTemplate { get; set; }

        /// <inheritdoc/>
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
