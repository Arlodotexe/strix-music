using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using Uno.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The SuperShell is a top-level overlay that will always show on top of all other shells. It provides various essential app functions, such as changing settings, setting your shell, viewing debug info, and managing cores.
    /// </summary>
    public sealed partial class SuperShell : UserControl
    {
        private readonly ISettingsService _settingsService;
        private bool _loadingShells = true;

        /// <summary>
        /// The labels for the skins that the user can choose from.
        /// </summary>
        public ObservableCollection<ShellAssemblyInfo> Skins { get; set; }

        /// <summary>
        /// TEMPORARY. Allows binding to a group of <see cref="AbstractUIElementGroup"/>s.
        /// </summary>
        public ObservableCollection<AbstractUIElementGroup> AbstractUIGroups { get; set; } = new ObservableCollection<AbstractUIElementGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell()
        {
            this.InitializeComponent();

            Skins = new ObservableCollection<ShellAssemblyInfo>();
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();

            Loaded += SuperShell_Loaded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class, and displays a core's <see cref="AbstractUIElement"/>s.
        /// </summary>
        public SuperShell(ICore core)
            : this()
        {
            SetupAbstractUI(core);
        }

        private async void SuperShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SuperShell_Loaded;

            await InitSkins();
        }

        // This is temporary, to test abstract UI.
        private void SetupAbstractUI(ICore core)
        {
            AbstractUIGroups.Clear();
            AbstractUIGroups.AddRange(core.CoreConfig.AbstractUIElements);
        }

        private async Task InitSkins()
        {
            // Gets the preferred shell's assembly name
            var preferredShell = await _settingsService.GetValue<string>(nameof(SettingsKeys.PreferredShell));

            var shellRegistry = await _settingsService.GetValue<IReadOnlyList<ShellAssemblyInfo>>(nameof(SettingsKeysUI.ShellRegistry));

            // Gets the list of loaded shells.
            foreach (var shell in shellRegistry)
            {
                Skins.Add(shell);

                // Mark the current shell selected or Default Shell as the backup.
                if (shell.DisplayName == "Default Shell" || shell.AssemblyName == preferredShell)
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
                return;

            // Gets the selected preferred skin.
            ShellAssemblyInfo? newPreferredSkin = ShellSelector.SelectedItem as ShellAssemblyInfo;
            if (newPreferredSkin == null)
                return;

            // Saves the assembly name.
            await _settingsService.SetValue<string>(nameof(SettingsKeys.PreferredShell), newPreferredSkin.AssemblyName);
        }
    }
}
