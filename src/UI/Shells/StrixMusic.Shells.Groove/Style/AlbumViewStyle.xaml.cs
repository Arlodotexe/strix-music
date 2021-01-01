using LaunchPad.ColorExtraction;
using Microsoft.Toolkit.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Sdk.ViewModels;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Color = Windows.UI.Color;

namespace StrixMusic.Shells.Groove.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="AlbumView"/> in the Strix Shell.
    /// </summary>
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
            // TODO: Navigate to ArtistView
            if ((sender as Control)?.DataContext is AlbumViewModel viewModel)
            {
                INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();
                navigationService.NavigateTo(typeof(ArtistView), false, viewModel.Artist);
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            /// Ensure sender is a Grid
            if (!(sender is Grid grid))
                return;

            /// Ensure DataContext is an AlbumViewModel
            if (!(grid.DataContext is AlbumViewModel albumViewModel))
                return;


            Image<Argb32>? image = await ImageParser.GetImage(albumViewModel.Images[0].Uri.AbsoluteUri, 0, 0);

            if (image is null)
                return;

            var rgbColors = ImageParser.GetImageColors(image, 128);
            var hsvColors = rgbColors.Select(x => HSVColor.FromColor(x));
            var palette = ColorExtracter.Palettize(hsvColors.ToList(), 3);
            Color finalColor = palette[0].AsArgb();

            grid.Background = new SolidColorBrush(finalColor);
        }
    }
}
