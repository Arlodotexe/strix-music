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
public class LocalFilesCoreSettings : SettingsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFilesCoreSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where app settings are stored.</param>
    public LocalFilesCoreSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
        LoadFailed += AppSettings_LoadFailed;
        SaveFailed += AppSettings_SaveFailed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFilesCoreSettings"/> class.
    /// </summary>
    public LocalFilesCoreSettings()
        : this(new MemoryFolder(Guid.NewGuid().ToString(), nameof(LocalFilesCoreSettings)))
    {
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
    /// Gets or sets an ID that represents the authenticated user.
    /// </summary>
    public string Path
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }
}
