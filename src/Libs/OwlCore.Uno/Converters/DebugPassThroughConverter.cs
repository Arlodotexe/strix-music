using System;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            Ioc.Default.GetRequiredService<ILogger<DebugPassThroughConverter>>().LogInformation($"Debug passthrough: Type is {value?.GetType().ToString() ?? "null"}");
            return value;
        }

        /// <inheritdoc/>
        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
