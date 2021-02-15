using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace OwlCore.Uno
{
    public partial class Converters
    {
        /// <summary>
        /// Converts a <see cref="long"/> to a <see cref="double"/>.
        /// </summary>
        /// <param name="input">The <see cref="long"/>  to convert</param>
        /// <returns>The converted value.</returns>
        public static double LongToDouble(long input)
        {
            return input;
        }
    }

    /// <inheritdoc cref="Converters.LongToDouble" />
    public class LongToDoubleConverter : IValueConverter
    {
        /// <inheritdoc cref="Converters.LongToDouble" />
        public static double Convert(object value)
        {
            return Converters.LongToDouble((long)value);
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
