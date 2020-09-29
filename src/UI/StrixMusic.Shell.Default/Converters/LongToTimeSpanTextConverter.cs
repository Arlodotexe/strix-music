using System;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Shell.Default.Converters
{
    /// <summary>
    /// A converter that converts a given <see cref="long"/> to a <see cref="TimeSpan"/> then to a natural time format string.
    /// </summary>
    public class LongToTimeSpanTextConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double dValue)
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(dValue);

                if (timeSpan.Hours > 0)
                {
                    return timeSpan.ToString(@"h\:mm\:ss");
                } 
                else
                {
                    return timeSpan.ToString(@"m\:ss");
                }
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
