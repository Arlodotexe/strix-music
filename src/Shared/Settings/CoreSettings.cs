using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;
using OwlCore.Storage.Uwp;
using StrixMusic.AppModels;
using StrixMusic.CoreModels;
using StrixMusic.Sdk.CoreModels;
using Windows.Storage;

namespace StrixMusic.Services
{
    /// <summary>
    /// A container for all settings related to cores.
    /// </summary>
    public class CoreSettings : SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreSettings"/> class.
        /// </summary>
        /// <param name="folder">The folder where settings are stored.</param>
        public CoreSettings(IModifiableFolder folder)
            : base(folder, AppSettingsSerializer.Singleton)
        {
            LoadFailed += AppSettings_LoadFailed;
            SaveFailed += AppSettings_SaveFailed;

            AvailableCores.Add(new AvailableCore
            (
                name: "Local Storage",
                description: "Listen to music on your local disk.",
                imageFactory: () => CoreImageFromApplicationPathAsync("ms-appx:///Assets/Cores/LocalFiles/Logo.svg"),
                defaultSettingsFactory: async () => new LocalStorageCoreSettings(await GetDataFolderByName($"{Guid.NewGuid()}")))
            );

            AvailableCores.Add(new AvailableCore
            (
                name: "OneDrive", 
                description: "Stream music directly from OneDrive.",
                imageFactory: () => CoreImageFromApplicationPathAsync("ms-appx:///Assets/Cores/OneDrive/Logo.svg"),
                defaultSettingsFactory: async () => new OneDriveCoreSettings(await GetDataFolderByName($"{Guid.NewGuid()}")))
            );

            async Task<ICoreImage> CoreImageFromApplicationPathAsync(string assetPath)
            {
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(assetPath));
                return new CoreFileImage(new WindowsStorageFile(storageFile));
            }
        }

        /// <summary>
        /// Gets the list of all registered storage cores that interact with files on disk.
        /// </summary>
        public ObservableCollection<LocalStorageCoreSettings> ConfiguredLocalStorageCores => GetSetting(defaultValue: () => new ObservableCollection<LocalStorageCoreSettings>());

        /// <summary>
        /// Gets the list of all registered storage cores that interact with OneDrive.
        /// </summary>
        public ObservableCollection<OneDriveCoreSettings> ConfiguredOneDriveCores => GetSetting(defaultValue: () => new ObservableCollection<OneDriveCoreSettings>());

        /// <summary>
        /// The cores that are available to be created.
        /// </summary>
        public ObservableCollection<AvailableCore> AvailableCores { get; } = new();

        private void AppSettings_SaveFailed(object? sender, SettingPersistFailedEventArgs e) => Logger.LogError($"Failed to save setting {e.SettingName}", e.Exception);

        private void AppSettings_LoadFailed(object? sender, SettingPersistFailedEventArgs e) => Logger.LogError($"Failed to load setting {e.SettingName}", e.Exception);

        private async Task<IModifiableFolder> GetDataFolderByName(string name)
        {
            var folder = await Folder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == name) ??
                         await Folder.CreateFolderAsync(name);

            if (folder is not IModifiableFolder modifiableData)
                throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

            return modifiableData;
        }
    }
}
