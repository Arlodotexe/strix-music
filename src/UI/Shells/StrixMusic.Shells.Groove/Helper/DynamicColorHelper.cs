using ClusterNet.Kernels;
using ClusterNet.Methods;
using OwlCore.Uno.ColorExtractor;
using OwlCore.Uno.ColorExtractor.ColorSpaces;
using OwlCore.Uno.ColorExtractor.Filters;
using OwlCore.Uno.ColorExtractor.Shapes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;
using Color = Windows.UI.Color;
using ISdkImage = StrixMusic.Sdk.Data.IImage;

namespace StrixMusic.Shells.Groove.Helper
{
    /// <summary>
    /// A class containing methods for handling dyanmic coloring.
    /// </summary>
    public static class DynamicColorHelper
    {
        /// <summary>
        /// Gets an accent color from an <see cref="Sdk.Data.IImage"/>.
        /// </summary>
        /// <param name="sdkImage">The image to get a color from.</param>
        /// <returns>The accent color for the image.</returns>
        public static async Task<Color> GetImageAccentColorAsync(ISdkImage sdkImage)
        {
            Image<Argb32>? image = await ImageParser.GetImage(sdkImage.Uri.AbsoluteUri);

            if (image is null)
                return Color.FromArgb(255, 0, 0, 0);

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

            Color finalColor = Color.FromArgb(255,
                (byte)(primary.R * 255),
                (byte)(primary.G * 255),
                (byte)(primary.B * 255)
            );

            return finalColor;
        }
    }
}
