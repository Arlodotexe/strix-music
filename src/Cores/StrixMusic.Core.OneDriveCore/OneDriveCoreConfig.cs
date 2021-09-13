using StrixMusic.Cores.Files;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.OneDrive.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Toolkit.Diagnostics;
using Nito.AsyncEx;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using OwlCore.Services.AbstractUIStorageExplorers;
using OwlCore.Services.AbstractUIStorageExplorers.Handlers;
using StrixMusic.Cores.OneDrive.Storage;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    ///  <inheritdoc/>
    public class OneDriveCoreConfig : FilesCoreConfig
    {
        private AbstractTextBox? _clientIdTb;
        private AbstractTextBox? _tenantTb;
        private AbstractTextBox? _redirectUriTb;
        private GraphServiceClient? _graphServiceClient;

        private ISettingsService? _settingsService;
        private AuthenticationManager? _authenticationManager;
        /// <inheritdoc/>
        public override event EventHandler? AbstractUIElementsChanged;


        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreConfig"/> class.
        /// </summary>
        public OneDriveCoreConfig(ICore sourceCore)
            : base(sourceCore)
        {
        }

        public override Task SetupConfigurationServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(OneDriveCoreStorageService));
            services.AddSingleton(typeof(FolderExplorerUIHandler));

            services.AddSingleton(x => new AbstractFolderExplorer(Services));

            Services = services.BuildServiceProvider();

            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public override void SetupAbstractUISettings()
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
        public async Task<bool> SetupAuthenticationManager(string clientId, string tenantId, string redirectUri = null)
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

            await InitFileExplorer(rootFolder, true);
        }

        private Task InitFileExplorer(IFolderData folder, bool isRoot = false)
        {
            Guard.IsNotNull(Services, nameof(Services));

            var folderExplorerService = Services.GetService<AbstractFolderExplorer>();
            var folderExplorerUIHandler = Services.GetService<FolderExplorerUIHandler>();

            Guard.IsNotNull(folderExplorerService, nameof(folderExplorerService));
            Guard.IsNotNull(folderExplorerUIHandler, nameof(folderExplorerUIHandler));

            _ = folderExplorerService.SetupFolderExplorerAsync(folder, isRoot);

            AttachEvents(folderExplorerUIHandler, folderExplorerService);

            return Task.CompletedTask;
        }

        private void AttachEvents(FolderExplorerUIHandler? folderExplorerUIHandler, AbstractFolderExplorer? folderExplorerService)
        {
            Guard.IsNotNull(folderExplorerService, nameof(folderExplorerService));
            Guard.IsNotNull(folderExplorerUIHandler, nameof(folderExplorerUIHandler));

            folderExplorerUIHandler.FolderExplorerUIUpdated += FolderExplorerUIHandler_FolderExplorerUIUpdated;

            folderExplorerService.FolderSelected += FolderExplorerService_FolderSelected;
        }

        private void FolderExplorerService_FolderSelected(object sender, IFolderData e)
        {
            AbstractUIElements = new List<AbstractUICollection>();

            var folderExplorerService = Services.GetService<AbstractFolderExplorer>();

            Guard.IsNotNull(folderExplorerService, nameof(folderExplorerService));
            folderExplorerService.FolderSelected -= FolderExplorerService_FolderSelected;

            Guard.IsNotNull(_settingsService, nameof(OneDriveCoreSettingsService));
            _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.FolderPath), e.Path);

            SourceCore.Cast<FilesCore>().ChangeCoreState(Sdk.Data.CoreState.Configured);

            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FolderExplorerUIHandler_FolderExplorerUIUpdated(object sender, AbstractUICollection e)
        {
            AbstractUIElements = e.IntoList();

            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
