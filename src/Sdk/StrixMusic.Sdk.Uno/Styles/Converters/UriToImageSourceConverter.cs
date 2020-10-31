using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// A simple converter that converts a given <see cref="Uri"/> to an <see cref="BitmapImage"/>.
    /// </summary>
    public sealed class UriToImageSourceConverter
    {
        /// <summary>
        /// Converts a <see cref="Uri"/> or url string to a <see cref="BitmapImage"/>.
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
            else if (value is ICoreImage iValue)
            {
                uri = iValue.Uri;
            }
            else if (value is ICollection<ICoreImage> imageCollection && imageCollection.Any())
            {
                uri = imageCollection.First().Uri;
            }

            if (uri != null)
            {
                return new BitmapImage(uri);
            }

            return null;
        }
    }
}
