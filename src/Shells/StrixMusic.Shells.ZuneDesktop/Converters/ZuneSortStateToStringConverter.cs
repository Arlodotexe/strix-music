using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection;
using Windows.ApplicationModel.Resources;
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
                var loacalizationService = ResourceLoader.GetForCurrentView("StrixMusic.Shells.ZuneDesktop/ZuneSettings");

                return zss switch
                {
                    ZuneSortState.AZ => loacalizationService.GetString("A-Z"),
                    ZuneSortState.ZA => loacalizationService.GetString("Z-A"),
                    ZuneSortState.Artists => loacalizationService.GetString("By Artists"),
                    ZuneSortState.ReleaseYear => loacalizationService.GetString("By Release Year"),
                    ZuneSortState.DateAdded => loacalizationService.GetString("By Date Added"),
                    _ => throw new NotImplementedException(),
                };
            }

            return string.Empty;
        }

        ///<inheritdoc  cref="ZuneSortStateToStringConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
