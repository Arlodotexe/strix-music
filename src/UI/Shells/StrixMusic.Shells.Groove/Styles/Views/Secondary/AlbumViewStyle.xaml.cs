using ClusterNet.Methods;
using ClusterNet.Kernels;
using OwlCore.Uno.ColorExtractor.Filters;
using OwlCore.Uno.ColorExtractor;
using OwlCore.Uno.ColorExtractor.ColorSpaces;
using OwlCore.Uno.ColorExtractor.Shapes;
using Microsoft.Toolkit.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views.Secondary;
using StrixMusic.Sdk.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Color = Windows.UI.Color;

namespace StrixMusic.Shells.Groove.Styles.Views.Secondary
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
                INavigationService<Control> navigationService = Shell.Ioc.GetRequiredService<INavigationService<Control>>();
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
            Image<Argb32>? image = await ImageParser.GetImage(url);

            if (image is null)
                return null;


            FilterCollection filters = new FilterCollection();
            filters.Add(new WhiteFilter(.4f));
            filters.Add(new BlackFilter(.15f));
            filters.Add(new GrayFilter(.3f));

            FilterCollection clamps = new FilterCollection();
            clamps.Add(new SaturationFilter(.6f));

            var colors = ImageParser.GetImageColors(image, 1920);

            colors = filters.Filter(colors, 160);

            //var palette = KMeansMethod.KMeans<RGBColor, RGBShape>(colors, 3);
            GaussianKernel kernel = new GaussianKernel(.15);
            var palette = ClusterAlgorithms.WeightedMeanShift<RGBColor, RGBShape, GaussianKernel>(colors, kernel, Math.Min(colors.Length, 480));
            RGBColor primary = clamps.Clamp(palette[0].Item1);
            Color finalColor = Color.FromArgb(
                255,
                (byte)(primary.R * 255),
                (byte)(primary.G * 255),
                (byte)(primary.B * 255));
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
