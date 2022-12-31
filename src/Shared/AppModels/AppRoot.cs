using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Controls;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.Kubo;
using StrixMusic.Plugins;
using StrixMusic.Sdk.Plugins.Model;
using ProgressBar = Windows.UI.Xaml.Controls.ProgressBar;
using ShellSettings = StrixMusic.Settings.ShellSettings;
using ShowType = OwlCore.Extensions.ShowType;

namespace StrixMusic.AppModels;

/// <summary>
/// The root for all data required by the Strix Music App to function.
/// </summary>
public partial class AppRoot : ObservableObject, IAsyncInit
{
    private static readonly SemaphoreSlim _dialogMutex = new(1, 1);
    private static readonly ConcurrentDictionary<string, CancellationTokenSource> _ongoingCoreInitCancellationTokens = new();

    private readonly SemaphoreSlim _initMutex = new(1, 1);
    private readonly IModifiableFolder _dataFolder;
    private readonly PlaybackHandlerService _playbackHandler = new();

    [ObservableProperty]
    private AppDiagnostics? _diagnostics;

    [ObservableProperty]
    private StrixDataRootViewModel? _strixDataRoot;

    [ObservableProperty]
    private MergedCore? _mergedCore;

    [ObservableProperty]
    private MusicSourcesSettings? _musicSourcesSettings;

    [ObservableProperty]
    private ShellSettings? _shellSettings;

    [ObservableProperty]
    private IpfsAccess? _ipfs;

    /// <summary>
    /// Creates a new instance of <see cref="AppRoot"/>.
    /// </summary>
    public AppRoot(IModifiableFolder dataFolder)
    {
        _dataFolder = dataFolder;
    }

    /// <summary>
    /// The message handler to use to handle all HTTP-based requests.
    /// </summary>
    public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();

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

            Logger.LogInformation($"Initializing app root using folder {_dataFolder.Id}");
            cancellationToken.ThrowIfCancellationRequested();

            if (Diagnostics is null)
            {
                Logger.LogInformation($"Initializing {nameof(DebugSettings)}");

                var debugSettingsFolder = await GetOrCreateSettingsFolder(nameof(DebugSettings));
                Diagnostics = new AppDiagnostics(debugSettingsFolder);

                await Diagnostics.Settings.LoadCommand.ExecuteAsync(cancellationToken);
            }

            if (MusicSourcesSettings is null)
            {
                Logger.LogInformation($"Initializing {nameof(MusicSourcesSettings)}");

                var musicSourceSettingsFolder = await GetOrCreateSettingsFolder(nameof(MusicSourcesSettings));
                MusicSourcesSettings = new MusicSourcesSettings(folder: musicSourceSettingsFolder);

                await MusicSourcesSettings.LoadCommand.ExecuteAsync(cancellationToken);
            }

            if (ShellSettings is null)
            {
                Logger.LogInformation($"Initializing {nameof(ShellSettings)}");

                var shellSettingsFolder = await GetOrCreateSettingsFolder(nameof(ShellSettings));
                ShellSettings = new ShellSettings(folder: shellSettingsFolder);

                await ShellSettings.LoadCommand.ExecuteAsync(cancellationToken);
            }

            if (Ipfs is null)
            {
                Logger.LogInformation($"Initializing {nameof(IpfsSettings)}");

                var ipfsSettingsFolder = await GetOrCreateSettingsFolder(nameof(IpfsSettings));
                var ipfsSettings = new IpfsSettings(ipfsSettingsFolder);

                Ipfs = new IpfsAccess(ipfsSettings);

                await ipfsSettings.LoadCommand.ExecuteAsync(cancellationToken);
                if (ipfsSettings.Enabled)
                {
                    Logger.LogInformation($"Initializing {nameof(Ipfs)}");
                    await Ipfs.InitCommand.ExecuteAsync(null);
                }
            }

            // Create/Remove cores when settings are added/removed.
            MusicSourcesSettings.ConfiguredLocalStorageCores.CollectionChanged += ConfiguredLocalStorageCores_OnCollectionChanged;
            MusicSourcesSettings.ConfiguredOneDriveCores.CollectionChanged += ConfiguredOneDriveCores_OnCollectionChanged;

            // Merge cores together and apply plugins
            var allNewCores = await CreateConfiguredCoresAsync().ToListAsync(cancellationToken: cancellationToken);

            // Initialize all cores.
            // Task will not complete until all cores are either loaded, or the user has given up on retrying to load them.
            await allNewCores.InParallel(x => TryInitCore(x, cancellationToken));

