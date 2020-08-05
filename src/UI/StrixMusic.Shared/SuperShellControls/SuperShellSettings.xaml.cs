using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Settings;
using StrixMusic.Services.Settings.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.SuperShellControls
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
            foreach (string name in Enum.GetNames(typeof(PreferredShell)))
            {
                Skins.Add(name);
            }

            var preferredSkin = await Ioc.Default.GetService<ISettingsService>().GetValue<PreferredShell>(nameof(SettingsKeys.PreferredShell));
            CurrentSkin = Enum.GetName(typeof(PreferredShell), preferredSkin);
            Bindings.Update();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault()?.ToString();
            if (selectedItem == null)
                return;

            var newPreferredSkin = Enum.Parse(typeof(PreferredShell), selectedItem);

            Ioc.Default.GetService<ISettingsService>().SetValue<PreferredShell>(nameof(SettingsKeys.PreferredShell), newPreferredSkin);
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
