using System;
using System.Reflection;
using System.Threading.Tasks;
using OwlCore.ArchTools;
using Strix_Music.Services;
using StrixMusic.Services.Settings;
using StrixMusic.Services.Settings.Enums;
using StrixMusic.Services.SettingsStorage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private static void RegSvc<T>(object value)
        {
            ServiceLocator.Instance.Register<T>((T)value);
        }

        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            AttachEvents();
            await Initialize();
        }

        private void MainPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AttachEvents()
        {
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;
        }

        private async Task Initialize()
        {
            await InitCores();
            InitServices();
            await SetupPreferredShell();
        }

        /// <summary>
        /// Initializes the media provider cores. Fire and forget.
        /// </summary>
        /// <returns><see cref="Task"/> representing the async operation. Returns once all cores have started init (fire and forget)</returns>
        private Task InitCores()
        {
            // TODO
            return Task.CompletedTask;
        }

        private void InitServices()
        {
            // TODO: Create storage service implementation
            RegSvc<IStorageService>(new StorageService());
            RegSvc<ISettingsService>(new SettingsService());
        }

        private async Task SetupPreferredShell()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();

            var settingsService = ServiceLocator.Instance.Resolve<ISettingsService>();

            var preferredShell = await settingsService.GetValue<PreferredShell>(nameof(SettingsKeys.PreferredShell));

            var assemblyName = $"ms-appx:///Strix_Music.Shell.{preferredShell}/Resources.xaml";

            var resourceDictionary = new ResourceDictionary() { Source = new Uri(assemblyName) };

            App.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
