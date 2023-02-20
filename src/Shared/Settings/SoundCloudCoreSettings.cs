using System;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;
using OwlCore.Storage.Memory;
using StrixMusic.AppModels;

namespace StrixMusic.Settings;

/// <summary>
/// A container for the settings needed to instantiate a <see cref="SoundCloudFolder"/>.
/// </summary>
public class SoundCloudCoreSettings : CoreSettingsBase, IInstanceId
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoundCloudCoreSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where app settings are stored.</param>
    public SoundCloudCoreSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
        LoadFailed += AppSettings_LoadFailed;
        SaveFailed += AppSettings_SaveFailed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicSourcesSettings"/> class.
    /// </summary>
    public SoundCloudCoreSettings()
        : this(new MemoryFolder(Guid.NewGuid().ToString(), nameof(SoundCloudCoreSettings)))
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
    /// Gets or sets the instance ID of the music source.
    /// </summary>
    public string InstanceId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets an ID that represents the authenticated user.
    /// </summary>
    public string UserDisplayName
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
    /// Gets or sets the client ID that the SoundCloud instance authentates with. 
    /// </summary>
    public string ClientId
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <summary>
    /// Gets or sets the OAuth token that the SoundCloud instance authentates with. 
    /// </summary>
    public string Token
    {
        get => GetSetting(() => string.Empty);
        set => SetSetting(value);
    }

    /// <inheritdoc />
    public override bool IsSettingValidForCoreCreation(string propertyName, object? value) => propertyName switch
    {
        nameof(InstanceId) or nameof(UserId) or nameof(ClientId) or nameof(Token)
            => !string.IsNullOrWhiteSpace((string?)value ?? string.Empty),
        _ => true,
    };

    /// <inheritdoc />
    public override object GetSettingByName(string settingName) => settingName switch
    {
        nameof(InstanceId) => InstanceId,
        nameof(UserId) => UserId,
        nameof(ClientId) => ClientId,
        nameof(Token) => Token,
        _ => throw new ArgumentOutOfRangeException(nameof(settingName), settingName, @"Unknown setting name specified.")
    };
}
