using System;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// A converter that converts a given <see cref="long"/> to a <see cref="TimeSpan"/> then to a natural time format string.
    /// </summary>
    public class LongToTimeSpanTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a formatted string.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to convert.</param>
        /// <returns>A formatted string of the <see cref="TimeSpan"/>.</returns>
        public static string Convert(object value)
        {
            TimeSpan timeSpan;
            if (value is double dValue)
            {
                timeSpan = TimeSpan.FromMilliseconds(dValue);
            } 
            else if (value is TimeSpan tsValue)
            {
                timeSpan = tsValue;
            }
            else
            {
                return string.Empty;
            }

            if (timeSpan.Hours > 0)
            {
                return timeSpan.ToString(@"h\:mm\:ss");
            }
            else
            {
                return timeSpan.ToString(@"m\:ss");
            }
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
