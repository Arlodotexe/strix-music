using System;
using Windows.UI.Xaml.Data;

namespace OwlCore.Uno.Converters
{
    /// <summary>
    /// A converter used for debugging the data passed by a binding.
    /// </summary>
    public sealed class DebugPassThroughConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            System.Diagnostics.Debug.Write($"Debug passthrough: Type is {value?.GetType().ToString() ?? "null"}");
            return value;
        }

        /// <inheritdoc/>
        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
