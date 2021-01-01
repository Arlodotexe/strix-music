using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Color = Windows.UI.Color;

namespace LaunchPad.ColorExtraction
{
    /// <summary>
    /// A <see langword="static"/> class containing methods to help get 
    /// </summary>
    public static class ImageParser
    {
        /// <summary>
        /// Gets a pixel array from a <see cref="BitmapImage"/>
        /// </summary>
        /// <param name="image">The <see cref="BitmapImage"/>.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>A pixel array.</returns>
        public static async Task<Image<Argb32>?> GetImage(BitmapImage image, uint width, uint height)
        {
            return await GetImage(image.UriSource.AbsoluteUri, width, height);
        }

        /// <summary>
        /// Gets a pixel array from a <see cref="Uri"/> string.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> string..</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <remarks>
        /// https://social.msdn.microsoft.com/Forums/windowsapps/en-US/02927d7a-077f-4263-8b60-b5567baed94b/uwp-convert-the-bitmapimage-into-writeablebitmap
        /// </remarks>
        /// <returns>A pixel array.</returns>
        public static async Task<Image<Argb32>?> GetImage(string uri, uint width, uint height)
        {
            return (await Image.LoadAsync(await GetImageStreamAsync(uri))).CloneAs<Argb32>();
        }

        /// <summary>
        /// Gets a list of colors in an image.
        /// </summary>
        /// <param name="image">The image to read from.</param>
        /// <param name="quality">The amount of pixels to skip (lower is more accurate).</param>
        /// <param name="config">A filter of what colors to ignore.</param>
        /// <returns>A list of colors in the image.</returns>
        public static List<Color> GetImageColors(
            Image<Argb32> image,
            int quality = 4,
            ColorFilterConfig? config = null)
        {
            ColorFilterConfig filter;
            if (config == null)
                filter = ColorFilterConfig.Default;
            else
                filter = (ColorFilterConfig)config;

            List<Color> colors = new List<Color>();
            Random rand = new Random(0);

            for (int rows = 0; rows < image.Height; rows++)
            {
                Span<Argb32> rowPixels = image.GetPixelRowSpan(rows);
                for (int i = 0; i < rowPixels.Length; i += quality)
                {
                    byte b = rowPixels[i].B;
                    byte g = rowPixels[i].G;
                    byte r = rowPixels[i].R;
                    byte a = rowPixels[i].A;

                    Color color = Color.FromArgb(a, r, g, b);

                    if (filter.TakeColor(color))
                        colors.Add(color);
                }
            }

            if (colors.Count < 16)
            {
                return GetImageColors(image, quality, filter.Ease());
            }

            return colors;
        }

        private static async Task<Stream?> GetImageStreamAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            var response = await HttpWebRequest.CreateHttp(uri).GetResponseAsync();
            return response.GetResponseStream();
        }
    }
}
