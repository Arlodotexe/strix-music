using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Toolkit.Diagnostics;
using Nito.AsyncEx;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Services.AbstractUIStorageExplorers;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    ///  <inheritdoc/>
    public sealed class OneDriveCoreConfig : ICoreConfig
    {
        private AbstractTextBox? _clientIdTb;
        private AbstractTextBox? _tenantTb;
        private AbstractTextBox? _redirectUriTb;
        private GraphServiceClient? _graphServiceClient;

        private ISettingsService? _settingsService;
        private AuthenticationManager? _authenticationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreConfig"/> class.
        /// </summary>
        public OneDriveCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public IReadOnlyList<AbstractUICollection> AbstractUIElements { get; private set; } = new List<AbstractUICollection>();

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        public Task SetupConfigurationServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(OneDriveCoreStorageService));

            Services = services.BuildServiceProvider();

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Set up <see cref="AbstractUIElements"/> for configuration.
        /// </summary>
        public void SetupAbstractUIForConfig()
        {
            _clientIdTb = new AbstractTextBox("ClientId", string.Empty)
            {
                PlaceholderText = "Enter client id here.",
            };

            _tenantTb = new AbstractTextBox("Tenant Id", string.Empty)
            {
                PlaceholderText = "Enter tenant id here.",
            };

            _redirectUriTb = new AbstractTextBox("Redirect Uri", string.Empty)
            {
                PlaceholderText = "Enter redirect uri (If Any).",
            };

            var startButton = new AbstractButton("Start", "Get Started");

            startButton.Clicked += StartButton_Clicked;

            AbstractUIElements = new List<AbstractUICollection>
            {
                new AbstractUICollection("SettingsGroup")
                {
                    Title = "OneDrive Settings.",
                    Items = new List<AbstractUIElement>
                    {
                        _clientIdTb,
                        _tenantTb,
                        _redirectUriTb,
                        startButton
                    },
                }
            };

            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Setups the graph client using the credentials, and prepares an action for access token.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="tenantId"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<bool> SetupAuthenticationManager(string clientId, string tenantId, string redirectUri)
        {
            _authenticationManager =
                new AuthenticationManager(clientId, tenantId, redirectUri);

            _graphServiceClient = await _authenticationManager.GenerateGraphToken();
            return _graphServiceClient != null;
        }

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            Guard.IsNotNull(Services, nameof(Services));
            Guard.IsNotNull(_clientIdTb, nameof(_clientIdTb));
            Guard.IsNotNull(_tenantTb, nameof(_tenantTb));
            Guard.IsNotNull(_redirectUriTb, nameof(_redirectUriTb));

            if (string.IsNullOrWhiteSpace(_clientIdTb.Value) || string.IsNullOrWhiteSpace(_tenantTb.Value))
                return;

            var tokenAcquired = await SetupAuthenticationManager(_clientIdTb.Value.Trim(), _tenantTb.Value.Trim(), _redirectUriTb.Value.Trim());

            if (!tokenAcquired)
            {
                // TODO: Show error: Unauthorized to get access token.
                return;
            }

            _settingsService = new OneDriveCoreSettingsService(SourceCore.InstanceId);

            var saveTasks = new List<Task>
            {
                _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.RedirectUri), _redirectUriTb.Value.Trim()),
                _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId), _clientIdTb.Value.Trim()),
                _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId), _tenantTb.Value.Trim()),
            };

            await saveTasks.WhenAll();

            var oneDriveService = Services.GetService<OneDriveCoreStorageService>();
            Guard.IsNotNull(oneDriveService, nameof(oneDriveService));
            Guard.IsNotNull(_graphServiceClient, nameof(_graphServiceClient));

            oneDriveService.Init(_graphServiceClient);

            var rootFolder = await oneDriveService.GetRootFolderAsync();

            await InitFileExplorer(rootFolder);
        }

        private async Task InitFileExplorer(IFolderData folder)
        {
            var fileExplorer = new AbstractFolderExplorer(folder);

            await fileExplorer.InitAsync();
            Guard.IsNotNull(fileExplorer.AbstractUI, nameof(fileExplorer.AbstractUI));

            AbstractUIElements = fileExplorer.AbstractUI.IntoList();

            fileExplorer.FolderSelected += FolderExplorerService_FolderSelected;
            fileExplorer.DirectoryChanged += FileExplorer_DirectoryChanged;
        }

        private void FileExplorer_DirectoryChanged(object sender, IFolderData e)
        {
            var fileExplorer = (AbstractFolderExplorer)sender;

            Guard.IsNotNull(fileExplorer.AbstractUI, nameof(fileExplorer.AbstractUI));

            AbstractUIElements = fileExplorer.AbstractUI.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FolderExplorerService_FolderSelected(object sender, IFolderData e)
        {
            Guard.IsNotNull(_settingsService, nameof(OneDriveCoreSettingsService));
            var fileExplorer = (AbstractFolderExplorer)sender;
            fileExplorer.FolderSelected -= FolderExplorerService_FolderSelected;

            SetupAbstractUIForConfig();

            _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.FolderPath), e.Path);
            SourceCore.Cast<OneDriveCore>().ChangeCoreState(CoreState.Configured);
            SourceCore.Cast<OneDriveCore>().ChangeCoreState(CoreState.Loaded);

            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
