using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
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
using CommunityToolkit.Mvvm.Input;

namespace StrixMusic.Settings;

/// <summary>
/// A container for all settings related to debugging and diagnostics.
/// </summary>
public partial class DiagnosticSettings : SettingsBase
{
    [JsonIgnore]
    private readonly SemaphoreSlim _saveLoadMutex = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicSourcesSettings"/> class.
    /// </summary>
    /// <param name="folder">The folder where settings are stored.</param>
    public DiagnosticSettings(IModifiableFolder folder)
        : base(folder, AppSettingsSerializer.Singleton)
    {

        FlushDefaultValues = false;
        LoadFailed += DebugSettings_LoadFailed;
        SaveFailed += DebugSettings_SaveFailed;
    }

    /// <summary>
    /// Gets or sets a value that indicates if logging is enabled.
    /// </summary>
    public bool IsLoggingEnabled
    {
        get => GetSetting(() =>
        {
            #if DEBUG
            return true;
            #else
            return false;
            #endif
        });
        set => SetSetting(value);
    }

    private void DebugSettings_SaveFailed(object? sender, SettingPersistFailedEventArgs e) => Logger.LogError($"Failed to save setting {e.SettingName}", e.Exception);

    private void DebugSettings_LoadFailed(object? sender, SettingPersistFailedEventArgs e) => Logger.LogError($"Failed to load setting {e.SettingName}", e.Exception);

    private async Task<IModifiableFolder> GetDataFolderByName(string name)
    {
        var folder = await Folder.CreateFolderAsync(name, overwrite: false);

        if (folder is not IModifiableFolder modifiableData)
            throw new InvalidOperationException($"A new folder was opened/created in the data folder, but it's not modifiable.");

        return modifiableData;
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
            {
                
                Logger.LogInformation($"Loading {nameof(MusicSourcesSettings)}");
                await base.LoadAsync(cancellationToken);
            }
        }
    }
}
