using System;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// A converter that converts a given <see cref="long"/> to a <see cref="TimeSpan"/> then to a natural time format string.
    /// </summary>
    public class DateTimeToYearTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a formatted string.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to convert.</param>
        /// <returns>A formatted string of the <see cref="TimeSpan"/>.</returns>
        public static string Convert(object value)
        {
            DateTime dateTime;
            if (value is long dValue)
            {
                dateTime = DateTime.FromBinary(dValue);
            } 
            else if (value is DateTime dtValue)
            {
                dateTime = dtValue;
            }
            else
            {
                return string.Empty;
            }

            return dateTime.Year.ToString();
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
