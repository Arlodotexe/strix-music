using OwlCore.ComponentModel;
using OwlCore.Storage;
using StrixMusic.Sdk.Services;
using StrixMusic.Shells.ZuneDesktop.Settings.Models;

namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// Contains settings specific to the Zune Desktop shell.
    /// </summary>
    public class ZuneDesktopSettings : SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettings"/> class.
        /// </summary>
        public ZuneDesktopSettings(IModifiableFolder settingsStorage)
            : base(settingsStorage, SystemJsonStreamSerializer.Singleton)
        {
        }

        /// <summary>
        /// Gets the default value for <see cref="BackgroundImage"/> in settings.
        /// </summary>
        public ZuneDesktopBackgroundImage BackgroundImage
        {
            get => GetSetting(() => new ZuneDesktopBackgroundImage());
            set => SetSetting(value);
        }
    }
}
