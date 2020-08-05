using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Strix_Music.Shell.Default.Controls;
using StrixMusic.Services.Settings;
using StrixMusic.Services.Settings.Enums;
using StrixMusic.Services.SuperShell;
using StrixMusix.ViewModels;
using System;
using System.Threading.Tasks;
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
            Ioc.Default.GetService<ISuperShellService>().Show(SuperShellDisplays.Settings);
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
            Ioc.Default.GetService<ISettingsService>().SettingChanged += SettingsService_SettingChanged;
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetService<ISettingsService>().SettingChanged -= SettingsService_SettingChanged;
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
            await SetupPreferredShell();

            SuperShellDisplay.Content = new SuperShell();
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
                        if (dict.Source.AbsoluteUri.Contains("Default"))
                            continue;

                        App.Current.Resources.MergedDictionaries.Remove(dict);
                        break;
                    }
                }

                var preferredShell = await Ioc.Default.GetService<ISettingsService>().GetValue<PreferredShell>(nameof(SettingsKeys.PreferredShell));

                if (preferredShell != PreferredShell.Default)
                {
                    var assemblyName = $"ms-appx:///{shellNamespacePrefix}.{preferredShell}/Resources.xaml";

                    var resourceDictionary = new ResourceDictionary() { Source = new Uri(assemblyName) };

                    App.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }

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
