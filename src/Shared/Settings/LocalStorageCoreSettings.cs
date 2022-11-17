using System;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;
using OwlCore.Storage.Memory;
using OwlCore.Storage.Uwp;

namespace StrixMusic.Services;

/// <summary>
/// A container for the settings needed to instantiate a <see cref="WindowsStorageFolder"/>.
/// </summary>
public class LocalStorageCoreSettings : SettingsBase, IInstanceId
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStorageCoreSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where app settings are stored.</param>
    public LocalStorageCoreSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
        InstanceId = folder.Id;
        LoadFailed += AppSettings_LoadFailed;
        SaveFailed += AppSettings_SaveFailed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStorageCoreSettings"/> class.
    /// </summary>
    /// <remarks>
    /// This overload is backed with a MemoryFolder.
    /// Acts as a memory-only settings instance where properties can be set but don't need to be saved to disk. Required for serializers.
    /// </remarks>
    public LocalStorageCoreSettings()
        : this(new MemoryFolder(Guid.NewGuid().ToString(), nameof(LocalStorageCoreSettings)))
    {
    }

    /// <summary>
    /// Copies all settings from once instance into another.
    /// </summary>
    public static void CopyFrom(LocalStorageCoreSettings from, LocalStorageCoreSettings to)
    {
        to.InstanceId = from.InstanceId;
        to.Path = from.Path;
        to.FutureAccessToken = from.FutureAccessToken;
    }

    private void AppSettings_SaveFailed(object? sender, SettingPersistFailedEventArgs e)
    {
        Logger.LogError($"Failed to save setting {e.SettingName}", e.Exception);
    }

    private void AppSettings_LoadFailed(object? sender, SettingPersistFailedEventArgs e)
    {
        Logger.LogError($"Failed to load setting {e.SettingName}", e.Exception);
    }

    /// <summary>
    /// Gets or sets the instance ID of the core.
    /// </summary>
    public string InstanceId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets an ID 
    /// </summary>
    public string Path
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets an ID that 
    /// </summary>
    public string FutureAccessToken
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }
}
