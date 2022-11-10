using System.Collections.Generic;
using System.Collections.ObjectModel;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;

namespace StrixMusic.Services
{
    /// <summary>
    /// A container for all settings needed throughout the main app.
    /// </summary>
    public class AppSettings : SettingsBase
    {
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
        public ObservableCollection<LocalStorageCoreSettings> ConfiguredLocalStorageCores => GetSetting(defaultValue: () => new ObservableCollection<LocalStorageCoreSettings>());

        /// <summary>
        /// Gets the list of all registered storage cores that interact with OneDrive.
        /// </summary>
        public ObservableCollection<OneDriveCoreSettings> ConfiguredOneDriveCores => GetSetting(defaultValue: () => new ObservableCollection<OneDriveCoreSettings>());

        private void AppSettings_SaveFailed(object? sender, SettingPersistFailedEventArgs e) => Logger.LogError($"Failed to save setting {e.SettingName}", e.Exception);

        private void AppSettings_LoadFailed(object? sender, SettingPersistFailedEventArgs e) => Logger.LogError($"Failed to load setting {e.SettingName}", e.Exception);
    }
}
