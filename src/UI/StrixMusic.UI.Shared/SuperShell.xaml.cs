using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shared.ViewModels;
using Uno.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Uno.Controls.Views;

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
        /// Dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(SuperShellViewModel), typeof(SuperShell), new PropertyMetadata(new SuperShellViewModel()));

        /// <summary>
        /// The labels for the skins that the user can choose from.
        /// </summary>
        public ObservableCollection<ShellAssemblyInfo> Skins { get; set; }

        /// <summary>
        /// ViewModel for this control.
        /// </summary>
        public SuperShellViewModel ViewModel
        {
            get => (SuperShellViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell()
        {
            InitializeComponent();

            Skins = new ObservableCollection<ShellAssemblyInfo>();

            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            Loaded += SuperShell_Loaded;
            Unloaded += SuperShell_Unloaded;
        }

        private void DetachEvents()
        {
            Unloaded -= SuperShell_Unloaded;
        }

        private void SuperShell_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private async void SuperShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SuperShell_Loaded;

            await ViewModel.InitAsync();
            await InitSkins();
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
