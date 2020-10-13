using System;
using StrixMusic.Sdk.Interfaces;
using Windows.UI.Xaml.Media.Imaging;

namespace StrixMusic.Shell.Default.Converters
{
    /// <summary>
    /// A simple converter that converts a given <see cref="Uri"/> to an <see cref="BitmapImage"/>.
    /// </summary>
    public sealed class UriToImageSourceConverter
    {
        /// <summary>
        /// Converts a <see cref="Uri"/> or url string to an <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="value">The uri or url string.</param>
        /// <returns>A <see cref="BitmapImage"/>.</returns>
        public static BitmapImage? Convert(object value)
        {
            Uri? uri = null;
            if (value is Uri)
            {
                uri = value as Uri;
            }
            else if (value is string sValue)
            {
                Uri.TryCreate(sValue, UriKind.Absolute, out uri);
            }
            else if (value is IImage iValue)
            {
                uri = iValue.Uri;
            }

            if (uri != null)
            {
                return new BitmapImage(uri);
            }

            return null;
        }
    }
}
