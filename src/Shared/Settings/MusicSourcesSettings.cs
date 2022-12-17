﻿using System;
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
using OwlCore.Extensions;

namespace StrixMusic.Settings;

/// <summary>
/// A container for all settings related to cores.
/// </summary>
public class MusicSourcesSettings : SettingsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MusicSourcesSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where settings are stored.</param>
    public MusicSourcesSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
        FlushDefaultValues = false;
        LoadFailed += AppSettings_LoadFailed;
        SaveFailed += AppSettings_SaveFailed;

        AvailableMusicSources.Add(new AvailableMusicSource
        (
            name: "Local Storage",
            description: "Listen to music on your local disk.",
            imageFactory: () => CoreImageFromApplicationPathAsync("ms-appx:///Assets/Cores/LocalStorage/Logo.svg"),
            defaultSettingsFactory: async instanceId => new LocalStorageCoreSettings(await GetDataFolderByName(instanceId.HashMD5Fast())))
        );

        AvailableMusicSources.Add(new AvailableMusicSource
        (
            name: "OneDrive",
            description: "Stream music directly from OneDrive.",
            imageFactory: () => CoreImageFromApplicationPathAsync("ms-appx:///Assets/Cores/OneDrive/Logo.svg"),
            defaultSettingsFactory: async instanceId => new OneDriveCoreSettings(await GetDataFolderByName(instanceId.HashMD5Fast())))
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
    public ObservableCollection<AvailableMusicSource> AvailableMusicSources { get; } = new();

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
