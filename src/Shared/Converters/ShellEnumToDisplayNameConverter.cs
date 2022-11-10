using System;
using CommunityToolkit.Diagnostics;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Converters
{
    /// <summary>
    /// Converts a <see cref="StrixMusicShells"/> or <see cref="AdaptiveShells"/> enum value to a string format.
    /// </summary>
    public class ShellEnumToDisplayNameConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language) => value switch
        {
            StrixMusicShells strixShells => strixShells switch
            {
                StrixMusicShells.ZuneDesktop => "Zune Desktop",
                StrixMusicShells.GrooveMusic => "Groove Music",
                StrixMusicShells.Sandbox => "Sandbox",
                _ => ThrowHelper.ThrowNotSupportedException<string>(),
            },
            AdaptiveShells adaptiveShells => adaptiveShells switch
            {
                AdaptiveShells.GrooveMusic => "Groove Music",
                AdaptiveShells.Sandbox => "Sandbox",
                _ => ThrowHelper.ThrowNotSupportedException<string>(),
            },
            _ => ThrowHelper.ThrowNotSupportedException<string>(),
        };

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(StrixMusicShells))
            {
                return value switch
                {
                    "Zune Desktop" => StrixMusicShells.ZuneDesktop,
                    "Groove Music" => StrixMusicShells.GrooveMusic,
                    "Sandbox" => StrixMusicShells.Sandbox,
                    _ => ThrowHelper.ThrowNotSupportedException<StrixMusicShells>(),
                };
            }

            if (targetType == typeof(AdaptiveShells))
            {
                return value switch
                {
                    "Groove Music" => AdaptiveShells.GrooveMusic,
                    "Sandbox" => AdaptiveShells.Sandbox,
                    _ => ThrowHelper.ThrowNotSupportedException<AdaptiveShells>(),
                };
            }

            return ThrowHelper.ThrowNotSupportedException<StrixMusicShells>();
        }
    }
}
