using StrixMusic.Sdk.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// A simple converter that converts a given <see cref="Uri"/> to an <see cref="BitmapImage"/>.
    /// </summary>
    public sealed class UriToImageSourceConverter
    {
        /// <inheritdoc cref="Convert(object, int)"/>
        public static BitmapImage? Convert(object value)
        {
            return Convert(value, 0);
        }

        /// <summary>
        /// Converts a <see cref="Uri"/> or url string to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="value">The uri or url string.</param>
        /// <param name="index">The index to get from the collection.</param>
        /// <returns>A <see cref="BitmapImage"/>.</returns>
        public static BitmapImage? Convert(object value, int index)
        {
            object? data = null;
            if (value is IEnumerable<object> uris)
            {
                var requestedItem = uris.ElementAtOrDefault(index);
                
                if (requestedItem is null)
                {
                    return null;
                }
                else
                {
                    data = requestedItem;
                }
            }

            Uri? uri = null;

            if (data is Uri)
            {
                uri = data as Uri;
            }
            else if (data is string stringUri)
            {
                Uri.TryCreate(stringUri, UriKind.Absolute, out uri);
            }
            else if (data is IImage image)
            {
                uri = image.Uri;
            }
            else if (data is ICollection<IImage> imageCollection && imageCollection.Any())
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
