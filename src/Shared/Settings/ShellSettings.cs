using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using OwlCore.Storage.Memory;
using StrixMusic.AppModels;
using Windows.Storage;

namespace StrixMusic.Settings;

/// <summary>
/// A container for settings related to shells.
/// </summary>
public partial class ShellSettings : SettingsBase, IInstanceId
{
    [JsonIgnore]
    private readonly SemaphoreSlim _saveLoadMutex = new(1, 1);

    [JsonIgnore]
    private readonly ApplicationDataContainer? _localSettings;

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

#if !__WASM__
        _localSettings = ApplicationData.Current.LocalSettings.CreateContainer(nameof(ShellSettings), ApplicationDataCreateDisposition.Always);
# endif
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
        set => SetSettingEx(PreferredShell, value);
    }

    /// <summary>
    /// The registry id of the user's current fallback shell. Used to cover display sizes that the <see cref="PreferredShell"/> doesn't support. 
    /// </summary>
    public AdaptiveShells FallbackShell
    {
        get => GetSettingEx(() => AdaptiveShells.GrooveMusic);
        set => SetSettingEx(FallbackShell, value);
    }

    /// <inheritdoc />
    [RelayCommand]
    public override async Task SaveAsync(CancellationToken? cancellationToken = null)
    {
        // Subsequent concurrent calls to this method will wait for the first call to complete, without saving settings again.
        // Forces the AsyncRelayCommand to wait for raw method calls to complete.
        var wasSavingOnEntry = _saveLoadMutex.CurrentCount == 0;

        using (await _saveLoadMutex.DisposableWaitAsync())
        {
            if (!wasSavingOnEntry)
                await base.SaveAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    [RelayCommand]
    public override async Task LoadAsync(CancellationToken? cancellationToken = null)
    {
        // Subsequent concurrent calls to this method will wait for the first call to complete, without loading settings again.
        // Forces the AsyncRelayCommand to wait for raw method calls to complete.
        var wasLoadingOnEntry = _saveLoadMutex.CurrentCount == 0;

        using (await _saveLoadMutex.DisposableWaitAsync())
        {
            if (!wasLoadingOnEntry)
                await base.LoadAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    [RelayCommand]
    public override void ResetAllSettings() => base.ResetAllSettings();

    private T GetSettingEx<T>(Func<T> getDefaultValue, [CallerMemberName] string key = "")
    {
        if (_localSettings is null)
            return GetSetting(getDefaultValue, key);

        if (_localSettings.Values.TryGetValue(key, out var value))
        {
            try
            {
                var savedValue = AppSettingsSerializer.Singleton.Deserialize<T>(new MemoryStream(System.Text.Encoding.UTF8.GetBytes($"{value}")));
                return savedValue;
            }
            catch
            {
                // ignored
            }
        }

        return GetSetting(getDefaultValue, key);
    }

    private void SetSettingEx<T>(T currentValue, T value, [CallerMemberName] string key = "")
    {
        if (Equals(value, currentValue))
            return;

        if (_localSettings is null)
        {
            SetSetting(value, key);
            return;
        }

        var localSettingsValue = System.Text.Encoding.UTF8.GetString(AppSettingsSerializer.Singleton.Serialize(value).ToBytes());

        if (_localSettings.Values.ContainsKey(key))
            _localSettings.Values[key] = localSettingsValue;
        else
            _localSettings.Values.Add(key, localSettingsValue);

        SetSetting(value, key);
    }
}
