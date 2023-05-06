using System;
using Windows.UI.Xaml.Data;

namespace OwlCore.WinUI.Converters;

/// <summary>
/// Converts the provided object to it's equivalent Type object. 
/// </summary>
public class ObjectToTypeConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language) => value.GetType();

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
