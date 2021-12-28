using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.ShellManagement;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// View model used to select shells.
    /// </summary>
    public class ShellSelectorViewModel : ObservableObject, IAsyncInit
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogger<ShellSelectorViewModel> _logger;
        private ShellInfoViewModel? _preferredShell;
        private ShellInfoViewModel? _fallbackShell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellSelectorViewModel"/> class.
        /// </summary>
        public ShellSelectorViewModel()
        {
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();
            _logger = Ioc.Default.GetRequiredService<ILogger<ShellSelectorViewModel>>();

            AllShells = new ObservableCollection<ShellInfoViewModel>();
            FullyResponsiveShells = new ObservableCollection<ShellInfoViewModel>();
            SaveSelectedShellAsyncCommand = new AsyncRelayCommand(SaveSelectedShell);
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            _logger.LogInformation($"Entered {nameof(InitAsync)}");

            // Gets the preferred shell's assembly name
            var preferredShell = await _settingsService.GetValue<string>(nameof(SettingsKeysUI.PreferredShell));
            _logger.LogInformation($"Retreived preferred shell: {preferredShell}");

            var fallbackShell = await _settingsService.GetValue<string>(nameof(SettingsKeysUI.FallbackShell));
            _logger.LogInformation($"Retreived fallback shell: {fallbackShell}");

            // Gets the list of loaded shells.
            foreach (var shell in ShellRegistry.MetadataRegistry)
            {
                var viewModel = new ShellInfoViewModel(shell);
                _logger.LogInformation($"Adding {viewModel.Metadata.Id} to available shells");

                AllShells.Add(viewModel);

                if (viewModel.IsFullyResponsive)
                {
                    _logger.LogInformation($"Adding {viewModel.Metadata.Id} to fully responsive shells");
                    FullyResponsiveShells.Add(viewModel);
                }
            }

            foreach (var shell in AllShells)
            {
                // Mark the current shell selected or Default (if unset)
                if (shell.Metadata.Id == preferredShell)
                {
                    _logger.LogInformation($"Setting preferred shell: {shell.Metadata.Id}");
                    PreferredShell = shell;
                }

                if (shell.Metadata.Id == fallbackShell)
                {
                    _logger.LogInformation($"Setting fallback shell: {shell.Metadata.Id}");
                    FallbackShell = shell;
                }
            }

            _logger.LogInformation($"Exited {nameof(InitAsync)}");
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
            set
            {
                SetProperty(ref _preferredShell, value, nameof(PreferredShell));

                _ = UpdateFallbackShell();
            }
        }

        private async Task UpdateFallbackShell()
        {
            if (PreferredShell is null)
                return;

            if (PreferredShell.IsFullyResponsive)
            {
                FallbackShell = null;
            }
            else
            {
                // Setting the correct fallback shell back for non-responsive shells.
                var fallbackShell = await _settingsService.GetValue<string>(nameof(SettingsKeysUI.FallbackShell));

                foreach (var shell in AllShells)
                {
                    if (shell.Metadata.Id == fallbackShell)
                    {
                        _logger.LogInformation($"Setting fallback shell: {shell.Metadata.Id}");
                        FallbackShell = shell;
                    }
                }
            }
        }

        /// <inheritdoc cref="SettingsKeysUI.FallbackShell"/>
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
                await _settingsService.SetValue<string>(nameof(SettingsKeysUI.PreferredShell), PreferredShell.Metadata.Id);

            if (FallbackShell != null)
                await _settingsService.SetValue<string>(nameof(SettingsKeysUI.FallbackShell), FallbackShell.Metadata.Id);
        }
    }
}
