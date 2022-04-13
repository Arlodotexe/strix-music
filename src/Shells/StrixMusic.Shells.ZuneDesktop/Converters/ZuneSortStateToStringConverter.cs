using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Shells.ZuneDesktop.Converters
{
    /// <summary>
    /// Converts <see cref="ZuneSortState"/> to string.
    /// </summary>
    public class ZuneSortStateToStringConverter : IValueConverter
    {
        ///<inheritdoc  cref="ZuneSortStateToStringConverter"/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ZuneSortState zss)
            {
                return zss switch
                {
                    ZuneSortState.AZ => "A-Z",
                    ZuneSortState.ZA => "Z-A",
                    _ => throw new NotImplementedException(),
                };
            }

            return string.Empty;
        }

        ///<inheritdoc  cref="ZuneSortStateToStringConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
