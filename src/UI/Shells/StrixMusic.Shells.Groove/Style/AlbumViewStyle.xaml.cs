using LaunchPad.ColorExtraction;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Sdk.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
            if ((sender as Control)?.DataContext is ArtistViewModel viewModel)
            {
                INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();
                navigationService.NavigateTo(typeof(ArtistView), false, viewModel);
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

            Color? color = await Task.Run(async () => await UpdateBackgroundColor(albumViewModel.Images[0].Uri.AbsoluteUri));

            if (color.HasValue)
                AnimateBackgroundChange(color.Value, grid);
            else
                AnimateBackgroundChange(Color.FromArgb(255, 51, 51, 51), grid);
                //grid.Background = new SolidColorBrush(color.Value);
        }

        private async Task<Color?> UpdateBackgroundColor(string url)
        {
            Image<Argb32>? image = await ImageParser.GetImage(url, 0, 0);

            if (image is null)
                return null;

            var rgbColors = ImageParser.GetImageColors(image, 128);
            var hsvColors = rgbColors.Select(x => HSVColor.FromColor(x));
            var palette = ColorExtracter.Palettize(hsvColors.ToList(), 3);
            Color finalColor = palette[0].AsArgb();
            return finalColor;
        }

        private void AnimateBackgroundChange(Color color, Grid background)
        {
            ColorAnimation colorAnimation = new ColorAnimation();
            colorAnimation.EasingFunction = new QuadraticEase();
            colorAnimation.EasingFunction.EasingMode = EasingMode.EaseInOut;
            colorAnimation.To = color;
            Storyboard.SetTarget(colorAnimation, background.Background);
            Storyboard.SetTargetProperty(colorAnimation, "(SolidColorBrush.Color)");
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(.666);
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();
        }
    }
}
