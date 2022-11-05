using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.PluginModels;
using StrixMusic.Sdk.Plugins.PlaybackHandler;
using StrixMusic.Sdk.Plugins.PopulateEmptyNames;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Services;
using Windows.UI.Xaml.Controls;
using OwlCore.Diagnostics;
using OwlCore.Storage.Uwp;
using StrixMusic.Cores.Storage;
using Windows.Media.Core;

namespace StrixMusic.AppModels;

/// <summary>
/// The root for all data required by the Strix Music App to function.
/// </summary>
[ObservableObject]
public partial class AppRoot : IAsyncInit
{
    private readonly SemaphoreSlim _initMutex = new(1, 1);
    private readonly IModifiableFolder _dataFolder;

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
        Settings = new AppSettings(dataFolder);

        SetupAvailableCore(new AvailableCore("Local Storage", "Listen to music on your local disk.", defaultSettingsFactory: async () => new LocalStorageCoreSettings(await GetDataFolderByName($"{Guid.NewGuid()}"))));
        SetupAvailableCore(new AvailableCore("OneDrive", "Stream music directly from OneDrive.", defaultSettingsFactory: async () => new OneDriveCoreSettings(await GetDataFolderByName($"{Guid.NewGuid()}"))));

        void SetupAvailableCore(AvailableCore availableCore)
        {
            AvailableCores.Add(availableCore);

            availableCore.CreateCoreRequested += (_, e) =>
            {
                if (e is LocalStorageCoreSettings localStorageSettings)
                    Settings.ConfiguredLocalStorageCores.Add(localStorageSettings);
            };
        }
    }

    /// <summary>
    /// A container for all settings needed throughout the main app.
    /// </summary>
    public AppSettings Settings { get; set; }

    /// <summary>
    /// The cores that are available to be created.
    /// </summary>
    public ObservableCollection<AvailableCore> AvailableCores { get; } = new();

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

            var coreFactory = new CoreFactory(_dataFolder);
            await Settings.LoadAsync(cancellationToken);

            // Create local storage cores
            var localStorageCores = await Settings.ConfiguredLocalStorageCores.InParallel(coreFactory.CreateLocalStorageCoreAsync);

            // Create OneDrive cores
            var oneDriveCores = await Settings.ConfiguredOneDriveCores.InParallel(coreFactory.CreateOneDriveCoreAsync);

            // Merge cores together and apply plugins
            var allCores = localStorageCores
                .Concat(oneDriveCores)
                .ToList<ICore>();

            await TemporaryCoreSetupAsync(allCores);

            if (!allCores.Any())
                return;

            _mergedCore = new MergedCore(allCores);
            var mergedCoreWithPlugins = CreatePluginLayer(_mergedCore);
            StrixDataRoot = new StrixDataRootViewModel(mergedCoreWithPlugins);

            await StrixDataRoot.InitAsync(cancellationToken);

            // Create/Remove cores when settings are added/removed.
            Settings.ConfiguredLocalStorageCores.CollectionChanged += ConfiguredLocalStorageCores_OnCollectionChanged;

            IsInitialized = true;
        }
    }

    private async Task TemporaryCoreSetupAsync(List<ICore> allCores)
    {
        var folder = await PickFolderAsync();
        if (folder is null)
            return;

        var storageCore = await CreateStorageCoreAsync(folder);

        allCores.Add(storageCore);

        async Task<WindowsStorageFolder?> PickFolderAsync()
        {
            var picker = new FolderPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.MusicLibrary,
                FileTypeFilter = { "*" },
            };

            var pickedFolder = await picker.PickSingleFolderAsync();

            return pickedFolder is null ? null : new WindowsStorageFolder(pickedFolder);
        }

        async Task<StorageCore> CreateStorageCoreAsync(IFolder folderToScan)
        {
            var settingsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderToScan.Id.HashMD5Fast(), CreationCollisionOption.OpenIfExists);

            return new StorageCore
            (
                folderToScan,
                metadataCacheFolder: new WindowsStorageFolder(settingsFolder),
                displayName: folderToScan.GetType().Name,
                fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Files Scanned: {x.FilesProcessed}"))
            )
            {
                ScannerWaitBehavior = ScannerWaitBehavior.NeverWait,
            };
        }
    }

    private void ConfiguredLocalStorageCores_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _ = HandleCoreSettingsCollectionChanged<LocalStorageCoreSettings>(e, async x => await new CoreFactory(_dataFolder).CreateLocalStorageCoreAsync(x));
    }

    private async Task HandleCoreSettingsCollectionChanged<TSettings>(NotifyCollectionChangedEventArgs e, Func<TSettings, Task<ICore>> settingsToCoreFactory)
        where TSettings : SettingsBase, IInstanceId
    {
        if (!IsInitialized)
        {
            await InitAsync();
            return;
        }

        Guard.IsNotNull(_mergedCore);

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems.Cast<TSettings>())
            {
                var newCore = await settingsToCoreFactory(item);
                _mergedCore.AddSource(newCore);
                await newCore.InitAsync();
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
        var playbackHandler = new PlaybackHandlerService();
        foreach (var core in existingRoot.Sources)
        {
            var mediaPlayer = new MediaPlayer();
            var mediaPlayerElement = new MediaPlayerElement();
            
            mediaPlayer.CommandManager.IsEnabled = false;
            mediaPlayerElement.SetMediaPlayer(mediaPlayer);
            playbackHandler.RegisterAudioPlayer(new MediaPlayerElementAudioService(mediaPlayerElement), core.InstanceId);

            MediaPlayerElements.Add(mediaPlayerElement);
        }

        var pluginLayer = new StrixDataRootPluginWrapper(existingRoot, new PlaybackHandlerPlugin(playbackHandler), new PopulateEmptyNamesPlugin
        {
            EmptyAlbumName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownAlbum") ?? "?",
            EmptyArtistName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownArtist") ?? "?",
            EmptyDefaultName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownName") ?? "?",
        });

        return pluginLayer;
    }

    private async Task<IModifiableFolder> GetDataFolderByName(string name)
    {
        var folder = await _dataFolder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == name) ??
                     await _dataFolder.CreateFolderAsync(name);

        if (folder is not IModifiableFolder modifiableData)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        return modifiableData;
    }
}
