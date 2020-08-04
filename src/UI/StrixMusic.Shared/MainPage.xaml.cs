using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.ArchTools;
using Strix_Music.Services;
using Strix_Music.Shell.Default.Controls;
using StrixMusic.Core.Dummy;
using StrixMusic.Services.Settings;
using StrixMusic.Services.Settings.Enums;
using StrixMusic.Services.StorageService;
using StrixMusic.Services.SuperShell;
using StrixMusix.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LazyService<ISuperShellService> _superShellService;
        private ISettingsService? _settingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Fires when the Super buttons is clicked. Temporary until a proper trigger mechanism is found for touch devices.
        /// </summary>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            _superShellService.Value.Show(SuperShellDisplays.Settings);
        }

        private static void RegSvc<T>(object value)
        {
            ServiceLocator.Instance.Register((T)value);
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            await Initialize();
            AttachEvents();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AttachEvents()
        {
            _settingService = ServiceLocator.Instance.Resolve<ISettingsService>();
            if (_settingService != null)
                _settingService.SettingChanged += SettingsService_SettingChanged;
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;
            if (_settingService != null)
                _settingService.SettingChanged -= SettingsService_SettingChanged;
        }

        private async void SettingsService_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.Key == nameof(PreferredShell))
            {
                await SetupPreferredShell();
            }
        }

        private async Task Initialize()
        {
            // TODO: Remove or replace.
            InitServices();
            await SetupPreferredShell();

            SuperShellDisplay.Content = new SuperShell();

            // Initailizing cores should not get in the way of showing the UI.
            InitCores();
        }

        /// <summary>
        /// Initializes the media provider cores. Fire and forget.
        /// </summary>
        private void InitCores()
        {
            // TODO: Create and register cores
            new DummyCore();
        }

        private void InitServices()
        {
            // TODO: Optimize load times, getting app setting takes time on other platforms.
            RegSvc<IStorageService>(new StorageService());
            RegSvc<ISettingsService>(new SettingsService());
            RegSvc<ISuperShellService>(new SuperShellService());
        }

        private async Task SetupPreferredShell()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var shellNamespacePrefix = "Strix_Music.Shell";

                ShellDisplay.Content = null;

                foreach (var dict in App.Current.Resources.MergedDictionaries)
                {
                    if (dict.Source.AbsoluteUri.Contains(shellNamespacePrefix))
                    {
                        App.Current.Resources.MergedDictionaries.Remove(dict);
                    }
                }

                var settingsService = ServiceLocator.Instance.Resolve<ISettingsService>();

                var preferredShell = await settingsService.GetValue<PreferredShell>(nameof(SettingsKeys.PreferredShell));

                var assemblyName = $"ms-appx:///{shellNamespacePrefix}.{preferredShell}/Resources.xaml";

                var resourceDictionary = new ResourceDictionary() { Source = new Uri(assemblyName) };

                App.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                ShellDisplay.Content = CreateShellControl();
            });
        }

        private ShellControl CreateShellControl()
        {
            ShellControl shell = new ShellControl();
            shell.DataContext = Ioc.Default.GetService<MainViewModel>();
            return shell;
        }
    }
}
