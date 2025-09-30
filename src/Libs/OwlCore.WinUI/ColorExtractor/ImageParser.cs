using System.IO;
using System.Threading.Tasks;
using OwlCore.WinUI.ColorExtractor.ColorSpaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace OwlCore.WinUI.ColorExtractor
{
    /// <summary>
    /// A class of methods for working with images.
    /// </summary>
    public static class ImageParser
    {
        /// <summary>
        /// Gets an <see cref="Image{TPixel}"/> with <see cref="Argb32"/> format from a url.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>An Argb32 image.</returns>
        public static async Task<Image<Argb32>?> GetImage(Stream stream)
        {
            var image = await Image.LoadAsync(stream);
            var clonedImage = image.CloneAs<Argb32>();

            return clonedImage;
        }

        /// <summary>
        /// Gets colors out of an image.
        /// </summary>
        /// <param name="image">The image to get colors from.</param>
        /// <param name="quality">The approximate amount of pixels to get/</param>
        /// <returns>An array of colors that appeared as pixels in the image.</returns>
        public static RGBColor[] GetImageColors(Image<Argb32> image, int quality = 1920)
        {
            var nth = image.Width * image.Height / quality;
            var pixelsPerRow = image.Width / nth;

            var colors = new RGBColor[image.Height * pixelsPerRow];

            var pos = 0;
            for (var row = 0; row < image.Height; row++)
            {
                var rowPixels = image.DangerousGetPixelRowMemory(row).Span;
                for (var i = 0; i < pixelsPerRow; i++)
                {
                    var b = rowPixels[i * nth].B / 255f;
                    var g = rowPixels[i * nth].G / 255f;
                    var r = rowPixels[i * nth].R / 255f;
                    //float a = rowPixels[i].A / 255;

                    var color = new RGBColor(r, g, b);

                    colors[pos] = color;
                    pos++;
                }
            }

            return colors;
        }
    }
}
