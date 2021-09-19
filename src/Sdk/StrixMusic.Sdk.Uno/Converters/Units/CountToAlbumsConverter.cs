using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Helpers;
using StrixMusic.Sdk.Uno.Services.Localization;
using System;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.Uno.Converters.Units
{
    /// <summary>
    /// A converter that adds an "Albums" suffix to a unit..
    /// </summary>
    public class CountToAlbumsConverter : IValueConverter
    {
        /// <summary>
        /// Adds a "Albums" suffix to a unit.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert</param>
        /// <returns>The converted value.</returns>
        public static string Convert(int value)
        {
            LocalizationResourceLoader localizationService = Ioc.Default.GetRequiredService<LocalizationResourceLoader>();
            return string.Format(localizationService[Constants.Localization.MusicResource, "AlbumsCount"], value);
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