            // Prune cores that didn't load successfully
            allNewCores = allNewCores.Where(x => x.IsInitialized).ToList();

            // Even if no new cores need to be created, as settings are changed, _mergedCore can be assigned and sources can be added/removed.
            // If _mergedCore exists, set it up as the data root.
            if (_mergedCore is not null)
            {
                foreach (var newCore in allNewCores)
                    _mergedCore.AddSource(newCore);
            }
            else if (allNewCores.Any())
            {
                Logger.LogInformation($"Creating {nameof(MergedCore)} and adding {allNewCores.Count} new cores.");
                _mergedCore = new MergedCore(allNewCores);
            }

            // If merged core still couldn't be created
            if (_mergedCore is null)
            {
                // It should *only* be because there are no cores to create.
                Guard.HasSizeEqualTo(allNewCores, 0);

                // TODO: Show OOBE if this InitAsync method is called and Initialized == false when completed.
                return;
            }

            var mergedCoreWithPlugins = CreatePluginLayer(_mergedCore, Ipfs);
            StrixDataRoot = new StrixDataRootViewModel(mergedCoreWithPlugins);

            IsInitialized = true;
            Logger.LogInformation($"{nameof(AppRoot)} is initialized and ready to use.");
        }
    }

    private async IAsyncEnumerable<ICore> CreateConfiguredCoresAsync()
    {
        Guard.IsNotNull(MusicSourcesSettings);

        // Create cores that haven't already been set up.
        foreach (var item in MusicSourcesSettings.ConfiguredLocalStorageCores.Where(NeedsToBeCreated).Where(x => x.CanCreateCore))
        {
            Logger.LogInformation($"Creating core {item.InstanceId}");
            var core = await CoreFactory.CreateLocalStorageCoreAsync(item);
            yield return core;
        }

        foreach (var item in MusicSourcesSettings.ConfiguredOneDriveCores.Where(NeedsToBeCreated).Where(x => x.CanCreateCore))
        {
            Logger.LogInformation($"Creating core {item.InstanceId}");
            var core = await CoreFactory.CreateOneDriveCoreAsync(item, HttpMessageHandler);
            yield return core;
        }
    }

    private async void ConfiguredLocalStorageCores_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Guard.IsNotNull(MusicSourcesSettings);

        await HandleCoreSettingsCollectionChangedAsync<LocalStorageCoreSettings>(sender, e, CoreFactory.CreateLocalStorageCoreAsync);
        await MusicSourcesSettings.SaveAsync();
    }

    private async void ConfiguredOneDriveCores_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Guard.IsNotNull(MusicSourcesSettings);

        await HandleCoreSettingsCollectionChangedAsync<OneDriveCoreSettings>(sender, e, async x => await CoreFactory.CreateOneDriveCoreAsync(x, HttpMessageHandler));
        await MusicSourcesSettings.SaveAsync();
    }

    private async Task HandleCoreSettingsCollectionChangedAsync<TSettings>(object sender, NotifyCollectionChangedEventArgs e, Func<TSettings, Task<ICore>> settingsToCoreFactory)
        where TSettings : CoreSettingsBase, IInstanceId
    {
        using (await _initMutex.DisposableWaitAsync())
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var settings in e.NewItems.Cast<TSettings>())
                {
                    if (settings.HasUnsavedChanges)
                        await settings.SaveAsync();
                    else
                        await settings.LoadAsync();

                    if (!settings.CanCreateCore)
                    {
                        Logger.LogInformation($"Core settings for {settings.GetType()} ({nameof(settings.InstanceId)} {settings.InstanceId}) are invalid. Core cannot be created.");
                        return;
                    }

                    var newCore = await settingsToCoreFactory(settings);

                    try
                    {

                        // Init core, present user with retry options on failure.
                        await TryInitCore(newCore, CancellationToken.None);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }

                    // Core didn't init correctly and user chose not to retry.
                    if (!newCore.IsInitialized)
                    {
                        Logger.LogInformation($"Core {newCore.InstanceId} didn't initialize correctly, and the user chose not to retry.");
                        var settingsInstances = (IList<TSettings>)sender;

                        Guard.IsTrue(settingsInstances.Remove(settings));
                        await MusicSourcesSettings!.SaveAsync();

                        return;
                    }

                    Logger.LogInformation($"Core initialized.");

                    // A merged core cannot be created without at least one source.
                    // If _mergedCore doesn't exist yet, this must be the first core being added.
                    // We'll need to create _mergedCore ourselves here, and not overwrite it elsewhere if not null.
                    if (_mergedCore is null)
                    {
                        Logger.LogInformation($"Creating {nameof(MergedCore)} and adding new core {newCore.DisplayName}, {nameof(newCore.InstanceId)} {newCore.InstanceId}");
                        _mergedCore = new MergedCore(newCore.IntoList());

                    }
                    else
                    {
                        Logger.LogInformation($"Adding new core {newCore.DisplayName}, {nameof(newCore.InstanceId)} {newCore.InstanceId}");
                        _mergedCore.AddSource(newCore);
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (_mergedCore is null)
                {
                    // Not yet loaded, nothing created so nothing to remove.
                    return;
                }

                foreach (var item in e.OldItems.Cast<TSettings>())
                {
                    // TSettings is contractually obligated to implement IInstanceId.
                    // If the target is null, it must be because the core hasn't been added to _mergedCore.
                    var target = _mergedCore.Sources.FirstOrDefault(x => x.InstanceId == item.InstanceId);
                    if (target is null)
                        return;

                    if (_ongoingCoreInitCancellationTokens.TryGetValue(target.InstanceId, out var cancellationTokenSource))
                        cancellationTokenSource.Cancel();

                    await target.DisposeAsync();
                    _mergedCore.RemoveSource(target);
                }
            }
        }

        await InitAsync();
    }

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Attempts to safely initialize the provided <paramref name="core"/>, allowing the user to retry in case of failure.
    /// </summary>
    /// <param name="core">The <see cref="ICore"/> instance to attempt to initialize.</param>
    private async Task TryInitCore(ICore core, CancellationToken cancellationToken)
    {
        Logger.LogInformation($"Started init for core {core.DisplayName}, instance id \"{core.InstanceId}\"");

        try
        {
            // Cancel any existing setup requests
            if (_ongoingCoreInitCancellationTokens.TryGetValue(core.InstanceId, out var cancellationTokenSource))
                cancellationTokenSource.Cancel();

            // Create new cancellation source, link with new parent token.
            cancellationTokenSource ??= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;
            _ongoingCoreInitCancellationTokens.TryAdd(core.InstanceId, cancellationTokenSource);

            await core.InitAsync(cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            using (await _dialogMutex.DisposableWaitAsync(cancellationToken))
            {
                await HandleFailureAsync(ex);
            }

            async Task HandleFailureAsync(Exception ex)
            {
                Logger.LogError($"Core failed to initialize: \"{core.DisplayName}\", id {core.InstanceId}.", ex);

                StackPanel CreateCoreDataStackPanel() => new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 20,
                    Children =
                    {
                        new CoreImage { Image = core.Logo, Height = 45, HorizontalAlignment = HorizontalAlignment.Left },
                        new StackPanel
                        {
                            Spacing = 5,
                            Children =
                            {
                                new TextBlock { Text = core.DisplayName, VerticalAlignment = VerticalAlignment.Bottom, FontSize = 16 },
                                new TextBlock { Text = core.InstanceDescriptor, FontSize = 12 },
                            },
                        },
                    },
                };

                var initCoreAsyncCommand = new AsyncRelayCommand(core.InitAsync, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

                // Wait for user to pick an option and close the dialog.
                var retryConfirmationDialog = new ContentDialog
                {
                    Title = $"This failed to load:",
                    Content = new StackPanel
                    {
                        Width = 275,
                        Spacing = 15,
                        Children =
                        {
                            CreateCoreDataStackPanel(),
                            new Expander
                            {
                                Width = 275,
                                Header = "View error",
                                ExpandDirection = ExpandDirection.Down,
                                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                Content = new TextBlock { Text = $"Reason: {ex}", FontSize = 11, IsTextSelectionEnabled = true },
                            },
                        },
                    },
                    CloseButtonText = "Ignore, remove source",
                    PrimaryButtonText = "Try again",
                    PrimaryButtonCommand = initCoreAsyncCommand,
                };

                await retryConfirmationDialog.ShowAsync(ShowType.QueueNext);

                // User chose to ignore and remove source.
                if (initCoreAsyncCommand.ExecutionTask is null)
                    return;

                // User chose to retry
                // Wait for user to pick an option and close the dialog.
                var retryStatusDialog = new ContentDialog
                {
                    Title = $"This failed to load:",
                    Content = new StackPanel
                    {
                        Width = 250,
                        Spacing = 7,
                        Children =
                        {
                            CreateCoreDataStackPanel(),
                            new ProgressBar { IsIndeterminate = true },
                            new TextBlock { Text = $"Initializing music source..." },
                        },
                    },
                    CloseButtonText = "Cancel, remove source",
                    CloseButtonCommand = new RelayCommand(initCoreAsyncCommand.Cancel),
                };

                _ = retryStatusDialog.ShowAsync(ShowType.Interrupt);

                if (initCoreAsyncCommand.ExecutionTask.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.WaitingForChildrenToComplete or TaskStatus.WaitingToRun)
                    await initCoreAsyncCommand.ExecutionTask;

                retryStatusDialog.Hide();

                if (initCoreAsyncCommand.ExecutionTask.Status == TaskStatus.Faulted)
                {
                    await HandleFailureAsync(initCoreAsyncCommand.ExecutionTask.Exception.Flatten());
                }
                else if (initCoreAsyncCommand.ExecutionTask.Status == TaskStatus.RanToCompletion)
                {
                    var successDialog = new ContentDialog
                    {
                        Title = $"Loaded successfully",
                        Content = CreateCoreDataStackPanel(),
                        CloseButtonText = "Ok",
                    };

                    await successDialog.ShowAsync(ShowType.QueueLast);
                }
            }
        }
    }

    private StrixDataRootPluginWrapper CreatePluginLayer(MergedCore existingRoot, IpfsAccess ipfs)
    {
        Logger.LogInformation("Creating plugin layer");

        foreach (var core in existingRoot.Sources)
            SetupMediaPlayerForCore(core);

        var plugins = new List<SdkModelPlugin>
        {
            new PlaybackHandlerPlugin(_playbackHandler),
            new PopulateEmptyNamesPlugin
            {
                EmptyAlbumName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownAlbum") ?? "?",
                EmptyArtistName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownArtist") ?? "?",
                EmptyDefaultName = Sdk.WinUI.Globalization.LocalizationResources.Music?.GetString("UnknownName") ?? "?",
            }
        };

        if (ipfs.Settings.Enabled)
        {
            Guard.IsNotNull(ipfs.ThisPeer);
            Guard.IsNotNull(ipfs.Client);

            if (ipfs.Settings.GlobalPlaybackStateCountPluginEnabled)
            {
                var pw = Environment.GetEnvironmentVariable($"EncryptionKeys.{nameof(GlobalPlaybackStateCountPlugin)}.pw") ?? typeof(AppRoot).AssemblyQualifiedName;
                var salt = Environment.GetEnvironmentVariable($"EncryptionKeys.{nameof(GlobalPlaybackStateCountPlugin)}.salt") ?? ipfs.ThisPeer.Id.ToString();

                var encryptedPubSub = new AesPasswordEncryptedPubSub(ipfs.Client.PubSub, pw, salt);

                plugins.Add(new GlobalPlaybackStateCountPlugin(ipfs.ThisPeer, encryptedPubSub));
            }
        }

        var pluginLayer = new StrixDataRootPluginWrapper(existingRoot, plugins.ToArray());

        return pluginLayer;
    }

    private void SetupMediaPlayerForCore(ICore core)
    {
        var mediaPlayer = new MediaPlayer();
        var mediaPlayerElement = new MediaPlayerElement();

        mediaPlayer.CommandManager.IsEnabled = false;
        mediaPlayerElement.SetMediaPlayer(mediaPlayer);
        _playbackHandler.RegisterAudioPlayer(new MediaPlayerElementAudioService(mediaPlayerElement), core.InstanceId);

        MediaPlayerElements.Add(mediaPlayerElement);
    }

    private async Task<IModifiableFolder> GetOrCreateSettingsFolder(string folderName)
    {
        var folder = await _dataFolder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == folderName) ?? await _dataFolder.CreateFolderAsync(folderName);

        if (folder is not IModifiableFolder modifiableFolder)
            return ThrowHelper.ThrowArgumentException<IModifiableFolder>($"The modifiable folder {_dataFolder.Id} returned a non-modifiable folder. The settings folder for music sources must be modifiable.");

        return modifiableFolder;
    }

    bool NeedsToBeCreated<TSettings>(TSettings settings) where TSettings : SettingsBase, IInstanceId => _mergedCore?.Sources.All(y => settings.InstanceId != y.InstanceId) ?? true;
}
