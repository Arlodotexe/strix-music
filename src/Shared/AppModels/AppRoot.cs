using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Storage;
using OwlCore.Storage.Memory;
using OwlCore.Storage.SystemIO;
using OwlCore.Storage.Uwp;
using StrixMusic.Cores.Storage;
using StrixMusic.MediaPlayback;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.PluginModels;
using StrixMusic.Sdk.Plugins.PlaybackHandler;
using StrixMusic.Sdk.Plugins.PopulateEmptyNames;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Settings;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using ShellSettings = StrixMusic.Settings.ShellSettings;

namespace StrixMusic.AppModels;

/// <summary>
/// The root for all data required by the Strix Music App to function.
/// </summary>
public partial class AppRoot : ObservableObject, IAsyncInit
{
    private readonly SemaphoreSlim _initMutex = new(1, 1);
    private readonly IModifiableFolder _dataFolder;
    private PlaybackHandlerService? _playbackHandler;

    [ObservableProperty]
    private StrixDataRootViewModel? _strixDataRoot;

    [ObservableProperty]
    private MergedCore? _mergedCore;

    /// <summary>
    /// Creates a new instance of <see cref="AppRoot"/>.
    /// </summary>
    public AppRoot(IModifiableFolder dataFolder)
    {
        _dataFolder = dataFolder;
        MusicSourcesSettings = new MusicSourcesSettings(dataFolder);
        ShellSettings = new ShellSettings(dataFolder);

        // Create/Remove cores when settings are added/removed.
        MusicSourcesSettings.ConfiguredLocalStorageCores.CollectionChanged += ConfiguredLocalStorageCores_OnCollectionChanged;
    }

    /// <summary>
    /// A container for the settings used throughout the app.
    /// </summary>
    public MusicSourcesSettings MusicSourcesSettings { get; set; }

    /// <summary>
    /// A container for the settings used by shells.
    /// </summary>
    public ShellSettings ShellSettings { get; set; }

    /// <summary>
    /// The media players that were created to play audio for cores.
    /// </summary>
    public ObservableCollection<MediaPlayerElement> MediaPlayerElements { get; } = new();

    /// <inheritdoc />
    [RelayCommand]
    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return;

        using (await _initMutex.DisposableWaitAsync(cancellationToken))
        {
            if (IsInitialized)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            // Avoid overwriting unsaved changes.
            // May have unsaved changed if a source was just added but settings were not saved before InitAsync is called.
            if (!MusicSourcesSettings.HasUnsavedChanges)
                await MusicSourcesSettings.LoadAsync(cancellationToken);

            // Create local storage cores
            var localStorageCores = await MusicSourcesSettings.ConfiguredLocalStorageCores.InParallel(settings => CoreFactory.CreateLocalStorageCoreAsync(settings, _dataFolder));

            // Create OneDrive cores
            var oneDriveCores = await MusicSourcesSettings.ConfiguredOneDriveCores.InParallel(x => CoreFactory.CreateOneDriveCoreAsync(x, _dataFolder));

            // Merge cores together and apply plugins
            var allCores = localStorageCores
                .Concat(oneDriveCores)
                .ToList<ICore>();

            if (!allCores.Any())
                return;

            _mergedCore = new MergedCore(allCores);
            var mergedCoreWithPlugins = CreatePluginLayer(_mergedCore);
            StrixDataRoot = new StrixDataRootViewModel(mergedCoreWithPlugins);

            IsInitialized = true;
        }
    }

    private void ConfiguredLocalStorageCores_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _ = HandleCoreSettingsCollectionChangedAsync<LocalStorageCoreSettings>(e, async x => await CoreFactory.CreateLocalStorageCoreAsync(x, _dataFolder));
    }

    private async Task HandleCoreSettingsCollectionChangedAsync<TSettings>(NotifyCollectionChangedEventArgs e, Func<TSettings, Task<ICore>> settingsToCoreFactory)
        where TSettings : SettingsBase, IInstanceId
    {
        if (!IsInitialized)
        {
            await InitAsync();
            return;
        }

        // InitAsync should populate these fields.
        Guard.IsNotNull(_mergedCore);
        Guard.IsNotNull(_strixDataRoot);
        Guard.IsNotNull(_playbackHandler);

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems.Cast<TSettings>())
            {
                var newCore = await settingsToCoreFactory(item);
                _mergedCore.AddSource(newCore);
                await InitAsync();

                SetupMediaPlayerForCore(newCore, _playbackHandler);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems.Cast<TSettings>())
            {
                var target = _mergedCore.Sources.First(x => x.InstanceId == item.InstanceId);
                _mergedCore.RemoveSource(target);
                await target.DisposeAsync();
            }
        }
    }

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    private StrixDataRootPluginWrapper CreatePluginLayer(MergedCore existingRoot)
    {
        // Apply plugin layer
        _playbackHandler ??= new PlaybackHandlerService();

        foreach (var core in existingRoot.Sources)
            SetupMediaPlayerForCore(core, _playbackHandler);

        var pluginLayer = new StrixDataRootPluginWrapper(existingRoot, new PlaybackHandlerPlugin(_playbackHandler), new PopulateEmptyNamesPlugin
        {
            EmptyAlbumName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownAlbum") ?? "?",
            EmptyArtistName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownArtist") ?? "?",
            EmptyDefaultName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownName") ?? "?",
        });

        return pluginLayer;
    }

    private void SetupMediaPlayerForCore(ICore core, IPlaybackHandlerService playbackHandler)
    {
        var mediaPlayer = new MediaPlayer();
        var mediaPlayerElement = new MediaPlayerElement();

        mediaPlayer.CommandManager.IsEnabled = false;
        mediaPlayerElement.SetMediaPlayer(mediaPlayer);
        playbackHandler.RegisterAudioPlayer(new MediaPlayerElementAudioService(mediaPlayerElement), core.InstanceId);

        MediaPlayerElements.Add(mediaPlayerElement);
    }
}
