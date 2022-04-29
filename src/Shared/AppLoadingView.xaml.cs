﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NLog.Config;
using NLog.Targets;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Services;
using StrixMusic.Cores.LocalFiles;
using StrixMusic.Cores.OneDrive;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Helpers;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Plugins.PlaybackHandler;
using StrixMusic.Sdk.Plugins.PopulateEmptyNames;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Models;
using StrixMusic.Sdk.WinUI.Services.Localization;
using StrixMusic.Sdk.WinUI.Services.NotificationService;
using StrixMusic.Sdk.WinUI.Services.ShellManagement;
using StrixMusic.Services;
using StrixMusic.Shells.Default;
using StrixMusic.Shells.Groove;
using StrixMusic.Shells.ZuneDesktop;
using Uno.UI.MSAL;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.Extensions;
using StrixMusic.Controls;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.PluginModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Services.CoreManagement;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The loading view used to initialize the app on startup. The user sees a splash screen, a text status indicator, and icons representing each core.
    /// </summary>
    public sealed partial class AppLoadingView : UserControl
    {
        private MergedCollectionConfig _mergedCollectionConfig = new();
        private bool _showingQuip;
        private INavigationService<Control>? _navService;

        /// <summary>
        /// The cores displayed in the loading UI.
        /// </summary>
        public ObservableCollection<CoreViewModel> Cores { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLoadingView"/> class.
        /// </summary>
        public AppLoadingView()
        {
            this.InitializeComponent();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif
        }

        private void AppLoadingView_OnLoaded(object sender, RoutedEventArgs e)
        {
            PrereleaseNoticeContainer.Visibility = Visibility.Collapsed;
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            var appFrame = Window.Current.GetAppFrame();
            UpdateStatusRaw("> ...");

            Logger.LogInformation("Setting up app settings");
            var settings = await InitAppSettings();
            settings.PropertyChanged += OnSettingChanged;
            _mergedCollectionConfig.MergedCollectionSorting = settings.MergedCollectionSorting;

            if (settings.IsLoggingEnabled)
            {
                Logger.LogInformation("Setting up persistent logging");
                SetupLogger();
            }

            Logger.LogInformation("Setting up localization");
            var localizationService = appFrame.LocalizationService;

            Logger.LogInformation("Enabling quips");
            ShowQuip(localizationService); // TODO: Add debug boot mode / #741

            Logger.LogInformation("Setting up playback and SMTP handlers");
            var playbackHandlerService = SetupPlaybackHandler();
            var smtpHandler = new SystemMediaTransportControlsHandler(playbackHandlerService);

            Logger.LogInformation("Setting up navigation service");
            var navService = new NavigationService<Control>();

            Logger.LogInformation("Initializing core registry");
            UpdateStatus("InitCoreReg", localizationService);
            InitializeCoreRegistry(appFrame.NotificationService);

            Logger.LogInformation("Initializing shell registry");
            await InitializeShellRegistryAsync(appFrame.NotificationService);

            Logger.LogInformation($"Initializing {nameof(FileSystemService)}");
            var fileSystemService = new FileSystemService();
            await fileSystemService.InitAsync();

            Logger.LogInformation($"Initializing {nameof(CoreManagementService)}");
            var cores = new List<ICore>();
            var coreManagementService = new CoreManagementService(settings);
            await coreManagementService.InitAsync();
            coreManagementService.CoreInstanceRegistered += PreOutOfBox_CoreInstanceRegistered;

            Logger.LogInformation("Checking for valid core registry");
            var registry = await coreManagementService.GetCoreInstanceRegistryAsync();
            if (registry.Count == 0)
            {
                Logger.LogInformation("No registered core instances. Displaying OOBE, waiting for at least 1 core to be configured.");
                await WaitForTempOutOfBoxSetupAsync(coreManagementService, appFrame.NotificationService, settings, cores);
            }

            coreManagementService.CoreInstanceRegistered -= PreOutOfBox_CoreInstanceRegistered;
            Guard.IsTrue(settings.CoreInstanceRegistry.Count > 0, nameof(settings.CoreInstanceRegistry));

            Logger.LogInformation($"Initializing core ranking");
            _mergedCollectionConfig.CoreRanking = settings.CoreRanking = await InitializeCoreRankingAsync(coreManagementService);
            Guard.HasSizeGreaterThan(settings.CoreRanking, 0, nameof(settings.CoreRanking));

            Logger.LogInformation("Constructing cores");
            UpdateStatus("CreatingCores", localizationService);
            var registeredCoreInstances = await coreManagementService.GetCoreInstanceRegistryAsync();
            foreach (var entry in registeredCoreInstances)
            {
                var core = await CoreRegistry.CreateCoreAsync(entry.Value.Id, entry.Key);
                core.CoreStateChanged += CoreOnCoreStateChanged;
                cores.Add(core);
                Cores.Add(new CoreViewModel(core));
            }

            Logger.LogInformation("Constructing inbox plugins");
            var playbackHandlerPlugin = new PlaybackHandlerPlugin(playbackHandlerService);
            var emptyNameFallbackPlugin = new PopulateEmptyNamesPlugin
            {
                EmptyAlbumName = localizationService.Music?.GetString("UnknownAlbum") ?? "?",
                EmptyArtistName = localizationService.Music?.GetString("UnknownArtist") ?? "?",
                EmptyDefaultName = localizationService.Music?.GetString("UnknownName") ?? "?",
            };

            Logger.LogInformation("Constructing data layers");
            var mergedLayer = new MergedCore(cores, _mergedCollectionConfig);
            var pluginLayer = new StrixDataRootPluginWrapper(mergedLayer, emptyNameFallbackPlugin, playbackHandlerPlugin);
            var rootViewModel = new StrixDataRootViewModel(pluginLayer);

            Logger.LogInformation("Setting up misc lifetime events");
            Guard.IsNotNull(appFrame.ContentOverlay);
            appFrame.ContentOverlay.Closed += ContentOverlay_Closed;
            coreManagementService.CoreInstanceRegistered += AfterInit_CoreInstanceRegistered;

            UpdateStatus("InitServices", localizationService);
            Logger.LogInformation("Setting up IoC");
            {
                var services = new ServiceCollection();
                services.AddSingleton<INavigationService<Control>>(navService);
                services.AddSingleton(playbackHandlerService);
                services.AddSingleton(smtpHandler);
                services.AddSingleton(rootViewModel);
                services.AddSingleton(localizationService);
                services.AddSingleton(settings);
                services.AddSingleton<IFileSystemService>(fileSystemService);
                services.AddSingleton<INotificationService>(appFrame.NotificationService);
                services.AddSingleton<ICoreManagementService>(coreManagementService);
                Ioc.Default.ConfigureServices(services.BuildServiceProvider());
            }

            Logger.LogInformation("Setting up primary app view");
            var mainPage = new MainPage(rootViewModel);

            Logger.LogInformation("Setting up audio containers");
            foreach (var core in cores)
            {
                if (core.PlaybackType == MediaPlayerType.Standard)
                {
                    var audioPlayer = new AudioPlayerService(mainPage.CreateMediaPlayerElement());
                    playbackHandlerService.RegisterAudioPlayer(audioPlayer, core.InstanceId);
                }
            }

            Logger.LogInformation("Initializing data root and cores");
            UpdateStatus("InitCores", localizationService);
            await rootViewModel.InitAsync();

            Logger.LogInformation("Loading completed, saving settings");
            await settings.SaveAsync();
            UpdateStatusRaw("Done loading");

            StrixIcon strixIcon = PART_StrixIcon;
            strixIcon.FinishAnimation();
            await OwlCore.Flow.EventAsTask(x => strixIcon.AnimationFinished += x, x => strixIcon.AnimationFinished -= x, CancellationToken.None);

            foreach (var core in Cores.ToList())
            {
                await Task.Delay(75);
                Cores.Remove(core);
            }

            appFrame.Present(mainPage);

            foreach (var core in cores)
                core.CoreStateChanged -= CoreOnCoreStateChanged;

            void CoreOnCoreStateChanged(object sender, CoreState e)
            {
                if (e == CoreState.NeedsConfiguration)
                    appFrame.DisplayAbstractUIPanel((ICore)sender, settings, cores, coreManagementService);
            }

            async void ContentOverlay_Closed(object? sender, EventArgs e)
            {
                Guard.IsNotNull(rootViewModel);
                Guard.IsNotNull(coreManagementService);

                foreach (var core in rootViewModel.Sources.ToArray())
                {
                    if (core.CoreState == CoreState.Unloaded)
                    {
                        await coreManagementService.UnregisterCoreInstanceAsync(core.InstanceId);
                    }
                }
            }

            async void PreOutOfBox_CoreInstanceRegistered(object sender, CoreInstanceEventArgs args)
            {
                var core = await CoreRegistry.CreateCoreAsync(args.CoreMetadata.Id, args.InstanceId);
                cores.Add(core);

                await core.InitAsync();

            }

            async void AfterInit_CoreInstanceRegistered(object sender, CoreInstanceEventArgs args)
            {
                var core = await CoreRegistry.CreateCoreAsync(args.CoreMetadata.Id, args.InstanceId);
                mergedLayer.AddSource(core);

                await core.InitAsync();
            }
        }

        private async static Task InitializeShellRegistryAsync(NotificationService notificationService)
        {
            ShellRegistry.ShellRegistered += OnShellRegistered;

            var shellFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shells", Windows.Storage.CreationCollisionOption.OpenIfExists);
            var zuneSettingsStorage = await shellFolder.CreateFolderAsync(ZuneShell.Metadata.Id, Windows.Storage.CreationCollisionOption.OpenIfExists);

            ShellRegistry.Register(DefaultShell.Metadata, () => new DefaultShell());
            ShellRegistry.Register(GrooveShell.Metadata, () => new GrooveShell());
            ShellRegistry.Register(ZuneShell.Metadata, () => new ZuneShell(new FolderData(zuneSettingsStorage), notificationService));

            if (ShellRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(ShellRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 shell.");

            ShellRegistry.ShellRegistered -= OnShellRegistered;
            void OnShellRegistered(object? sender, ShellMetadata metadata) => Logger.LogInformation($"Shell registered. Id {metadata.Id}, Display name {metadata.DisplayName}");
        }

        private static void InitializeCoreRegistry(NotificationService notificationService)
        {
            CoreRegistry.CoreRegistered += OnCoreRegistered;

            CoreRegistry.Register(LocalFilesCore.Metadata, async instanceId =>
            {
                var coreFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cores", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceFolder = await coreFolder.CreateFolderAsync(instanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);

                return new LocalFilesCore(instanceId, new FileSystemService(coreInstanceFolder), new FolderData(coreInstanceFolder), notificationService);
            });

            CoreRegistry.Register(OneDriveCore.Metadata, async instanceId =>
            {
                var coreFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cores", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceFolder = await coreFolder.CreateFolderAsync(instanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceAbstractFolder = new FolderData(coreInstanceFolder);

                var loginMethod = LoginMethod.DeviceCode;
                var messageHandler = new HttpClientHandler();

#if __WASM__
                loginMethod = LoginMethod.Interactive;
                messageHandler = new Uno.UI.Wasm.WasmHttpHandler();
#endif

                var core = new OneDriveCore(instanceId, coreInstanceAbstractFolder, coreInstanceAbstractFolder, notificationService)
                {
                    LoginMethod = loginMethod,
                    HttpMessageHandler = messageHandler,
                };

                // TODO: Detach event when unregistered
                core.LoginNavigationRequested += OnUriNavigationRequested;
                core.MsalAcquireTokenInteractiveParameterBuilderCreated += OnInteractiveParamBuilderCreated;
                core.MsalPublicClientApplicationBuilderCreated += OnPublicClientApplicationBuilderCreated;

                return core;

                void OnInteractiveParamBuilderCreated(object sender, AcquireTokenInteractiveParameterBuilderCreatedEventArgs args) => args.Builder = args.Builder.WithUnoHelpers();
                void OnPublicClientApplicationBuilderCreated(object sender, MsalPublicClientApplicationBuilderCreatedEventArgs args) => args.Builder = args.Builder.WithUnoHelpers();
            });

            if (CoreRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(CoreRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 core.");

            CoreRegistry.CoreRegistered -= OnCoreRegistered;
            void OnCoreRegistered(object? sender, CoreMetadata metadata) => Logger.LogInformation($"Core registered. Id {metadata.Id}, Display name {metadata.DisplayName}");
        }

        private static void OnUriNavigationRequested(object? sender, Uri e) => Windows.System.Launcher.LaunchUriAsync(e);

        private static async Task<List<string>> InitializeCoreRankingAsync(ICoreManagementService coreManager)
        {
#warning Move into core management service.
            var coreInstanceRegistry = await coreManager.GetCoreInstanceRegistryAsync();

            Guard.IsGreaterThan(coreInstanceRegistry.Count, 0, nameof(coreInstanceRegistry.Count));
            var existingCoreRanking = await coreManager.GetCoreInstanceRanking();

            var coreRanking = new List<string>();

            foreach (var instanceId in existingCoreRanking)
            {
                var coreMetadata = coreInstanceRegistry.FirstOrDefault(x => x.Key == instanceId).Value;
                if (coreMetadata is null)
                    continue;

                // If this core is still registered, add it to the ranking.
                if (CoreRegistry.MetadataRegistry.Any(x => x.Id == coreMetadata.Id))
                    coreRanking.Add(instanceId);
            }

            // If no cores exist in ranking, initialize with all loaded cores.
            if (coreRanking.Count == 0)
            {
                Logger.LogInformation($"No existing core rankings found, creating default ranking...");

                // TODO: Show abstractUI and let the user rank the cores manually.
                foreach (var instance in coreInstanceRegistry)
                    coreRanking.Add(instance.Key);
            }

            Guard.IsGreaterThan(coreRanking.Count, 0, nameof(coreRanking.Count));
            Logger.LogInformation($"Core ranking initialized with {coreRanking.Count} instances.");
            return coreRanking;
        }

        private void OnSettingChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var appSettings = sender as AppSettings;
            Guard.IsNotNull(appSettings, nameof(appSettings));

            if (e.PropertyName == nameof(AppSettings.CoreRanking))
                _mergedCollectionConfig.CoreRanking = appSettings.CoreRanking;

            if (e.PropertyName == nameof(AppSettings.MergedCollectionSorting))
                _mergedCollectionConfig.MergedCollectionSorting = appSettings.MergedCollectionSorting;
        }

        private async static Task WaitForTempOutOfBoxSetupAsync(ICoreManagementService coreManagementService, INotificationService notificationService, AppSettings settings, IReadOnlyList<ICore> loadedCores)
        {
            var doneButton = new AbstractButton($"{nameof(AppLoadingView)}.OOBEFinishedButton", "Done", null, AbstractButtonType.Confirm);
            var notification = notificationService.RaiseNotification(new AbstractUICollection($"{nameof(AppLoadingView)}.OOBEElementGroup", PreferredOrientation.Horizontal)
            {
                Title = "First time?",
                Subtitle = "Set up your skins and services before proceeding. A proper OOBE will come later.",
                Items = new List<AbstractUIElement>()
                {
                    doneButton,
                },
            });

            // TODO Need a real OOBE instead of using SuperShell
            Window.Current.GetAppFrame().DisplaySuperShell(settings, loadedCores, coreManagementService);

            // TODO Temp, not great. Need a proper flow here.
            var setupFinishedSemaphore = new SemaphoreSlim(0, 1);

            notification.Dismissed += OnNotificationDismissed;
            doneButton.Clicked += OnDoneButtonClicked;

            async void OnNotificationDismissed(object? sender, EventArgs e)
            {
                Logger.LogInformation($"{nameof(WaitForTempOutOfBoxSetupAsync)}: {nameof(OnNotificationDismissed)}");

                var registry = await coreManagementService.GetCoreInstanceRegistryAsync();
                if (registry.Count == 0)
                    await WaitForTempOutOfBoxSetupAsync(coreManagementService, notificationService, settings, loadedCores);

                setupFinishedSemaphore.Release();
            }

            async void OnDoneButtonClicked(object? sender, EventArgs e)
            {
                Logger.LogInformation($"{nameof(WaitForTempOutOfBoxSetupAsync)}: {nameof(OnDoneButtonClicked)}");

                var registry = await coreManagementService.GetCoreInstanceRegistryAsync();
                if (registry.Count > 0)
                    setupFinishedSemaphore.Release();
            }

            await setupFinishedSemaphore.WaitAsync();
            Logger.LogInformation("OOBE completed");

            notification.Dismissed -= OnNotificationDismissed;
            doneButton.Clicked -= OnDoneButtonClicked;
            notification.Dismiss();
        }

        private void ShowQuip(LocalizationResourceLoader localizationService)
        {
            var quip = new QuipLoader(CultureInfo.CurrentCulture.TwoLetterISOLanguageName).GetGroupIndexQuip();

            PART_Status.Text = localizationService.Quips?.GetString($"{quip.Item1}{quip.Item2}") ?? string.Empty;
            _showingQuip = true;
        }

        private void UpdateStatus(string text, LocalizationResourceLoader localizationService)
        {
            if (_showingQuip)
                return;

            if (localizationService != null)
                text = localizationService?.Startup?.GetString(text) ?? string.Empty;

            PART_Status.Text = text;
        }

        private void UpdateStatusRaw(string text)
        {
            PART_Status.Text = text;
        }

        private static IPlaybackHandlerService SetupPlaybackHandler()
        {
            var playbackHandlerService = new PlaybackHandlerService();

            var strixDevice = new StrixDevice(playbackHandlerService);
            playbackHandlerService.SetStrixDevice(strixDevice);

#warning TODO: Connect with remote devices from cores.
            playbackHandlerService.ActiveDevice = strixDevice;

            return playbackHandlerService;
        }

        private async static Task<AppSettings> InitAppSettings()
        {
            var settingsDirectory = await ApplicationData.Current.LocalFolder.CreateFolderAsync(nameof(AppSettings), Windows.Storage.CreationCollisionOption.OpenIfExists);
            var settings = new AppSettings(new FolderData(settingsDirectory));

            await settings.LoadAsync();
            return settings;
        }

        private static void SetupLogger()
        {
            var logPath = ApplicationData.Current.LocalCacheFolder.Path + @"\Logs\${date:format=yyyy-MM-dd}.log";

            NLog.LogManager.Configuration = CreateConfig(shouldArchive: true);

            // Event is connected for the lifetime of the application
            Logger.MessageReceived += Logger_MessageReceived;

            Logger.LogInformation("Logger initialized");

            LoggingConfiguration CreateConfig(bool shouldArchive)
            {
                var config = new LoggingConfiguration();

                var fileTarget = new FileTarget("filelog")
                {
                    FileName = logPath,
                    EnableArchiveFileCompression = shouldArchive,
                    MaxArchiveDays = 7,
                    ArchiveNumbering = ArchiveNumberingMode.Sequence,
                    ArchiveOldFileOnStartup = shouldArchive,
                    KeepFileOpen = true,
                    OpenFileCacheTimeout = 10,
                    AutoFlush = false,
                    OpenFileFlushTimeout = 10,
                    ConcurrentWrites = false,
                    CleanupFileName = false,
                    OptimizeBufferReuse = true,
                    Layout = "${message}",
                };

                config.AddTarget(fileTarget);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, "filelog");

                var debuggerTarget = new DebuggerTarget("debuggerTarget")
                {
                    OptimizeBufferReuse = true,
                    Layout = "${message}",
                };

                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, debuggerTarget);
                config.AddTarget(debuggerTarget);

                return config;
            }
        }

        private static void Logger_MessageReceived(object sender, LoggerMessageEventArgs e)
        {
            var message = $"{DateTime.UtcNow:O} [{e.Level}] [Thread {Thread.CurrentThread.ManagedThreadId}] L{e.CallerLineNumber} {System.IO.Path.GetFileName(e.CallerFilePath)} {e.CallerMemberName} {(e.Exception is not null ? $"Exception: {e.Exception} |" : string.Empty)} {e.Message}";

            NLog.LogManager.GetLogger(string.Empty).Log(NLog.LogLevel.Info, message);
            Console.WriteLine(message);
        }
    }
}
