using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OwlCore.Provisos;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Helpers;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Messages;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.MediaPlayback;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using StrixMusic.Shared.Services;
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
        private readonly DefaultSettingsService _settingsService;
        private readonly TextStorageService _textStorageService;
        private bool _showingQuip;
        private LocalizationResourceLoader? _localizationService;
        private PlaybackHandlerService? _playbackHandlerService;
        private SystemMediaTransportControlsHandler? _smtpHandler;
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

            _textStorageService = new TextStorageService();
            _settingsService = new DefaultSettingsService(_textStorageService);

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

            await InitializeServices();
            InitializeCoreRegistry();
            InitializeShellRegistry();
            await InitializeOutOfBoxSetupIfNeeded();
            await InitializeCoreRanking();
            await InitializeConfiguredCores();

            Guard.IsNotNull(_logger, nameof(_logger));

            _logger?.LogInformation($"{nameof(AppLoadingView_OnLoaded)} completed, navigating to shell.");
            UpdateStatusRaw($"Done loading");

            StrixIcon.FinishAnimation();
        }

        private void InitializeShellRegistry()
        {
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"Initializing shell registry");

            ShellRegistry.ShellRegistered += OnShellRegistered;

            // TODO: Create a way to load these dynamically (no reflection).
            Shells.Sandbox.Registration.Execute();
            Shells.ZuneDesktop.Registration.Execute();
            Shells.Groove.Registration.Execute();

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
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"Initializing core registry");

            CoreRegistry.CoreRegistered += OnCoreRegistered;

            // TODO: Create a way to load these dynamically (no reflection).
            Cores.LocalFiles.Registration.Execute();
            Cores.OneDrive.Registration.Execute();

            if (CoreRegistry.MetadataRegistry.Count == 0)
                ThrowHelper.ThrowInvalidOperationException($"{nameof(CoreRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 core.");

            CoreRegistry.CoreRegistered -= OnCoreRegistered;

            void OnCoreRegistered(object? sender, CoreMetadata metadata)
            {
                _logger?.LogInformation($"Core registered. Id {metadata.Id}, Display name {metadata.DisplayName}");
            }
        }

        private async Task InitializeCoreRanking()
        {
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"Initializing core ranking");

            var coreManager = Ioc.Default.GetRequiredService<ICoreManagementService>();
            var coreInstanceRegistry = await coreManager.GetCoreInstanceRegistryAsync();

            Guard.IsGreaterThan(coreInstanceRegistry.Count, 0, nameof(coreInstanceRegistry.Count));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var existingCoreRanking = await Task.Run(() => _settingsService.GetValue<List<string>>(nameof(SettingsKeys.CoreRanking)));

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

            await _settingsService.SetValue<List<string>>(nameof(SettingsKeys.CoreRanking), coreRanking);
            _logger?.LogInformation($"Core ranking initialized with {coreRanking.Count} instances.");
        }

        private async Task InitializeServices()
        {
            UpdateStatus("> ...");

            IServiceCollection services = new ServiceCollection();

            var isLoggingEnabled = await _settingsService.GetValue<bool>(nameof(SettingsKeys.IsLoggingEnabled));

            UpdateStatus("InitServices");

            _logger = isLoggingEnabled ? AddNLog(services) : AddLoggerBlackHole(services);

            Guard.IsNotNull(_logger, nameof(_logger));

            _logger?.LogInformation("Logger initialized");

            _logger?.LogInformation("Constructing manually instantiated services");

            _playbackHandlerService = new PlaybackHandlerService();

#if NETFX_CORE
            _smtpHandler = new SystemMediaTransportControlsHandler(_playbackHandlerService);
#endif

            _mainPage = new MainPage();

            var strixDevice = new StrixDevice(_playbackHandlerService);
            _playbackHandlerService.SetStrixDevice(strixDevice);

            var fileSystemService = new FileSystemService();
            var cacheFileSystemService = new DefaultCacheService();
            var coreManagementService = new CoreManagementService(_settingsService);

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
            services.AddSingleton<MainViewModel>();
            services.AddSingleton(_mainPage);

            services.AddSingleton<ILocalizationService>(_localizationService);
            services.AddSingleton<ITextStorageService>(_textStorageService);
            services.AddSingleton<ISettingsService>(_settingsService);
            services.AddSingleton<CacheServiceBase>(cacheFileSystemService);
            services.AddSingleton<ISharedFactory>(new SharedFactory());
            services.AddSingleton<IFileSystemService>(fileSystemService);
            services.AddSingleton<INotificationService>(new NotificationService());
            services.AddSingleton<ICoreManagementService>(coreManagementService);
            services.AddSingleton<ILauncher, Launcher>();

            var serviceProvider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

            UpdateStatus("SetupServices");
            _logger?.LogInformation($"Services created and configured, initializing services");

            await fileSystemService.InitAsync();
            await cacheFileSystemService.InitAsync();
            await coreManagementService.InitAsync();

            var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();
            _navService = serviceProvider.GetRequiredService<INavigationService<Control>>();
            _mainPage = serviceProvider.GetRequiredService<MainPage>();

            DataContext = mainViewModel;
            Bindings.Update();

            _navService.RegisterCommonPage(_mainPage);
            App.AppFrame.SetupMainViewModel(mainViewModel);
            App.AppFrame.SetupNotificationService((NotificationService)notificationService);

            _logger?.LogInformation($"{nameof(InitializeServices)} completed");
        }

        private async Task InitializeOutOfBoxSetupIfNeeded()
        {
            Guard.IsNotNull(_logger, nameof(_logger));
            _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeeded)} entered");

            var coreManager = Ioc.Default.GetRequiredService<ICoreManagementService>();
            var coreInstanceRegistry = await coreManager.GetCoreInstanceRegistryAsync();
            Guard.IsNotNull(CurrentWindow.AppFrame.ContentOverlay, nameof(CurrentWindow.AppFrame.ContentOverlay));

            // Todo: If coreRegistry is empty, show out of box setup page.
            if (coreInstanceRegistry.Count != 0)
            {
                _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeeded)}: {coreInstanceRegistry.Count} core instances registered, bypassing OOBE.");
                return;
            }

            _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeeded)}: Setting up temporary OOBE");

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
                _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeeded)}: {nameof(OnNotificationDismissed)}, completing setup");

                var registry = await coreManager.GetCoreInstanceRegistryAsync();

                if (registry.Count > 0)
                    setupFinishedSemaphore.Release();
                else
                {
                    await InitializeOutOfBoxSetupIfNeeded();
                    setupFinishedSemaphore.Release();
                }
            }

            async void OnDoneButtonClicked(object? sender, EventArgs e)
            {
                _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeeded)}: {nameof(OnDoneButtonClicked)}, completing setup");

                var registry = await coreManager.GetCoreInstanceRegistryAsync();

                if (registry.Count > 0)
                    setupFinishedSemaphore.Release();
            }

            await setupFinishedSemaphore.WaitAsync();
            _logger?.LogInformation($"{nameof(InitializeOutOfBoxSetupIfNeeded)}: Setup complete");

            notification.Dismissed -= OnNotificationDismissed;
            doneButton.Clicked -= OnDoneButtonClicked;

            notification.Dismiss();
        }

        private async Task InitializeConfiguredCores()
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

            _logger?.LogInformation($"Setting up MediaPlayer for core instance {core.InstanceId}");

            if (core.CoreConfig.PlaybackType == MediaPlayerType.Standard)
            {
                var mediaPlayerElement = _mainPage.CreateMediaPlayerElement();

                _playbackHandlerService.RegisterAudioPlayer(new AudioPlayerService(mediaPlayerElement), core.InstanceId);
            }
        }
    }
}
