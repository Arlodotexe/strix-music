using System;
using StrixMusic.Sdk.MediaPlayback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace StrixMusic.Shells.ZuneDesktop.Converters.TrackTable
{
    /// <summary>
    /// Decides the <see cref="Brush"/> for a ForegroundColor based a playback state.
    /// </summary>
    public class PlaybackStateToForegroundConverter : IValueConverter
    {
        /// <inheritdoc cref="PlaybackStateToForegroundConverter"/>
        public static Brush Convert(PlaybackState playbackState)
        {
            switch (playbackState)
            {
                case PlaybackState.Playing:
                    return new SolidColorBrush(Color.FromArgb(255, 241, 14, 160));
                default:
                    return (SolidColorBrush)Application.Current.Resources["SystemColorWindowTextBrush"]; ;
            }
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((PlaybackState)value);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
