using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Provisos;
using StrixMusic.Cores.LocalFiles;
using StrixMusic.Cores.OneDrive;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.CoreManagement;
using StrixMusic.Sdk.Helpers;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Messages;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Plugins.PlaybackHandler;
using StrixMusic.Sdk.Plugins.PopulateEmptyNames;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using StrixMusic.Services;
using StrixMusic.Shared.Services;
using StrixMusic.Shells.Default;
using StrixMusic.Shells.Groove;
using StrixMusic.Shells.ZuneDesktop;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The loading view used to initialize the app on startup. The user sees a splash screen, a text status indicator, and icons representing each core.
    /// </summary>
    public sealed partial class AppLoadingView : UserControl
    {
        private static List<Assembly> _assemblyLinker = new List<Assembly>();

        private bool _showingQuip;
        private AppSettings? _settings;
        private LocalizationResourceLoader? _localizationService;
        private PlaybackHandlerService? _playbackHandlerService;
        private NotificationService? _notificationService;
#if NETFX_CORE
        private SystemMediaTransportControlsHandler? _smtpHandler;
#endif
        private MainPage? _mainPage;
        private INavigationService<Control>? _navService;
        private ILogger? _logger;

        /// <summary>
        /// The ViewModel for this page.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLoadingView"/> class.
        /// </summary>
        public AppLoadingView()
        {
            this.InitializeComponent();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif
        }

        /// <summary>
        /// Keeps the logger type available in the <paramref name="services"/> but without any output functionality.
        /// </summary>
        private static ILogger AddLoggerBlackHole(IServiceCollection services)
        {
            var tempServices = new ServiceCollection();

            Setup(services);
            Setup(tempServices);

            var provider = tempServices.BuildServiceProvider();
            return provider.GetRequiredService<ILogger<AppLoadingView>>();

            void Setup(IServiceCollection services)
            {
                services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
                services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            }
        }

        private static ILogger AddNLog(IServiceCollection services)
        {
            var logPath = ApplicationData.Current.LocalCacheFolder.Path + @"\Logs\${date:format=yyyy-MM-dd}.log";
            var defaultLayoutStr = @"${date} [Thread ${threadname:whenEmpty=${threadid}}] ${logger} [${level}] | ${message} ${exception:format=ToString}";

            // This temp logger will be used first, so it needs to archive logs from the previous run.
            var tempConfig = CreateConfig(shouldArchive: true);

            // This logger will run after the temp logger, so it shouldn't archive logs or we won't get one continuous log.
            var primaryConfig = CreateConfig(shouldArchive: false);

            SetupLogger(services, primaryConfig);

            var tempServices = new ServiceCollection();
            SetupLogger(tempServices, tempConfig);

            var provider = tempServices.BuildServiceProvider();
            return provider.GetRequiredService<ILogger<AppLoadingView>>();

            LoggingConfiguration CreateConfig(bool shouldArchive)
            {
                var config = new LoggingConfiguration();

                var fileTarget = new FileTarget("filelog")
                {
                    FileName = logPath,
                    Layout = defaultLayoutStr,
                    EnableArchiveFileCompression = shouldArchive,
                    MaxArchiveDays = 7,
                    ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                    ArchiveOldFileOnStartup = shouldArchive,
                    KeepFileOpen = true,
                    OpenFileCacheTimeout = 10,
                    AutoFlush = false,
                    OpenFileFlushTimeout = 10,
                    ConcurrentWrites = false,
                    CleanupFileName = false,
                    OptimizeBufferReuse = true,
                };

                config.AddTarget(fileTarget);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, "filelog");

                var debuggerTarget = new DebuggerTarget("debuggerTarget")
                {
                    Layout = defaultLayoutStr,
                    OptimizeBufferReuse = true,
                };

                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, debuggerTarget);
                config.AddTarget(debuggerTarget);

                var customTarget = new MethodCallTarget("customTarget", (logInfo, parameters) =>
                {
                    var formattedMessage = $"{DateTime.UtcNow} [Thread {Thread.CurrentThread.ManagedThreadId}] {logInfo.LoggerName} [{logInfo.Level}] | {logInfo.Message}";
                    Console.WriteLine(formattedMessage);

                    WeakReferenceMessenger.Default.Send(new LogMessage(formattedMessage, (Microsoft.Extensions.Logging.LogLevel)logInfo.Level.Ordinal, logInfo.StackTrace));
                });

                config.AddTarget(customTarget);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, customTarget);

                return config;
            }

            void SetupLogger(IServiceCollection services, LoggingConfiguration config)
            {
                services.AddLogging(loggingBuilder =>
                {
                    // configure Logging exclusively with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
                });
            }
        }

        private async void AppLoadingView_OnLoaded(object sender, RoutedEventArgs e)
        {
            PrereleaseNoticeContainer.Visibility = Visibility.Collapsed;

            await InitializeServicesAsync();
            InitializeCoreRegistry();
            await InitializeShellRegistryAsync();
            await InitializeOutOfBoxSetupIfNeededAsync();
            await InitializeCoreRankingAsync();
            await InitializeConfiguredCoresAsync();

            Guard.IsNotNull(_logger, nameof(_logger));

            _logger?.LogInformation($"{nameof(AppLoadingView_OnLoaded)} completed, navigating to shell.");
            UpdateStatusRaw($"Done loading");

            StrixIcon.FinishAnimation();
        }

        private async Task InitializeShellRegistryAsync()
        {
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"Initializing shell registry");

            ShellRegistry.ShellRegistered += OnShellRegistered;

            var shellFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shells", Windows.Storage.CreationCollisionOption.OpenIfExists);
            var zuneSettingsStorage = await shellFolder.CreateFolderAsync(ZuneShell.Metadata.Id, Windows.Storage.CreationCollisionOption.OpenIfExists);

            ShellRegistry.Register(DefaultShell.Metadata, () => new DefaultShell());
            ShellRegistry.Register(GrooveShell.Metadata, () => new GrooveShell());
            ShellRegistry.Register(ZuneShell.Metadata, () => new ZuneShell(new FolderData(zuneSettingsStorage)));

            if (ShellRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(ShellRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 shell.");

            ShellRegistry.ShellRegistered -= OnShellRegistered;

            void OnShellRegistered(object? sender, ShellMetadata metadata)
            {
                _logger?.LogInformation($"Shell registered. Id {metadata.Id}, Display name {metadata.DisplayName}");
            }
        }

        private void InitializeCoreRegistry()
        {
            _logger?.LogInformation($"Initializing core registry");
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            CoreRegistry.CoreRegistered += OnCoreRegistered;

            CoreRegistry.Register(LocalFilesCore.Metadata, async instanceId =>
            {
                var coreFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cores", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceFolder = await coreFolder.CreateFolderAsync(instanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);

                return new LocalFilesCore(instanceId, new FileSystemService(coreInstanceFolder), new FolderData(coreInstanceFolder), _notificationService);
            });

            CoreRegistry.Register(OneDriveCore.Metadata, async instanceId =>
            {
                var coreFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cores", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceFolder = await coreFolder.CreateFolderAsync(instanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);
                var coreInstanceAbstractFolder = new FolderData(coreInstanceFolder);

                return new OneDriveCore(instanceId, coreInstanceAbstractFolder, coreInstanceAbstractFolder, _notificationService, new Launcher());
            });

            if (CoreRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(CoreRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 core.");

            CoreRegistry.CoreRegistered -= OnCoreRegistered;

            void OnCoreRegistered(object? sender, CoreMetadata metadata)
            {
                _logger?.LogInformation($"Core registered. Id {metadata.Id}, Display name {metadata.DisplayName}");
            }
        }

        private async Task InitializeCoreRankingAsync()
        {
#warning Move into core management service.
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"Initializing core ranking");

            var coreManager = Ioc.Default.GetRequiredService<ICoreManagementService>();
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
                _logger?.LogInformation($"No existing core rankings found, creating default ranking...");

                // TODO: Show abstractUI and let the user rank the cores manually.
                foreach (var instance in coreInstanceRegistry)
                    coreRanking.Add(instance.Key);
            }

            Guard.IsGreaterThan(coreRanking.Count, 0, nameof(coreRanking.Count));
            Guard.IsNotNull(_settings, nameof(_settings));

            _settings.CoreRanking = coreRanking;
            await _settings.SaveAsync();

            _logger?.LogInformation($"Core ranking initialized with {coreRanking.Count} instances.");
        }

        private async Task InitializeServicesAsync()
        {
            UpdateStatus("> ...");

            IServiceCollection services = new ServiceCollection();

            var settingsDirectory = await ApplicationData.Current.LocalFolder.CreateFolderAsync(nameof(AppSettings), Windows.Storage.CreationCollisionOption.OpenIfExists);

            _settings = new AppSettings(new FolderData(settingsDirectory));
            await _settings.LoadAsync();

            UpdateStatus("InitServices");

            _logger = _settings.IsLoggingEnabled ? AddNLog(services) : AddLoggerBlackHole(services);

            Guard.IsNotNull(_logger, nameof(_logger));

            _logger?.LogInformation("Logger initialized");

            _logger?.LogInformation("Constructing manually instantiated services");

            _mainPage = new MainPage();

            var fileSystemService = new FileSystemService();
            var coreManagementService = new CoreManagementService(_settings);

            _playbackHandlerService = new PlaybackHandlerService();
            _notificationService = new NotificationService();
            
            var strixDevice = new StrixDevice(_playbackHandlerService);
            _playbackHandlerService.SetStrixDevice(strixDevice);

#if NETFX_CORE
            _smtpHandler = new SystemMediaTransportControlsHandler(_playbackHandlerService);
#endif

            var mainViewModel = new MainViewModel(strixDevice, _notificationService, coreManagementService);

            _settings.PropertyChanged += OnSettingChanged;

            _logger?.LogInformation($"Setting up localization");

            _localizationService = new LocalizationResourceLoader();
            _localizationService.RegisterProvider(Constants.Localization.StartupResource);
            _localizationService.RegisterProvider(Constants.Localization.QuipsResource);

            _localizationService.RegisterProvider(Constants.Localization.SuperShellResource);
            _localizationService.RegisterProvider(Constants.Localization.CommonResource);
            _localizationService.RegisterProvider(Constants.Localization.TimeResource);
            _localizationService.RegisterProvider(Constants.Localization.MusicResource);

            // TODO: Add debug boot mode.
            // #741
            ShowQuip();

            _logger?.LogInformation("Adding services to IoC");

            services.AddSingleton<INavigationService<Control>, NavigationService<Control>>();
            services.AddSingleton<IPlaybackHandlerService>(_playbackHandlerService);

#if NETFX_CORE
            services.AddSingleton(_smtpHandler);
#endif

            services.AddSingleton(strixDevice);
            services.AddSingleton(mainViewModel);
            services.AddSingleton(_mainPage);

            services.AddSingleton<ILocalizationService>(_localizationService);
            services.AddSingleton(_settings);
            services.AddSingleton<ISharedFactory>(new SharedFactory());
            services.AddSingleton<IFileSystemService>(fileSystemService);
            services.AddSingleton<INotificationService>(_notificationService);
            services.AddSingleton<ICoreManagementService>(coreManagementService);
            services.AddSingleton<ILauncher, Launcher>();

            var serviceProvider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

            UpdateStatus("SetupServices");
            _logger?.LogInformation($"Services created and configured, initializing services");

            await fileSystemService.InitAsync();

            var notificationService = serviceProvider.GetRequiredService<INotificationService>();
            _navService = serviceProvider.GetRequiredService<INavigationService<Control>>();

            // TODO: Remove all usage of DataContext. Use DependencyProperties instead.
            DataContext = mainViewModel;
            Bindings.Update();

            await coreManagementService.InitAsync();

            UpdateStatus("SetupPlugins");
            _logger?.LogInformation($"Services initialized, setting up plugins");

            mainViewModel.Plugins.ModelPlugins.Import(new PlaybackHandlerPlugin(_playbackHandlerService));
            mainViewModel.Plugins.ModelPlugins.Import(new PopulateEmptyNamesPlugin()
            {
                EmptyAlbumName = _localizationService["Music", "UnknownAlbum"],
                EmptyArtistName = _localizationService["Music", "UnknownArtist"],
                EmptyDefaultName = _localizationService["Music", "UnknownName"],
            });

            _navService.RegisterCommonPage(_mainPage);
            App.AppFrame.SetupMainViewModel(mainViewModel);
            App.AppFrame.SetupNotificationService((NotificationService)notificationService);

            _logger?.LogInformation($"{nameof(InitializeServicesAsync)} completed");
        }

        private void OnSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel is null || _settings is null)
                return;

            if (e.PropertyName == nameof(AppSettings.CoreRanking))
                ViewModel.MergeConfig.CoreRanking = _settings.CoreRanking;

            if (e.PropertyName == nameof(AppSettings.MergedCollectionSorting))
                ViewModel.MergeConfig.MergedCollectionSorting = _settings.MergedCollectionSorting;
        }

        private async Task InitializeOutOfBoxSetupIfNeededAsync()
        {
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeededAsync)} entered");

            var coreManager = Ioc.Default.GetRequiredService<ICoreManagementService>();
            var coreInstanceRegistry = await coreManager.GetCoreInstanceRegistryAsync();
            Guard.IsNotNull(CurrentWindow.AppFrame.ContentOverlay, nameof(CurrentWindow.AppFrame.ContentOverlay));

            // Todo: If coreRegistry is empty, show out of box setup page.
            if (coreInstanceRegistry.Count != 0)
            {
                _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeededAsync)}: {coreInstanceRegistry.Count} core instances registered, bypassing OOBE.");
                return;
            }

            _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeededAsync)}: Setting up temporary OOBE");

            // Temp out of box setup
            var notifService = Ioc.Default.GetRequiredService<INotificationService>();

            var doneButton = new AbstractButton($"{nameof(AppLoadingView)}.OOBEFinishedButton", "Done", null, AbstractButtonType.Confirm);
            var notification = notifService.RaiseNotification(new AbstractUICollection($"{nameof(AppLoadingView)}.OOBEElementGroup", PreferredOrientation.Horizontal)
            {
                Title = "First time?",
                Subtitle = "Set up your skins and services before proceeding. A proper OOBE will come later.",
                Items = new List<AbstractUIElement>()
                {
                    doneButton,
                },
            });

            // TODO Need a real OOBE instead of using SuperShell
            CurrentWindow.NavigationService.NavigateTo(typeof(SuperShell), true);

            // TODO Temp, not great. Need a proper flow here.
            var setupFinishedSemaphore = new SemaphoreSlim(0, 1);

            notification.Dismissed += OnNotificationDismissed;
            doneButton.Clicked += OnDoneButtonClicked;

            async void OnNotificationDismissed(object? sender, EventArgs e)
            {
                _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeededAsync)}: {nameof(OnNotificationDismissed)}, completing setup");

                var registry = await coreManager.GetCoreInstanceRegistryAsync();

                if (registry.Count > 0)
                    setupFinishedSemaphore.Release();
                else
                {
                    await InitializeOutOfBoxSetupIfNeededAsync();
                    setupFinishedSemaphore.Release();
                }
            }

            async void OnDoneButtonClicked(object? sender, EventArgs e)
            {
                _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeededAsync)}: {nameof(OnDoneButtonClicked)}, completing setup");

                var registry = await coreManager.GetCoreInstanceRegistryAsync();

                if (registry.Count > 0)
                    setupFinishedSemaphore.Release();
            }

            await setupFinishedSemaphore.WaitAsync();
            _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeededAsync)}: Setup complete");

            notification.Dismissed -= OnNotificationDismissed;
            doneButton.Clicked -= OnDoneButtonClicked;

            notification.Dismiss();
        }

        private async Task InitializeConfiguredCoresAsync()
        {
            Guard.IsNotNull(_localizationService, nameof(_localizationService));
            Guard.IsNotNull(_logger, nameof(_logger));

            _logger?.LogInformation($"Initializing configured cores");
            UpdateStatus("InitCores");

            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            await mainViewModel.InitAsync();

            UpdateStatus("SetupMedia");
            foreach (var core in mainViewModel.Cores)
            {
                SetupMediaPlayer(core);
            }
        }

        private void ShowQuip()
        {
            Guard.IsNotNull(_logger, nameof(_logger));

            if (_localizationService == null)
            {
                _logger?.LogWarning($"Localization service not found. Unable to display quip.");
                PART_Status.Text = string.Empty;
                return;
            }

            var quip = new QuipLoader(CultureInfo.CurrentCulture.TwoLetterISOLanguageName).GetGroupIndexQuip();

            PART_Status.Text = _localizationService[Constants.Localization.QuipsResource, $"{quip.Item1}{quip.Item2}"];
            _showingQuip = true;
        }

        private void UpdateStatus(string text)
        {
            if (_showingQuip)
                return;

            if (_localizationService != null)
                text = _localizationService[Constants.Localization.StartupResource, text];

            PART_Status.Text = text;
        }

        private void UpdateStatusRaw(string text)
        {
            PART_Status.Text = text;
        }

        private void StrixIconAnimation_Finished(object sender, EventArgs e)
        {
            Guard.IsNotNull(_mainPage, nameof(_mainPage));
            CurrentWindow.NavigationService.NavigateTo(_mainPage);
        }

        private void SetupMediaPlayer(ICore core)
        {
            Guard.IsNotNull(_playbackHandlerService, nameof(_playbackHandlerService));
            Guard.IsNotNull(_mainPage, nameof(_mainPage));
            Guard.IsNotNull(_logger, nameof(_logger));

#if __WASM__
            _logger?.LogInformation($"WASM detected, skipping mediaplayer setup");
            return;
#endif

#pragma warning disable CS0162 // Unreachable code detected
            _logger?.LogInformation($"Setting up MediaPlayer for core instance {core.InstanceId}");

            if (core.CoreConfig.PlaybackType == MediaPlayerType.Standard)
            {
                var mediaPlayerElement = _mainPage.CreateMediaPlayerElement();

                _playbackHandlerService.RegisterAudioPlayer(new AudioPlayerService(mediaPlayerElement), core.InstanceId);
            }
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}
