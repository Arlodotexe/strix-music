using ClusterNet.Kernels;
using ClusterNet.Methods;
using OwlCore;
using OwlCore.WinUI.ColorExtractor;
using OwlCore.WinUI.ColorExtractor.ColorSpaces;
using OwlCore.WinUI.ColorExtractor.Filters;
using OwlCore.WinUI.ColorExtractor.Shapes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using Color = Windows.UI.Color;
using ISdkImage = StrixMusic.Sdk.AppModels.IImage;

namespace StrixMusic.Shells.Groove.Helper
{
    /// <summary>
    /// A class containing methods for handling dyanmic coloring.
    /// </summary>
    public static class DynamicColorHelper
    {
        private static SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Gets an accent color from an <see cref="Sdk.AppModels.IImage"/>.
        /// </summary>
        /// <param name="sdkImage">The image to get a color from.</param>
        /// <returns>The accent color for the image.</returns>
        public static async Task<Color> GetImageAccentColorAsync(Uri imageUri)
        {
            using (await Flow.EasySemaphore(_mutex))
            {
                var image = await ImageParser.GetImage(imageUri.OriginalString);

                if (image is null)
                    return Color.FromArgb(255, 0, 0, 0);

                var filters = new FilterCollection();
                filters.Add(new WhiteFilter(.4f));
                filters.Add(new BlackFilter(.15f));
                filters.Add(new GrayFilter(.3f));

                var clamps = new FilterCollection();
                clamps.Add(new SaturationFilter(.6f));

                var colors = ImageParser.GetImageColors(image, 1920);

                colors = filters.Filter(colors, 160);

                //var palette = KMeansMethod.KMeans<RGBColor, RGBShape>(colors, 3);
                var kernel = new GaussianKernel(.15);
                var palette = ClusterAlgorithms.WeightedMeanShift<RGBColor, RGBShape, GaussianKernel>(colors, kernel, Math.Min(colors.Length, 480));
                var primary = clamps.Clamp(palette[0].Item1);

                var finalColor = Color.FromArgb(255,
                    (byte)(primary.R * 255),
                    (byte)(primary.G * 255),
                    (byte)(primary.B * 255)
                );

                return finalColor;
            }
        }
    }
}
