using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using StrixMusic.Services;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// View model used to select shells.
    /// </summary>
    public class ShellSelectorViewModel : ObservableObject, IAsyncInit
    {
        private readonly AppSettings _settings;
        private readonly ILogger<ShellSelectorViewModel> _logger;
        private ShellInfoViewModel? _preferredShell;
        private ShellInfoViewModel? _fallbackShell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellSelectorViewModel"/> class.
        /// </summary>
        public ShellSelectorViewModel()
        {
            _settings = Ioc.Default.GetRequiredService<AppSettings>();
            _logger = Ioc.Default.GetRequiredService<ILogger<ShellSelectorViewModel>>();

            AllShells = new ObservableCollection<ShellInfoViewModel>();
            FullyResponsiveShells = new ObservableCollection<ShellInfoViewModel>();
            SaveSelectedShellAsyncCommand = new AsyncRelayCommand(SaveSelectedShell);
        }

        /// <inheritdoc/>
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Entered {nameof(InitAsync)}");

            await _settings.LoadAsync(cancellationToken);

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

                await viewModel.InitAsync();
            }

            foreach (var shell in AllShells)
            {
                // Mark the current shell selected or Default (if unset)
                if (shell.Metadata.Id == _settings.PreferredShell)
                {
                    _logger.LogInformation($"Setting preferred shell: {shell.Metadata.Id}");
                    PreferredShell = shell;
                }

                if (shell.Metadata.Id == _settings.FallbackShell)
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
                foreach (var shell in AllShells)
                {
                    if (shell.Metadata.Id == _settings.FallbackShell)
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
                _settings.PreferredShell = PreferredShell.Metadata.Id;

            if (FallbackShell != null)
                _settings.FallbackShell = FallbackShell.Metadata.Id;

            await _settings.SaveAsync();
        }
    }
}
