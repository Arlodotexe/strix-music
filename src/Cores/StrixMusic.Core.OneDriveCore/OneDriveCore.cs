using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.LocalFiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    ///<inheritdoc/>
    public class OneDriveCore : LocalFilesCore
    {
        private ISettingsService? _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public OneDriveCore(string instanceId)
            : base(instanceId)
        {
            CoreConfig = new OneDriveCoreConfig(this);
        }


        /// <inheritdoc/>
        public async override Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(Sdk.Data.CoreState.Loading);

            if (!(CoreConfig is OneDriveCoreConfig coreConfig))
                return;

            _settingsService = new OneDriveCoreSettingsService(InstanceId);

            // TODO: they can happen in parallel.
            var clientId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId));
            var folderPath = await _settingsService.GetValue<string>( nameof(OneDriveCoreSettingsKeys.FolderPath));
            var tenantId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId));
            var redirectUri = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.RedirectUri));

            await coreConfig.SetupConfigurationServices(services);

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(tenantId))
            {
                coreConfig.SetupAbstractUISettings();

                ChangeCoreState(Sdk.Data.CoreState.NeedsSetup);
            }
            else
            {
                var tokenAcquired = await coreConfig.SetupAuthenticationManager(clientId, tenantId, redirectUri);

                if (tokenAcquired)
                {
                    ChangeCoreState(Sdk.Data.CoreState.Configured);
                    return;
                }

                ChangeCoreState(Sdk.Data.CoreState.NeedsSetup);
                coreConfig.SetupAbstractUISettings();
            }


        }
    }
}
