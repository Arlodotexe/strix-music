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
        /// <remarks>
        /// https://social.msdn.microsoft.com/Forums/windowsapps/en-US/02927d7a-077f-4263-8b60-b5567baed94b/uwp-convert-the-bitmapimage-into-writeablebitmap
        /// </remarks>
        /// <returns>A pixel array.</returns>
        public static async Task<byte[]?> GetPixels(BitmapImage image, uint width, uint height)
        {
            Uri? localUri = await GetLocalImageAsync(image.UriSource.AbsoluteUri);

            if (localUri is null)
            {
                return null;
            }

            RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(localUri);
            using (IRandomAccessStream stream = await random.OpenReadAsync())
            {
                //Create a decoder for the image
                var decoder = await BitmapDecoder.CreateAsync(stream);

                //Initialize bitmap transformations to be applied to the image.
                var transform = new BitmapTransform() { ScaledWidth = width, ScaledHeight = height, InterpolationMode = BitmapInterpolationMode.Cubic };

                //Get image pixels.
                var pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);
                var pixels = pixelData.DetachPixelData();

                return pixels;
            }
        }

        /// <summary>
        /// Gets a list of colors in an image.
        /// </summary>
        /// <param name="image">The image to read from.</param>
        /// <param name="quality">The amount of pixels to skip (lower is more accurate).</param>
        /// <param name="ignoreWhite">Whether or not to skip white pixels.</param>
        /// <param name="whiteTolerance">How close a pixel must be to white to be considered white.</param>
        /// <returns>A list of colors in the image.</returns>
        public static List<Color> GetImageColors(
            byte[] pixels,
            int quality = 4,
            bool ignoreWhite = true,
            float whiteTolerance = .05f,
            bool ignoreBlack = true,
            float blackTolerance = .10f)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < pixels.Length / 4; i += quality)
            {
                var offset = i * 4;
                byte b = pixels[offset];
                byte g = pixels[offset + 1];
                byte r = pixels[offset + 2];
                byte a = pixels[offset + 3];

                Color color = Color.FromArgb(a, r, g, b);

                if (ignoreBlack && color.GetValue() < blackTolerance)
                    continue;

                if (ignoreWhite && color.GetCMin() > 1 - whiteTolerance)
                    continue;

                colors.Add(color);
            }

            return colors;
        }

        private static async Task<Uri?> GetLocalImageAsync(string uri)
        {
            string name = "parseBuffer";

            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            using (var response = await HttpWebRequest.CreateHttp(uri).GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    var desiredName = string.Format("{0}.jpg", name);
                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(desiredName, CreationCollisionOption.ReplaceExisting);

                    using (var filestream = await file.OpenStreamForWriteAsync())
                    {
                        await stream.CopyToAsync(filestream);
                        return new Uri(string.Format("ms-appdata:///local/{0}.jpg", name), UriKind.Absolute);
                    }
                }
            }
        }
    }
}
