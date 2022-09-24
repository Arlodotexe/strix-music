using System;
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
using OwlCore.AbstractUI.Models;
using OwlCore.Diagnostics;
using OwlCore.Storage.SystemIO;
using StrixMusic.Controls;
using StrixMusic.CoreModels;
using StrixMusic.Helpers;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.PluginModels;
using StrixMusic.Sdk.Plugins.ImageResizer;
using StrixMusic.Sdk.Plugins.PlaybackHandler;
using StrixMusic.Sdk.Plugins.PopulateEmptyNames;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Services.Localization;
using StrixMusic.Sdk.WinUI.Services.NotificationService;
using StrixMusic.Sdk.WinUI.Services.ShellManagement;
using StrixMusic.Services;
using StrixMusic.Services.CoreManagement;
using StrixMusic.Shells.Default;
using StrixMusic.Shells.Groove;
using StrixMusic.Shells.ZuneDesktop;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic
{
    /// <summary>
    /// The loading view used to initialize the app on startup. The user sees a splash screen, a text status indicator, and icons representing each core.
    /// </summary>
    public sealed partial class AppLoadingView : UserControl
    {
        private MergedCollectionConfig _mergedCollectionConfig = new();
        private bool _showingQuip;

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
            ShowQuip(localizationService); // TODO: Add debug boot mode

            Logger.LogInformation("Setting up playback and SMTP handlers");

            var playbackHandlerService = new PlaybackHandlerService();
            var smtpHandler = new SystemMediaTransportControlsHandler(playbackHandlerService);

            Logger.LogInformation("Setting up navigation service");
            var navService = new NavigationService<Control>();

            Logger.LogInformation("Initializing core registry");
            UpdateStatus("InitCoreReg", localizationService);
            InitializeCoreRegistry(appFrame.NotificationService);

            Logger.LogInformation("Initializing shell registry");
            await InitializeShellRegistryAsync(appFrame.NotificationService);

            Logger.LogInformation($"Initializing {nameof(CoreManagementService)}");
            var cores = new List<ICore>();
            var coreManagementService = new CoreManagementService(settings);
            await coreManagementService.InitAsync();
            coreManagementService.CoreInstanceRegistered += OutOfBox_CoreInstanceRegistered;

            Logger.LogInformation("Checking for valid core registry");
            var registry = await coreManagementService.GetCoreInstanceRegistryAsync();
            if (registry.Count == 0)
            {
                Logger.LogInformation("No registered core instances. Displaying OOBE, waiting for at least 1 core to be configured.");
                await WaitForTempOutOfBoxSetupAsync(coreManagementService, appFrame.NotificationService, settings, cores);
            }

            coreManagementService.CoreInstanceRegistered -= OutOfBox_CoreInstanceRegistered;
            Guard.IsTrue(settings.CoreInstanceRegistry.Count > 0, nameof(settings.CoreInstanceRegistry));

            Logger.LogInformation($"Initializing core ranking");
            _mergedCollectionConfig.CoreRanking = settings.CoreRanking = await InitializeCoreRankingAsync(coreManagementService);
            Guard.HasSizeGreaterThan(settings.CoreRanking, 0, nameof(settings.CoreRanking));

            Logger.LogInformation("Constructing cores");
            UpdateStatus("CreatingCores", localizationService);
            var registeredCoreInstances = await coreManagementService.GetCoreInstanceRegistryAsync();
            foreach (var entry in registeredCoreInstances)
            {
                if (cores.FirstOrDefault(x => x.InstanceId == entry.Key) is not ICore core)
                {
                    Logger.LogInformation($"Creating core with instanceId {entry.Key} id {entry.Value.Id}");
                    core = await CoreRegistry.CreateCoreAsync(entry.Value.Id, entry.Key);
                    cores.Add(core);
                }

                core.CoreStateChanged += RegisteredCore_OnStateChanged;
                Cores.Add(new CoreViewModel(core));
            }

            Logger.LogInformation("Constructing inbox plugins");
            var playbackHandlerPlugin = new PlaybackHandlerPlugin(playbackHandlerService);
            var imageResizerPlugin = new ImageResizerPlugin(originalSize =>
            {
                const int maxSize = 1800;

                // If the actual size isn't known
                if (originalSize.Height is null || originalSize.Width is null)
                    return (Width: maxSize, Height: maxSize);

                // If the image is too wide OR too tall.
                if (originalSize.Height > maxSize || originalSize.Width > maxSize)
                    return (Width: maxSize, Height: maxSize);

                return originalSize;
            });

            var emptyNameFallbackPlugin = new PopulateEmptyNamesPlugin
            {
                EmptyAlbumName = localizationService.Music?.GetString("UnknownAlbum") ?? "?",
                EmptyArtistName = localizationService.Music?.GetString("UnknownArtist") ?? "?",
                EmptyDefaultName = localizationService.Music?.GetString("UnknownName") ?? "?",
            };

            Logger.LogInformation("Constructing data layers");
            var mergedLayer = new MergedCore(cores, _mergedCollectionConfig);
            var pluginLayer = new StrixDataRootPluginWrapper(mergedLayer, emptyNameFallbackPlugin, playbackHandlerPlugin, imageResizerPlugin);
            var rootViewModel = new StrixDataRootViewModel(pluginLayer);

            Logger.LogInformation("Setting up primary app view");
            var mainPage = new MainPage(rootViewModel, settings, coreManagementService);

            Logger.LogInformation("Setting up misc lifetime events");
            Guard.IsNotNull(appFrame.ContentOverlay);
            appFrame.ContentOverlay.Closed += ContentOverlay_Closed;
            coreManagementService.CoreInstanceRegistered += Lifetime_CoreInstanceRegistered;
            coreManagementService.CoreInstanceUnregistered += OnCoreInstanceUnregistered;

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
                services.AddSingleton<INotificationService>(appFrame.NotificationService);
                services.AddSingleton<ICoreManagementService>(coreManagementService);
                Ioc.Default.ConfigureServices(services.BuildServiceProvider());
            }

            Logger.LogInformation("Initializing data root and cores");
            UpdateStatus("InitCores", localizationService);
            await rootViewModel.InitAsync();

            Logger.LogInformation("Setting up audio containers");
            foreach (var core in cores)
            {
                if (core.PlaybackType == MediaPlayerType.Standard)
                {
#if __WASM__
                    continue;
#endif
                    var audioPlayer = new AudioPlayerService(mainPage.CreateMediaPlayerElement());
                    playbackHandlerService.RegisterAudioPlayer(audioPlayer, core.InstanceId);
                }
            }

            Logger.LogInformation("Loading completed, saving settings");
            await settings.SaveAsync();
            UpdateStatusRaw("Done loading");

            StrixIcon strixIcon = PART_StrixIcon;
            strixIcon.FinishAnimation();
            await OwlCore.Flow.EventAsTask(x => strixIcon.AnimationFinished += x, x => strixIcon.AnimationFinished -= x, CancellationToken.None);

            foreach (var core in Cores.ToList())
            {
                Cores.Remove(core);
                await Task.Delay(75);
            }

            appFrame.Present(mainPage);

            void RegisteredCore_OnStateChanged(object? sender, CoreState e)
            {
                var core = sender as ICore;
                Guard.IsNotNull(core);

                if (e == CoreState.NeedsConfiguration)
                    appFrame.DisplayAbstractUIPanel(core, settings, cores, coreManagementService);
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

            async void OutOfBox_CoreInstanceRegistered(object? sender, CoreInstanceEventArgs args)
            {
                var core = await CoreRegistry.CreateCoreAsync(args.CoreMetadata.Id, args.InstanceId);
                cores.Add(core);

                core.CoreStateChanged += Core_CoreStateChanged;

                try
                {
                    await core.InitAsync();
                }
                catch (OperationCanceledException)
                {
                    // The core initialization was cancelled.
                    // Unregister from the core service, everything should update from there.
                    await coreManagementService.UnregisterCoreInstanceAsync(args.InstanceId);
                }

                core.CoreStateChanged -= Core_CoreStateChanged;

                void Core_CoreStateChanged(object? sender, CoreState e)
                {
                    if (e == CoreState.NeedsConfiguration)
                        appFrame.DisplayAbstractUIPanel(core, settings, cores, coreManagementService);
                }
            }

            async void Lifetime_CoreInstanceRegistered(object? sender, CoreInstanceEventArgs args)
            {
                var core = await CoreRegistry.CreateCoreAsync(args.CoreMetadata.Id, args.InstanceId);
                core.CoreStateChanged += RegisteredCore_OnStateChanged;

                mergedLayer.AddSource(core);

                cores.Add(core);
                Cores.Add(new CoreViewModel(core));

                try
                {
                    await core.InitAsync();
                }
                catch (OperationCanceledException)
                {
                    // The core initialization was cancelled.
                    // Unregister from the core service, everything should update from there.
                    await coreManagementService.UnregisterCoreInstanceAsync(args.InstanceId);
                    return;
                }

                if (core.PlaybackType == MediaPlayerType.Standard)
                {
                    var audioPlayer = new AudioPlayerService(mainPage.CreateMediaPlayerElement());
                    playbackHandlerService.RegisterAudioPlayer(audioPlayer, core.InstanceId);
                }
            }

            void Core_CoreStateChanged(object? sender, CoreState e)
            {
                var core = sender as ICore;
                Guard.IsNotNull(core);

                if (e == CoreState.NeedsConfiguration)
                    appFrame.DisplayAbstractUIPanel(core, settings, rootViewModel.Sources, coreManagementService);
            }

            void OnCoreInstanceUnregistered(object? sender, CoreInstanceEventArgs e)
            {
                var sourceToRemove = mergedLayer.Sources.First(x => x.InstanceId == e.InstanceId);
                sourceToRemove.CoreStateChanged -= Core_CoreStateChanged;
                mergedLayer.RemoveSource(sourceToRemove);
                cores.Remove(sourceToRemove);
            }
        }

        private static async Task InitializeShellRegistryAsync(NotificationService notificationService)
        {
            ShellRegistry.ShellRegistered += OnShellRegistered;

            var shellFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shells", Windows.Storage.CreationCollisionOption.OpenIfExists);
            var zuneSettingsStorage = await shellFolder.CreateFolderAsync(ZuneShell.Metadata.Id, Windows.Storage.CreationCollisionOption.OpenIfExists);

            ShellRegistry.Register(DefaultShell.Metadata, dataRoot => new DefaultShell(dataRoot));
            ShellRegistry.Register(GrooveShell.Metadata, dataRoot => new GrooveShell(dataRoot));
            ShellRegistry.Register(ZuneShell.Metadata, dataRoot => new ZuneShell(dataRoot, new FolderData(zuneSettingsStorage), notificationService));

            if (ShellRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(ShellRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 shell.");

            ShellRegistry.ShellRegistered -= OnShellRegistered;
            void OnShellRegistered(object? sender, ShellMetadata metadata) => Logger.LogInformation($"Shell registered. Id {metadata.Id}, Display name {metadata.DisplayName}");
        }

        private void InitializeCoreRegistry(NotificationService notificationService)
        {
            CoreRegistry.CoreRegistered += OnCoreRegistered;

            CoreRegistry.Register(new CoreMetadata(nameof(LocalStorageCore), "Local Files") { Logo = new Cores.LocalFiles.LogoImage(null!) }, async instanceId =>
            {
                var coreFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cores", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceFolder = await coreFolder.CreateFolderAsync(instanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);

                return new LocalStorageCore(instanceId, new FileSystemService(coreInstanceFolder), new FolderData(coreInstanceFolder), notificationService, new(x => StorageCoreProgressChanged(instanceId, x)))
                {
                    ScannerWaitBehavior = StrixMusic.Cores.Storage.ScannerWaitBehavior.NeverWait,
                };
            });

            CoreRegistry.Register(new CoreMetadata("OneDriveCore", "OneDrive") { Logo = new CoreFileImage(new SystemFile("")) }, async instanceId =>
            {
                var coreFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cores", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceFolder = await coreFolder.CreateFolderAsync(instanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceAbstractFolder = new FolderData(coreInstanceFolder);

                var loginMethod = LoginMethod.DeviceCode;
                HttpMessageHandler messageHandler = new HttpClientHandler();
                var settings = new OneDriveCoreSettings(coreInstanceAbstractFolder);
                await settings.LoadAsync();

                if (string.IsNullOrWhiteSpace(settings.ClientId))
                    settings.ClientId = Secrets.OneDriveClientId;

                if (string.IsNullOrWhiteSpace(settings.TenantId))
                    settings.TenantId = Secrets.OneDriveTenantId;

                if (string.IsNullOrWhiteSpace(settings.RedirectUri))
                    settings.RedirectUri = Secrets.OneDriveRedirectUri;

                await settings.SaveAsync();

#if __WASM__
                loginMethod = LoginMethod.Interactive;
                messageHandler = new Uno.UI.Wasm.WasmHttpHandler();
#endif

                var core = new OneDriveCore(instanceId, settings, coreInstanceAbstractFolder, notificationService, new(x => StorageCoreProgressChanged(instanceId, x)))
                {
                    LoginMethod = loginMethod,
                    HttpMessageHandler = messageHandler,
                    ScannerWaitBehavior = StrixMusic.Cores.Storage.ScannerWaitBehavior.NeverWait,
                };

                // TODO: Detach event when unregistered
                core.LoginNavigationRequested += OnUriNavigationRequested;
                core.MsalAcquireTokenInteractiveParameterBuilderCreated += OnInteractiveParamBuilderCreated;
                core.MsalPublicClientApplicationBuilderCreated += OnPublicClientApplicationBuilderCreated;

                return core;

                void OnInteractiveParamBuilderCreated(object? sender, AcquireTokenInteractiveParameterBuilderCreatedEventArgs args) => args.Builder = args.Builder.WithUnoHelpers();
                void OnPublicClientApplicationBuilderCreated(object? sender, MsalPublicClientApplicationBuilderCreatedEventArgs args) => args.Builder = args.Builder.WithUnoHelpers();
            });

            if (CoreRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(CoreRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 core.");

            CoreRegistry.CoreRegistered -= OnCoreRegistered;
            void OnCoreRegistered(object? sender, CoreMetadata metadata) => Logger.LogInformation($"Core registered. Id {metadata.Id}");
        }

        private static void OnUriNavigationRequested(object? sender, Uri e) => Windows.System.Launcher.LaunchUriAsync(e);

        private static async Task<List<string>> InitializeCoreRankingAsync(ICoreManagementService coreManager)
        {
#warning TODO: Move into core management service.
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

        private static async Task WaitForTempOutOfBoxSetupAsync(ICoreManagementService coreManagementService, INotificationService notificationService, AppSettings settings, IReadOnlyList<ICore> loadedCores)
        {
            var doneButton = new AbstractButton($"{nameof(AppLoadingView)}.OOBEFinishedButton", "Done", null, AbstractButtonType.Confirm);
            var tempOOBEContinuationUI = new AbstractUICollection($"{nameof(AppLoadingView)}.OOBEElementGroup", PreferredOrientation.Horizontal)
            {
                Title = "First time?",
                Subtitle = "Set up your skins and services before proceeding. A proper OOBE will come later.",
            };

            tempOOBEContinuationUI.Add(doneButton);

            var notification = notificationService.RaiseNotification(tempOOBEContinuationUI);

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

        private static async Task<AppSettings> InitAppSettings()
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

        private static void Logger_MessageReceived(object? sender, LoggerMessageEventArgs e)
        {
            var message = $"{DateTime.UtcNow:O} [{e.Level}] [Thread {Thread.CurrentThread.ManagedThreadId}] L{e.CallerLineNumber} {System.IO.Path.GetFileName(e.CallerFilePath)} {e.CallerMemberName} {(e.Exception is not null ? $"Exception: {e.Exception} |" : string.Empty)} {e.Message}";

            NLog.LogManager.GetLogger(string.Empty).Log(NLog.LogLevel.Info, message);
            Console.WriteLine(message);
        }
    }
}
