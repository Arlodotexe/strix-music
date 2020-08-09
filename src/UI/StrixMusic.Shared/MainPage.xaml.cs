using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Settings;
using StrixMusic.Services.Settings.Enums;
using StrixMusic.Services.StorageService;
using StrixMusic.Services.SuperShell;
using StrixMusic.Shell.Default.Controls;
using StrixMusic.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic
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

            // Events must be attached before initializing this.
            await Ioc.Default.GetService<IFileSystemService>().Init();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AttachEvents()
        {
            Ioc.Default.GetService<ISettingsService>().SettingChanged += SettingsService_SettingChanged;

            var fileSystemSvc = Ioc.Default.GetService<IFileSystemService>();

            fileSystemSvc.FolderScanStarted += FileSystemSvc_FolderScanStarted;
            fileSystemSvc.FolderDeepScanCompleted += FileSystemSvc_FolderDeepScanCompleted;
            fileSystemSvc.FileScanStarted += FileSystemSvc_FileScanStarted;
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetService<ISettingsService>().SettingChanged -= SettingsService_SettingChanged;

            var fileSystemSvc = Ioc.Default.GetService<IFileSystemService>();

            fileSystemSvc.FolderScanStarted -= FileSystemSvc_FolderScanStarted;
            fileSystemSvc.FolderDeepScanCompleted -= FileSystemSvc_FolderDeepScanCompleted;
            fileSystemSvc.FileScanStarted -= FileSystemSvc_FileScanStarted;
        }

        private void FileSystemSvc_FolderDeepScanCompleted(object sender, StrixMusic.CoreInterfaces.Interfaces.Storage.IFolderData e)
        {
#if !__ANDROID__
            Debug.WriteLine($"Deep scan of folder {e.Name} completed");
#endif
        }

        private void FileSystemSvc_FileScanStarted(object sender, FileScanStateEventArgs e)
        {
#if !__ANDROID__
            Debug.WriteLine($"Scanning file {e.FileData.Name}");
#endif
        }

        private void FileSystemSvc_FolderScanStarted(object sender, StrixMusic.CoreInterfaces.Interfaces.Storage.IFolderData e)
        {
#if !__ANDROID__
            Debug.WriteLine($"Scanning folder {e.Name}");
#endif
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
                var shellNamespacePrefix = "StrixMusic.Shell";

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
            ShellControl shell = new ShellControl
            {
                DataContext = Ioc.Default.GetService<MainViewModel>(),
            };
            return shell;
        }
    }
}
