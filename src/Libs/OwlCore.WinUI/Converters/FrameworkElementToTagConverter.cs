using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OwlCore.WinUI.Converters;

/// <summary>
/// Converts the provided <see cref="FrameworkElement"/> to the value held in its' <see cref="FrameworkElement.Tag"/>.
/// </summary>
public class FrameworkElementToTagConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is FrameworkElement element)
            return element.Tag;

        return value;
    }
        
    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
