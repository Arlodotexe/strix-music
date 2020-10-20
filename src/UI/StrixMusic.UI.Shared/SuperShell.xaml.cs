using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using StrixMusic.Helpers;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    public sealed partial class SuperShell : UserControl
    {

        private bool _loadingShells = true;

        /// <summary>
        /// The labels for the skins that the user can choose from.
        /// </summary>
        public ObservableCollection<ShellModel> Skins { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell()
        {
            this.InitializeComponent();

            Skins = new ObservableCollection<ShellModel>();

            Loaded += SuperShell_Loaded;
        }

        private async void SuperShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SuperShell_Loaded;
            await InitSkins();
        }

        private async Task InitSkins()
        {
            // Gets the preferred shell's assembly name
            var preferredShell = await Ioc.Default.GetService<ISettingsService>().GetValue<string>(nameof(SettingsKeys.PreferredShell));

            // Gets the list of loaded shells.
            foreach (ShellModel shell in Constants.Shells.LoadedShells.Values)
            {
                Skins.Add(shell);

                // Mark the current shell selected or Default Shell as the backup.
                if (shell.DisplayName == Constants.Shells.DefaultShellDisplayName || shell.AssemblyName == preferredShell)
                {
                    ShellSelector.SelectedItem = shell;
                }
            }

            // Declares loading finished.
            _loadingShells = false;
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Returns if the shell list is still initializing.
            if (_loadingShells)
            {
                return;
            }

            // Gets the selected preferred skin.
            ShellModel? newPreferredSkin = ShellSelector.SelectedItem as ShellModel;
            if (newPreferredSkin == null)
            {
                return;
            }

            // Saves the assembly name.
            await Ioc.Default.GetService<ISettingsService>().SetValue<string>(nameof(SettingsKeys.PreferredShell), newPreferredSkin.AssemblyName);
        }

        private async void ButtonFolderSelect_Clicked(object sender, RoutedEventArgs e)
        {
            var fileSystemSvc = Ioc.Default.GetService<IFileSystemService>();

            var folder = await fileSystemSvc.PickFolder();
        }
    }
}
