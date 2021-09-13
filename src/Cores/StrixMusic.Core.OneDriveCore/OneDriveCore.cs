using System;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Cores.Files;
using System.Threading.Tasks;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    /// <inheritdoc/>
    public sealed class OneDriveCore : FilesCore
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
        public override ICoreConfig CoreConfig { get; protected set; }

        /// <inheritdoc/>
        public override event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public override event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc/>
        public override async Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(CoreState.Loading);

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
                coreConfig.SetupAbstractUIForConfig();
                ChangeCoreState(CoreState.NeedsSetup);
            }
            else
            {
                var tokenAcquired = await coreConfig.SetupAuthenticationManager(clientId, tenantId, redirectUri);

                if (tokenAcquired)
                {
                    ChangeCoreState(CoreState.Configured);
                    return;
                }

                ChangeCoreState(CoreState.NeedsSetup);
                coreConfig.SetupAbstractUIForConfig();
            }
        }

        internal void ChangeCoreState(CoreState state)
        {
            CoreState = state;
            CoreStateChanged?.Invoke(this, state);
        }
    }
}
