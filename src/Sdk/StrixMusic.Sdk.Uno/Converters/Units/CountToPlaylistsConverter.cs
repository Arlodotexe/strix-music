using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Uno.Helpers;
using StrixMusic.Sdk.Uno.Services.Localization;
using System;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Converters.Units
{
    /// <summary>
    /// A converter that adds a "Playlists" suffix to a unit..
    /// </summary>
    public class CountToPlaylistsConverter : IValueConverter
    {
        /// <summary>
        /// Adds a "Playlists" suffix to a unit.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert</param>
        /// <returns>The converted value.</returns>
        public static string Convert(int value)
        {
            var localizationService = Ioc.Default.GetService<LocalizationResourceLoader>();

            if (localizationService == null)
                return Constants.Localization.LocalizationErrorString;

            return string.Format(localizationService[Constants.Localization.MusicResource, "PlaylistsCount"], value);
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
