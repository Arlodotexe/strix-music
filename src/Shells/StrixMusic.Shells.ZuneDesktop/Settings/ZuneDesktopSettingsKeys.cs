using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services;
using StrixMusic.Shells.ZuneDesktop.Settings.Models;

namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the Zune shell.
    /// </summary>
    public class ZuneDesktopSettingsKeys : SettingsKeysBase
    {
        /// <summary>
        /// Gets the default value for <see cref="BackgroundImage"/> in settings.
        /// </summary>
        public static readonly ZuneDesktopBackgroundImage BackgroundImage = new ZuneDesktopBackgroundImage();

        /// <inheritdoc />
        public override object GetDefaultValue(string settingKey)
        {
            return settingKey switch
            {
                nameof(BackgroundImage) => BackgroundImage,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(),
            };
        }
    }
}
