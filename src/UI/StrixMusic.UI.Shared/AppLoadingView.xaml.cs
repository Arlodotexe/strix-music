using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Helpers;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
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
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        private Dictionary<string, CoreMetadata>? _coreInstanceRegistry;
        private MainPage? _mainPage;
        private INavigationService<Control>? _navService;

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

        private void ShowQuip()
        {
            if (_localizationService == null)
            {
                PART_Status.Text = "Localization Error";
                return;
            }

            var quip = new QuipLoader(Language).GetGroupIndexQuip();

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

        private async void AppLoadingView_OnLoaded(object sender, RoutedEventArgs e)
        {
            PrereleaseNoticeContainer.Visibility = Visibility.Collapsed;

            InitializeCoreRegistry();
            InitializeShellRegistry();

            await InitializeServices();
            await InitializeInstanceRegistry();
            await InitializeOutOfBoxSetupIfNeeded();
            await InitializeCoreRanking();
            await InitializeConfiguredCores();

            UpdateStatusRaw($"Done loading, navigating to {nameof(MainPage)}");

            Guard.IsNotNull(_mainPage, nameof(_mainPage));

            CurrentWindow.NavigationService.NavigateTo(_mainPage);
        }

        private void InitializeShellRegistry()
        {
            if (ShellRegistry.MetadataRegistry.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException($"{nameof(ShellRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 shell.");
                return;
            }
        }

        private void InitializeCoreRegistry()
        {
            if (CoreRegistry.MetadataRegistry.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException($"{nameof(CoreRegistry.MetadataRegistry)} contains no elements after registry initialization. App cannot function without at least 1 core.");
                return;
            }
        }

        private async Task InitializeInstanceRegistry()
        {
            UpdateStatus("InitCoreReg");

            var coreManager = Ioc.Default.GetRequiredService<ICoreManagementService>();

            _coreInstanceRegistry = await coreManager.GetCoreInstanceRegistryAsync();
        }

        private async Task InitializeCoreRanking()
        {
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var existingCoreRanking = await Task.Run(() => _settingsService.GetValue<List<string>>(nameof(SettingsKeys.CoreRanking)));

            var coreRanking = new List<string>();

            foreach (var instanceId in existingCoreRanking)
            {
                var coreMetadata = _coreInstanceRegistry.FirstOrDefault(x => x.Key == instanceId).Value;
                if (coreMetadata is null)
                    continue;

                // If this core is still configured, add it to the ranking.
                if (CoreRegistry.MetadataRegistry.Any(x => x.Id == coreMetadata.Id))
                    coreRanking.Add(instanceId);
            }

            // If no cores exist in ranking, initialize with all loaded cores.
            if (coreRanking.Count == 0)
            {
                // TODO: Show abstractUI and let the user rank the cores manually.
                foreach (var instance in _coreInstanceRegistry)
                    coreRanking.Add(instance.Key);
            }

            await _settingsService.SetValue<List<string>>(nameof(SettingsKeys.CoreRanking), coreRanking);
        }

        private async Task InitializeServices()
        {
            UpdateStatus("...");

            IServiceCollection services = new ServiceCollection();

            _playbackHandlerService = new PlaybackHandlerService();
            _smtpHandler = new SystemMediaTransportControlsHandler(_playbackHandlerService);
            _mainPage = new MainPage();

            var strixDevice = new StrixDevice(_playbackHandlerService);
            _playbackHandlerService.SetStrixDevice(strixDevice);

            services.AddSingleton<INavigationService<Control>, NavigationService<Control>>();
            services.AddSingleton<IPlaybackHandlerService>(_playbackHandlerService);
            services.AddSingleton(_smtpHandler);
            services.AddSingleton(strixDevice);
            services.AddSingleton<MainViewModel>();
            services.AddSingleton(_mainPage);

            _localizationService = new LocalizationResourceLoader();
            _localizationService.RegisterProvider(Constants.Localization.StartupResource);
            _localizationService.RegisterProvider(Constants.Localization.QuipsResource);

            _localizationService.RegisterProvider(Constants.Localization.SuperShellResource);
            _localizationService.RegisterProvider(Constants.Localization.CommonResource);
            _localizationService.RegisterProvider(Constants.Localization.TimeResource);
            _localizationService.RegisterProvider(Constants.Localization.MusicResource);

            // TODO: Add debug boot mode.
#if NETFX_CORE
            // #741
            ShowQuip();
#endif

            UpdateStatus("InitServices");

            var fileSystemService = new FileSystemService();
            var cacheFileSystemService = new DefaultCacheService();

            var coreManagementService = new CoreManagementService(_settingsService);

            services.AddSingleton<ILocalizationService>(_localizationService);
            services.AddSingleton<ITextStorageService>(_textStorageService);
            services.AddSingleton<ISettingsService>(_settingsService);
            services.AddSingleton<CacheServiceBase>(cacheFileSystemService);
            services.AddSingleton<ISharedFactory>(new SharedFactory());
            services.AddSingleton<IFileSystemService>(fileSystemService);
            services.AddSingleton<INotificationService>(new NotificationService());
            services.AddSingleton<ICoreManagementService>(coreManagementService);

            var serviceProvider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

            UpdateStatus("SetupServices");

            // Hack. Ioc doesn't resolve services immediately after building the service provider.
            // await Task.Delay(100);

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
        }

        private async Task InitializeOutOfBoxSetupIfNeeded()
        {
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsNotNull(CurrentWindow.AppFrame.ContentOverlay, nameof(CurrentWindow.AppFrame.ContentOverlay));

            // Todo: If coreRegistry is empty, show out of box setup page.
            if (_coreInstanceRegistry.Count != 0)
                return;

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

            void OnNotificationDismissed(object sender, EventArgs e)
            {
                setupFinishedSemaphore.Release();
            }

            void OnDoneButtonClicked(object sender, EventArgs e)
            {
                setupFinishedSemaphore.Release();
            }

            await setupFinishedSemaphore.WaitAsync();

            notification.Dismissed -= OnNotificationDismissed;
            doneButton.Clicked -= OnDoneButtonClicked;

            notification.Dismiss();
        }

        private async Task InitializeConfiguredCores()
        {
            Guard.IsNotNull(_localizationService, nameof(_localizationService));

            UpdateStatus("InitCores");

            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            await mainViewModel.InitAsync();

            UpdateStatus("SetupMedia");
            foreach (var core in mainViewModel.Cores)
            {
                SetupMediaPlayer(core);
            }
        }

        private void SetupMediaPlayer(ICore core)
        {
            Guard.IsNotNull(_playbackHandlerService, nameof(_playbackHandlerService));
            Guard.IsNotNull(_mainPage, nameof(_mainPage));

            if (core.CoreConfig.PlaybackType == MediaPlayerType.Standard)
            {
                var mediaPlayerElement = _mainPage.CreateMediaPlayerElement();

                _playbackHandlerService.RegisterAudioPlayer(new AudioPlayerService(mediaPlayerElement), core);
            }
        }
    }
}
