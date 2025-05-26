using System;
using CommunityToolkit.Diagnostics;
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
                    ZuneSortState.AZ => loacalizationService.GetString("A-Z") ?? ThrowHelper.ThrowArgumentNullException<string>(),
                    ZuneSortState.ZA => loacalizationService.GetString("Z-A") ?? ThrowHelper.ThrowArgumentNullException<string>(),
                    ZuneSortState.Artists => loacalizationService.GetString("By Artists") ?? ThrowHelper.ThrowArgumentNullException<string>(),
                    ZuneSortState.ReleaseYear => loacalizationService.GetString("By Release Year") ?? ThrowHelper.ThrowArgumentNullException<string>(),
                    ZuneSortState.DateAdded => loacalizationService.GetString("By Date Added") ?? ThrowHelper.ThrowArgumentNullException<string>(),
                    _ => throw new NotImplementedException(),
                };
            }

            return string.Empty;
        }

        ///<inheritdoc  cref="ZuneSortStateToStringConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
