using System;
using StrixMusic.Sdk.WinUI.Globalization;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.WinUI.Converters.Units
{
    /// <summary>
    /// A converter that adds a "Songs" suffix to a unit.
    /// </summary>
    public sealed class CountToSongsConverter : IValueConverter
    {
        /// <summary>
        /// Adds a "Songs" suffix to a unit.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert</param>
        /// <returns>The converted value.</returns>
        public static string Convert(int value)
        {
            return LocalizationResources.Music is null ?
                value.ToString() :
                string.Format(LocalizationResources.Music.GetString("SongsCount"), value);
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((int)value);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
