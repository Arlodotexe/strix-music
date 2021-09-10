using StrixMusic.Core.LocalFiles;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.OneDrive.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Cores.OneDrive.Storage;
using StrixMusic.Sdk.Components;
using StrixMusic.Sdk.Components.Explorers;

namespace StrixMusic.Cores.OneDrive
{
    ///  <inheritdoc/>
    public class OneDriveCoreConfig : LocalFilesCoreConfig
    {
        private AbstractTextBox? _clientIdTb;
        private AbstractTextBox? _tenantTb;
        private AbstractTextBox? _redirectUriTb;

        /// <inheritdoc/>
        public override event EventHandler? AbstractUIElementsChanged;


        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreConfig"/> class.
        /// </summary>
        public OneDriveCoreConfig(ICore sourceCore)
            : base(sourceCore)
        {
            SetupAbstractUISettings();
        }

        public override Task SetupConfigurationServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(AuthenticationManager));
            services.AddSingleton(typeof(OneDriveCoreStorageService));
            services.AddSingleton(typeof(FolderExplorerUIHandler));
            services.AddSingleton(new FolderExplorer(Services));

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
        }

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            Guard.IsNotNull(Services, nameof(Services));
            Guard.IsNotNull(_clientIdTb, nameof(_clientIdTb));
            Guard.IsNotNull(_tenantTb, nameof(_tenantTb));
            Guard.IsNotNull(_redirectUriTb, nameof(_redirectUriTb));

            if (string.IsNullOrWhiteSpace(_clientIdTb.Value) || string.IsNullOrWhiteSpace(_tenantTb.Value))
                return;

            var authManager = Services.GetService<AuthenticationManager>();

            Guard.IsNotNull(authManager, nameof(authManager));
            authManager.Init(_clientIdTb.Value, _tenantTb.Value, _redirectUriTb.Value);

            var client = await authManager.GenerateGraphToken();

            var oneDriveService = Services.GetService<OneDriveCoreStorageService>();
            Guard.IsNotNull(oneDriveService, nameof(oneDriveService));

            oneDriveService.Init(client);

            var rootFolder = await oneDriveService.GetRootFolderAsync();

            await InitFileExplorer(rootFolder, true);
        }

        private Task InitFileExplorer(IFolderData folder, bool isRoot = false)
        {
            Guard.IsNotNull(Services, nameof(Services));

            var folderExplorerService = Services.GetService<FolderExplorer>();
            var folderExplorerUIHandler = Services.GetService<FolderExplorerUIHandler>();

            Guard.IsNotNull(folderExplorerService, nameof(folderExplorerService));
            Guard.IsNotNull(folderExplorerUIHandler, nameof(folderExplorerUIHandler));

            _ = folderExplorerService.SetupFolderExplorerAsync(folder, isRoot);

            AttachEvents(folderExplorerUIHandler, folderExplorerService);

            return Task.CompletedTask;
        }

        private void AttachEvents(FolderExplorerUIHandler? folderExplorerUIHandler, FolderExplorer? folderExplorerService)
        {
            Guard.IsNotNull(folderExplorerService, nameof(folderExplorerService));
            Guard.IsNotNull(folderExplorerUIHandler, nameof(folderExplorerUIHandler));

            folderExplorerUIHandler.FolderExplorerUIUpdated += FolderExplorerUIHandler_FolderExplorerUIUpdated;

            folderExplorerService.FolderSelected += FolderExplorerService_FolderSelected;
        }

        private void FolderExplorerService_FolderSelected(object sender, IFolderData e)
        {
            AbstractUIElements = new List<AbstractUICollection>();

            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FolderExplorerUIHandler_FolderExplorerUIUpdated(object sender, AbstractUICollection e)
        {
            AbstractUIElements = e.IntoList();

            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
