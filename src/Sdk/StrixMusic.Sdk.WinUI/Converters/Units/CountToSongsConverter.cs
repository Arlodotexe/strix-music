using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services;
using Windows.UI.Xaml.Data;
using StrixMusic.Sdk.WinUI.Services.Localization;

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
            var localizationService = Ioc.Default.GetService<LocalizationResourceLoader>();

            return localizationService?.Music is null ?
                value.ToString() :
                string.Format(localizationService.Music.GetString("SongsCount"), value);
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
