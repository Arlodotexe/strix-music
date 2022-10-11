using System.Collections.Generic;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;

namespace StrixMusic.Services
{
    /// <summary>
    /// A container for keys of all settings throughout the main app.
    /// </summary>
    public class AppSettings : SettingsBase
    {
        private static bool _isDebug =
#if DEBUG
            true;
#else
            false;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        /// <param name="folder">The folder where app settings are stored.</param>
        public AppSettings(IModifiableFolder folder)
            : base(folder, AppSettingsSerializer.Singleton)
        {
            LoadFailed += AppSettings_LoadFailed;
            SaveFailed += AppSettings_SaveFailed;
        }

        /// <summary>
        /// Gets the list of all registered storage cores that interact with files on disk.
        /// </summary>
        public List<LocalFilesCoreSettings> ConfiguredLocalFileCores => GetSetting(() => new List<LocalFilesCoreSettings>());

        /// <summary>
        /// Gets the list of all registered storage cores that interact with OneDrive.
        /// </summary>
        public List<OneDriveCoreSettings> ConfiguredOneDriveCores => GetSetting(() => new List<OneDriveCoreSettings>());

        /// <summary>
        /// The user's preferred ranking for each core, stored as the core's instance ID. Highest ranking first.
        /// </summary>
        public List<string> CoreRanking
        {
            get => GetSetting(() => new List<string>());
            set => SetSetting(value);
        }

        /// <summary>
        /// If true, the app will log information about the app while its running.
        /// </summary>
        public bool IsLoggingEnabled
        {
            get => GetSetting(() => _isDebug);
            set => SetSetting(value);
        }

        /// <summary>
        /// Stores the registry id of the user's preferred shell.
        /// </summary>
        public string PreferredShell
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// The registry id of the user's current fallback shell. Used to cover display sizes that the <see cref="PreferredShell"/> doesn't support. 
        /// </summary>
        public string FallbackShell
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        private void AppSettings_SaveFailed(object? sender, SettingPersistFailedEventArgs e)
        {
            Logger.LogError($"Failed to save setting {e.SettingName}", e.Exception);
        }

        private void AppSettings_LoadFailed(object? sender, SettingPersistFailedEventArgs e)
        {
            Logger.LogError($"Failed to load setting {e.SettingName}", e.Exception);
        }
    }
}
