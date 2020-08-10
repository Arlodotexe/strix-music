using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Helpers;
using StrixMusic.Services.Settings;
using StrixMusic.Services.StorageService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.SuperShellControls
{
    public sealed partial class SuperShellSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShellSettings"/> class.
        /// </summary>
        public SuperShellSettings()
        {
            this.InitializeComponent();

            Loaded += SuperShellSettings_Loaded;
        }

        private async void SuperShellSettings_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SuperShellSettings_Loaded;
            await InitSkins();
        }

        private async Task InitSkins()
        {
            foreach (string name in Constants.Shells.LoadedShells)
            {
                Skins.Add(name);
            }

            CurrentSkin = await Ioc.Default.GetService<ISettingsService>().GetValue<string>(nameof(SettingsKeys.PreferredShell));

            Bindings.Update();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newPreferredSkin = e.AddedItems.FirstOrDefault()?.ToString();
            if (newPreferredSkin == null)
                return;

            Ioc.Default.GetService<ISettingsService>().SetValue<string>(nameof(SettingsKeys.PreferredShell), newPreferredSkin);
        }

        private async void ButtonFolderSelect_Clicked(object sender, RoutedEventArgs e)
        {
            var fileSystemSvc = Ioc.Default.GetService<IFileSystemService>();

            await fileSystemSvc.PickFolder();
        }

        /// <summary>
        /// The labels for the skins that the user can choose from.
        /// </summary>
        public ObservableCollection<string> Skins { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The skin that the app is currently using.
        /// </summary>
        public string CurrentSkin { get; set; } = "Loading...";
    }
}
