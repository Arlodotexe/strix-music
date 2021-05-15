using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// View model used to select shells.
    /// </summary>
    public class ShellSelectorViewModel : ObservableObject, IAsyncInit
    {
        private readonly ISettingsService _settingsService;
        private ShellInfoViewModel? _preferredShell;
        private ShellInfoViewModel? _fallbackShell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellSelectorViewModel"/> class.
        /// </summary>
        public ShellSelectorViewModel()
        {
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();

            AllShells = new ObservableCollection<ShellInfoViewModel>();
            FullyResponsiveShells = new ObservableCollection<ShellInfoViewModel>();
            SaveSelectedShellAsyncCommand = new AsyncRelayCommand(SaveSelectedShell);
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            // Gets the preferred shell's assembly name
            var preferredShell = await _settingsService.GetValue<string>(nameof(SettingsKeysUI.PreferredShell));
            var fallbackShell = await _settingsService.GetValue<string>(nameof(SettingsKeysUI.FallbackShell));
            var shellRegistry = await _settingsService.GetValue<IReadOnlyList<ShellAssemblyInfo>>(nameof(SettingsKeysUI.ShellRegistry));

            // Gets the list of loaded shells.
            foreach (var shell in shellRegistry)
            {
                var viewModel = new ShellInfoViewModel(shell);

                AllShells.Add(viewModel);
                
                if (viewModel.IsFullyResponsive)
                    FullyResponsiveShells.Add(viewModel);
            }

            foreach (var shell in AllShells)
            {
                // Mark the current shell selected or Default (if unset)
                if (shell.AssemblyInfo.AssemblyName == preferredShell)
                {
                    PreferredShell = shell;
                    break;
                }

                if (shell.AssemblyInfo.AssemblyName == fallbackShell)
                {
                    FallbackShell = shell;
                }
            }
        }

        /// <summary>
        /// All shells that the user can pick from.
        /// </summary>
        public ObservableCollection<ShellInfoViewModel> AllShells { get; }

        /// <summary>
        /// All shells that are fully responsive. One of these should be used as a fallback when the primary shell isn't fully responsive.
        /// </summary>
        public ObservableCollection<ShellInfoViewModel> FullyResponsiveShells { get; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        public ShellInfoViewModel? PreferredShell
        {
            get => _preferredShell;
            set => SetProperty(ref _preferredShell, value, nameof(PreferredShell));
        }

        /// <inheritdoc cref="SettingsKeys.FallbackShell"/>
        public ShellInfoViewModel? FallbackShell
        {
            get => _fallbackShell;
            set => SetProperty(ref _fallbackShell, value, nameof(FallbackShell));
        }

        /// <summary>
        /// When fired, the <see cref="PreferredShell"/> is saved and applied.
        /// </summary>
        public IAsyncRelayCommand SaveSelectedShellAsyncCommand { get; }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        private async Task SaveSelectedShell()
        {
            if (PreferredShell != null)
                await _settingsService.SetValue<string>(nameof(SettingsKeysUI.PreferredShell), PreferredShell.AssemblyInfo.AssemblyName);

            if (FallbackShell != null)
                await _settingsService.SetValue<string>(nameof(SettingsKeysUI.FallbackShell), FallbackShell.AssemblyInfo.AssemblyName);
        }
    }
}
