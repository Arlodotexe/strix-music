using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using OwlCore.Services;
using StrixMusic.Core.MusicBrainz;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Shared.Services;
using Windows.ApplicationModel.Core;
using Windows.Storage;
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
        private DefaultSettingsService? _settingsService;
        private ILocalizationService? _localizationService;
        private IPlaybackHandlerService? _playbackHandlerService;

        /// <summary>
        /// The ViewModel for this page.
        /// </summary>
        public MainViewModel ViewModel => CurrentWindow.MainViewModel;

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

        private void UpdateStatus(string text)
        {
            PART_Status.Text = text;
        }

        private async void AppLoadingView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeServices();

            await InitRegisteredCoresOrShowOOBE();
        }

        private async Task InitializeServices()
        {
            //UpdateStatus("Initializing services");

            IServiceCollection services = new ServiceCollection();
            _localizationService = new LocalizationService();
            _localizationService.RegisterProvider(Constants.Localization.StartupResource);

            // UpdateStatus("Initializing services");
            UpdateStatus(_localizationService[
                Constants.Localization.StartupResource,
                "InitServices"]);

            _localizationService.RegisterProvider(Constants.Localization.SuperShellResource);
            _localizationService.RegisterProvider(Constants.Localization.CommonResource);
            _localizationService.RegisterProvider(Constants.Localization.MusicResource);

            var contextualServiceLocator = new ContextualServiceLocator();

            var textStorageService = new TextStorageService();
            _settingsService = new DefaultSettingsService(textStorageService);

            var fileSystemService = new FileSystemService();
            var cacheFileSystemService = new DefaultCacheService();

            services.AddSingleton(_localizationService);
            services.AddSingleton(contextualServiceLocator);
            services.AddSingleton<ITextStorageService>(textStorageService);
            services.AddSingleton<ISettingsService>(_settingsService);
            services.AddSingleton<CacheServiceBase>(cacheFileSystemService);
            services.AddSingleton<ISharedFactory, SharedFactory>();
            services.AddSingleton<IFileSystemService>(fileSystemService);

            _playbackHandlerService = new PlaybackHandlerService();
            services.AddSingleton(_playbackHandlerService);

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            // UpdateStatus("Initializing filesystem");
            UpdateStatus(_localizationService[
                Constants.Localization.StartupResource,
                "InitFilesystem"]);

            await fileSystemService.InitAsync();
            await cacheFileSystemService.InitAsync();
        }

        // TODO: Rename this method or split up the code better.
        private async Task InitRegisteredCoresOrShowOOBE()
        {
            if (_localizationService is null)
            {
                UpdateStatus("Localization service error.");
                return;
            }

            if (_settingsService is null)
            {
                // UpdateStatus("Fatal internal error: Settings service wasn't initialized.");
                UpdateStatus(_localizationService[
                    Constants.Localization.StartupResource,
                    "ErrSettingsInit"]);
                return;
            }

            // UpdateStatus("Initializing core registry");
            UpdateStatus(_localizationService[
                Constants.Localization.StartupResource,
                "InitCoreReg"]);
            var coreRegistry = await Task.Run(() => _settingsService.GetValue<Dictionary<string, Type>>(nameof(SettingsKeys.CoreRegistry)));

            // Todo: If coreRegistry is null, show out of box setup page.
            if (coreRegistry == null)
            {
            }

            // UpdateStatus("Creating core instances");
            UpdateStatus(_localizationService[
                Constants.Localization.StartupResource,
                "CreatingCores"]);
            var cores = await Task.Run(() => coreRegistry.Select(x => (ICore)Activator.CreateInstance(x.Value, x.Key)).ToList());

            UpdateStatus($"Adding temp {nameof(MusicBrainzCore)} instance");
            cores.Add(new MusicBrainzCore("testInstance"));

            UpdateStatus("Setting up ranking for temp cores.");
            await _settingsService.SetValue<IReadOnlyList<Type>>(nameof(SettingsKeys.CoreRanking), typeof(MusicBrainzCore).IntoList()).RunInBackground();

            //UpdateStatus("Initializing cores");
            UpdateStatus(_localizationService[
                Constants.Localization.StartupResource,
                "InitCores"]);
            var initData = await cores.InParallel(CreateCoreInitDataAsync);

            await CurrentWindow.MainViewModel.InitializeCoresAsync(initData).RunInBackground();

            // UpdateStatus("Setting up media players");
            UpdateStatus(_localizationService[
                Constants.Localization.StartupResource,
                "SetupMedia"]);
            cores.ForEach(SetupMediaPlayer);

            UpdateStatus($"Done loading, navigating to {nameof(MainPage)}");
            CurrentWindow.NavigationService?.NavigateTo(typeof(MainPage));
        }

        private async Task<(ICore core, IServiceCollection services)> CreateCoreInitDataAsync(ICore core)
        {
            var services = await CreateInitialServicesForCoreAsync(core);
            return (core, services);
        }

        private async Task<IServiceCollection> CreateInitialServicesForCoreAsync(ICore core)
        {
            var services = new ServiceCollection();
            StorageFolder rootStorageFolder;

            try
            {
                rootStorageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(core.InstanceId);
            }
            catch (FileNotFoundException)
            {
                rootStorageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(core.InstanceId);
            }

            services.AddSingleton<IFileSystemService>(new FileSystemService(rootStorageFolder));

            return services;
        }

        private void SetupMediaPlayer(ICore core)
        {
            Guard.IsNotNull(_playbackHandlerService, nameof(_playbackHandlerService));

            if (core.CoreConfig.PreferredPlayerType == MediaPlayerType.Standard)
            {
                var mediaPlayerElement = CurrentWindow.AppFrame.MainPage.CreateMediaPlayerElement();

                _playbackHandlerService.RegisterAudioPlayer(new AudioPlayerService(mediaPlayerElement), core);
            }
        }
    }
}
