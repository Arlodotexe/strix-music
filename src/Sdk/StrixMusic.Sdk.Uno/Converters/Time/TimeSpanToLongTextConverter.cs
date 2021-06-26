using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Uno.Helpers;
using StrixMusic.Sdk.Uno.Services.Localization;
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.Uno.Converters.Time
{
    /// <summary>
    /// A converter that converts a given <see cref="TimeSpan"/> to a natural time format string.
    /// </summary>
    public sealed class TimeSpanToLongTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a formatted string.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to convert.</param>
        /// <returns>A formatted string of the <see cref="TimeSpan"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Convert(TimeSpan value)
        {
            var localizationService = Ioc.Default.GetService<LocalizationResourceLoader>();

            if (localizationService == null)
                return Constants.Localization.LocalizationErrorString;

            string returnValue = string.Empty;

            // TODO: Make more rigorous cases
            if (value.Hours >= 2)
                returnValue += string.Format(localizationService[Constants.Localization.TimeResource, "HoursCount"], value.Hours) + " ";
            else if (value.Hours == 1)
                returnValue += localizationService[Constants.Localization.TimeResource, "HourSingular"] + " ";

            if (value.Minutes >= 2)
                returnValue += string.Format(localizationService[Constants.Localization.TimeResource, "MinutesCount"], value.Minutes) + " ";
            else if (value.Minutes == 1)
                returnValue += localizationService[Constants.Localization.TimeResource, "MinuteSingular"] + " ";

            if (value.Seconds >= 2)
                returnValue += string.Format(localizationService[Constants.Localization.TimeResource, "SecondsCount"], value.Seconds) + " ";
            else if (value.Seconds == 1)
                returnValue += localizationService[Constants.Localization.TimeResource, "SecondSingular"] + " ";

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
