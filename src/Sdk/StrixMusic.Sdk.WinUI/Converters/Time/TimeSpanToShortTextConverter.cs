using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.WinUI.Services.Localization;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.WinUI.Converters.Time
{
    /// <summary>
    /// A converter that converts a given <see cref="TimeSpan"/> to a natural time format string.
    /// </summary>
    /// <example>
    ///  "1 Hr 4 Min 40 Sec"
    /// </example>
    public sealed class TimeSpanToShortTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a formatted string.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to convert.</param>
        /// <returns>A formatted string of the <see cref="TimeSpan"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Convert(TimeSpan value)
        {
            var localizationService = Ioc.Default.GetRequiredService<LocalizationResourceLoader>();

            var returnValue = string.Empty;

            // TODO: Make more rigorous cases
            if (value.Hours > 0)
                returnValue += string.Format(localizationService.Time?.GetString("HrCount") ?? string.Empty, value.Hours) + " ";

            if (value.Minutes > 0)
                returnValue += string.Format(localizationService.Time?.GetString("MinCount") ?? string.Empty, value.Minutes) + " ";

            if (value.Seconds > 0 || returnValue == string.Empty)
                returnValue += string.Format(localizationService.Time?.GetString("SecCount") ?? string.Empty, value.Seconds) + " ";

            return returnValue;
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                return Convert(timeSpan);
            }

            return Convert(TimeSpan.Zero);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
