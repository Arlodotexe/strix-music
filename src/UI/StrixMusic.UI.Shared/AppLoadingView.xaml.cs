using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Extensions.AsyncExtensions;
using OwlCore.Services;
using StrixMusic.Core.MusicBrainz;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    public sealed partial class AppLoadingView : UserControl
    {
        private DefaultSettingsService? _settingsService;

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
            UpdateStatus("Initializing services");

            IServiceCollection services = new ServiceCollection();
            var contextualServiceLocator = new ContextualServiceLocator();

            var textStorageService = new TextStorageService();
            _settingsService = new DefaultSettingsService(textStorageService);
            var cacheFileSystemService = new FileSystemService(ApplicationData.Current.LocalCacheFolder);
            var fileSystemService = new FileSystemService();

            var fileSystemServices = new[]
            {
                cacheFileSystemService,
                fileSystemService,
            };

            UpdateStatus("Initializing filesystem");

            await fileSystemServices.InParallel(x => x.Init());

            contextualServiceLocator.Register<IFileSystemService>(cacheFileSystemService, typeof(CacheServiceBase));

            UpdateStatus("Initializing services");

            services.AddSingleton(contextualServiceLocator);
            services.AddSingleton<ITextStorageService>(textStorageService);
            services.AddSingleton<ISettingsService>(_settingsService);
            services.AddSingleton<CacheServiceBase, DefaultCacheService>();
            services.AddSingleton<ISharedFactory, SharedFactory>();
            services.AddSingleton<IFileSystemService>(fileSystemService);

            Ioc.Default.ConfigureServices(services);
        }

        // TODO: Rename this method or split up the code better.
        private async Task InitRegisteredCoresOrShowOOBE()
        {
            if (_settingsService is null)
            {
                UpdateStatus("Fatal internal error: Settings service wasn't initialized.");
                return;
            }

            UpdateStatus("Initializing core registry");
            var coreRegistry = await Task.Run(() => _settingsService.GetValue<Dictionary<string, Type>>(nameof(SettingsKeys.CoreRegistry)));

            // Todo: If coreRegistry is null, show out of box setup page.
            if (coreRegistry == null)
            {
            }

            UpdateStatus("Creating core instances");
            var cores = await Task.Run(() => coreRegistry.Select(x => (ICore)Activator.CreateInstance(x.Value, x.Key)).ToList());

            UpdateStatus($"Adding temp {nameof(MusicBrainzCore)} instance");
            cores.Add(new MusicBrainzCore("testInstance"));

            UpdateStatus("Initializing cores");
            await Task.Run(() => CurrentWindow.MainViewModel.InitializeCores(cores));

            UpdateStatus("Simulating lag");
            await Task.Delay(2000);

            UpdateStatus($"Done loading, navigating to {nameof(ShellLoader)}");
            CurrentWindow.NavigationService?.NavigateTo(typeof(ShellLoader));
        }
    }
}
