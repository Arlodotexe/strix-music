using System;
using CommunityToolkit.Diagnostics;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Converters
{
    /// <summary>
    /// Converts a <see cref="StrixMusicShells"/> or <see cref="AdaptiveShells"/> enum value to a string containing a description of the shell.
    /// </summary>
    public class ShellEnumToDescriptionConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language) => value switch
        {
            StrixMusicShells strixShells => strixShells switch
            {
                StrixMusicShells.ZuneDesktop => "A faithful recreation of the iconic Zune Desktop app.",
                StrixMusicShells.GrooveMusic => "A faithful recreation of the Groove Music app from Windows 10",
                StrixMusicShells.Sandbox => "Used by developers to test components and create inbox controls.",
                _ => ThrowHelper.ThrowNotSupportedException<string>(),
            },
            AdaptiveShells adaptiveShells => adaptiveShells switch
            {
                AdaptiveShells.GrooveMusic => Convert(StrixMusicShells.GrooveMusic, targetType, parameter, language),
                AdaptiveShells.Sandbox => Convert(StrixMusicShells.Sandbox, targetType, parameter, language),
                _ => ThrowHelper.ThrowNotSupportedException<string>(),
            },
            _ => ThrowHelper.ThrowNotSupportedException<string>(),
        };

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
