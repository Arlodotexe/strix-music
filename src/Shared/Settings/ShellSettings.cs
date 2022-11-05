using System;
using System.Runtime.CompilerServices;
using Windows.Storage;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Storage;
using OwlCore.Storage.Memory;

namespace StrixMusic.Services;

/// <summary>
/// A container for settings related to shells.
/// </summary>
public class ShellSettings : SettingsBase, IInstanceId
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where app settings are stored.</param>
    public ShellSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {
        InstanceId = folder.Id;
        LoadFailed += AppSettings_LoadFailed;
        SaveFailed += AppSettings_SaveFailed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellSettings"/> class.
    /// </summary>
    /// <remarks>
    /// This overload is backed with a MemoryFolder.
    /// Acts as a memory-only settings instance where properties can be set but don't need to be saved to disk. Required for serializers.
    /// </remarks>
    public ShellSettings()
        : this(new MemoryFolder(Guid.NewGuid().ToString(), nameof(ShellSettings)))
    {
    }

    /// <summary>
    /// Copies all settings from once instance into another.
    /// </summary>
    public static void CopyFrom(ShellSettings from, ShellSettings to)
    {
        to.InstanceId = from.InstanceId;
        to.PreferredShell = from.PreferredShell;
        to.FallbackShell = from.FallbackShell;
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
    /// Stores the registry id of the user's preferred shell.
    /// </summary>
    public StrixMusicShells PreferredShell
    {
        get => GetSettingEx(() => StrixMusicShells.ZuneDesktop);
        set => SetSettingEx(value);
    }

    /// <summary>
    /// The registry id of the user's current fallback shell. Used to cover display sizes that the <see cref="PreferredShell"/> doesn't support. 
    /// </summary>
    public AdaptiveShells FallbackShell
    {
        get => GetSettingEx(() => AdaptiveShells.GrooveMusic);
        set => SetSettingEx(value);
    }

    private T GetSettingEx<T>(Func<T> getDefaultValue, [CallerMemberName] string key = "")
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var value) && value is T savedValue)
            return savedValue;

        return GetSetting(getDefaultValue, key);
    }

    private void SetSettingEx<T>(T value, [CallerMemberName] string key = "")
    {
        ApplicationData.Current.LocalSettings.Values[key] = value;
        SetSetting(value, key);
    }
}
