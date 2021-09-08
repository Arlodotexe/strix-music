using StrixMusic.Core.LocalFiles;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Text;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.OneDriveCore.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Core.OneDriveCore.Storage;
using StrixMusic.Sdk.Components;

namespace StrixMusic.Core.OneDriveCore
{
    ///  <inheritdoc/>
    public class OneDriveCoreConfig : LocalFilesCoreConfig
    {
        private AbstractTextBox _clientIdTb;
        private AbstractTextBox _tenantTb;
        private AbstractTextBox _redirectUriTb;

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
            services.AddScoped(typeof(AuthenticationManager));
            services.AddScoped(typeof(OneDriveCoreStorageService));
            services.AddScoped(x => new DefaultFileExplorer(Services));

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

            AbstractUIElements = new List<AbstractUIElementGroup>
            {
                new AbstractUIElementGroup("SettingsGroup")
                {
                    Title="OneDrive Settings.",

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
            Guard.IsNotNull(Services, "Services are null.");

            var authManager = Services.GetService<AuthenticationManager>();

            if (!string.IsNullOrWhiteSpace(_clientIdTb.Value) && !string.IsNullOrWhiteSpace(_tenantTb.Value))
            {
                Guard.IsNotNull(authManager, nameof(authManager));
                authManager.Init(_clientIdTb.Value, _tenantTb.Value, _redirectUriTb.Value);

                var client = await authManager.GenerateGraphToken();

                var oneDriveService = Services.GetService<OneDriveCoreStorageService>();
                Guard.IsNotNull(oneDriveService, nameof(oneDriveService));

                oneDriveService.Init(client);

                var rootFolder = await oneDriveService.GetRootFolderAsync();

                await UpdateSettingsUI(rootFolder, true);
            }
            else
            {
                // TODO: Show error.
            }
        }

        private async Task UpdateSettingsUI(IFolderData folder, bool isRoot = false)
        {
            Guard.IsNotNull(Services, nameof(Services));

            var fileExplorerService = Services.GetService<DefaultFileExplorer>();

            Guard.IsNotNull(fileExplorerService, nameof(fileExplorerService));

            var abstractUIElementGroup = await fileExplorerService.SetupFileExplorerAsync(folder, isRoot);

            AbstractUIElements = new List<AbstractUIElementGroup>
            {
                abstractUIElementGroup
            };

            Guard.IsNotNull(abstractUIElementGroup, nameof(abstractUIElementGroup));

            fileExplorerService.DirectoryChanged += DataList_ItemTapped;

            AbstractUIElementChanged();
        }

        private async void DataList_ItemTapped(object sender, AbstractUIMetadata e)
        {
            // TODO: Move this logic to the fileExplorer.

            ((AbstractDataList)sender).ItemTapped -= DataList_ItemTapped;

            Guard.IsNotNull(Services, "Services is null.");

            var fileExplorerService = Services.GetService<DefaultFileExplorer>();

            Guard.IsNotNull(fileExplorerService, nameof(fileExplorerService));

            Guard.IsNotNull(fileExplorerService.CurrentFolder, nameof(fileExplorerService.CurrentFolder));

            IFolderData targetFolder;

            if (!e.Id.Equals(fileExplorerService.BackBtnId))
            {
                targetFolder = await fileExplorerService.CurrentFolder.GetFolderAsync(e.Title);
            }
            else
            {
                Guard.IsNotNull(fileExplorerService.PreviousFolder, nameof(fileExplorerService.PreviousFolder));

                targetFolder = fileExplorerService.PreviousFolder;
            }

            if (targetFolder is OneDriveFolderData oneDriveFolder)
            {
                await UpdateSettingsUI(targetFolder, oneDriveFolder.IsRoot);
            }
            else Guard.IsNotOfType<OneDriveFolderData>(targetFolder, nameof(targetFolder));
        }
    }
}
