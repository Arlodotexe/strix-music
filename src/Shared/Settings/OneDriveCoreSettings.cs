using System;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;
using OwlCore.Storage.Memory;
using OwlCore.Storage.OneDrive;

namespace StrixMusic.Services;

/// <summary>
/// A container for the settings needed to instantiate a <see cref="OneDriveFolder"/>.
/// </summary>
public class OneDriveCoreSettings : SettingsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneDriveCoreSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where app settings are stored.</param>
    public OneDriveCoreSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
        LoadFailed += AppSettings_LoadFailed;
        SaveFailed += AppSettings_SaveFailed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettings"/> class.
    /// </summary>
    public OneDriveCoreSettings()
        : this(new MemoryFolder(Guid.NewGuid().ToString(), nameof(OneDriveCoreSettings)))
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
    public string UserId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets the ID of the OneDrive folder.
    /// </summary>
    public string FolderId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets the client ID of the registered Azure application. 
    /// </summary>
    public string ClientId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets the tenant ID that the registered Azure application belongs to. 
    /// </summary>
    public string TenantId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets the redirect uri to use for authentication. Must match one registered with the Azure application. 
    /// </summary>
    public string RedirectUri
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }
}
