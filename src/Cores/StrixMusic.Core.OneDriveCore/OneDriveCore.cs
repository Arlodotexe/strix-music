using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Cores.Files;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    /// <inheritdoc/>
    public sealed class OneDriveCore : FilesCore
    {
        private OneDriveCoreSettingsService? _settingsService;

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
        public override string InstanceDescriptor { get; set; } = string.Empty;

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

            var clientId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId));
            var tenantId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId));
            var folderId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.SelectedFolderId));

            await coreConfig.SetupConfigurationServices(services);

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(tenantId))
            {
                await _settingsService.ResetAllAsync();
                ChangeCoreState(CoreState.NeedsSetup);

                var oobeUI = coreConfig.CreateOutOfBoxSetupAsync();

                var actionButtons = (AbstractUICollection)oobeUI.First(x => x is AbstractUICollection { Id: "actionButtons" });
                var confirmButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Confirm });
                var cancelButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Cancel });

                confirmButton.Clicked += OnConfirmClicked;
                cancelButton.Clicked += OnCancelClicked;

                // Hack, configured should only be emitted if everything is configured. The SDK re-runs InitAsync regardless and our code below will catch that the remaining config isn't set up.
                void OnConfirmClicked(object sender, EventArgs e) => Finished(CoreState.Configured);
                void OnCancelClicked(object sender, EventArgs e) => Finished(CoreState.Unloaded);

                void Finished(CoreState state)
                {
                    confirmButton.Clicked -= OnConfirmClicked;
                    cancelButton.Clicked -= OnCancelClicked;
                    ChangeCoreState(state);
                }

                coreConfig.SaveAbstractUI(oobeUI);
                return;
            }

            var loggedIn = await coreConfig.TryLoginAsync();
            if (!loggedIn)
            {
                ChangeInstanceDescriptor("Login failed");
                ChangeCoreState(CoreState.Faulted);
                return;
            }

            if (string.IsNullOrWhiteSpace(folderId))
            {
                ChangeCoreState(CoreState.NeedsSetup);

                var folder = await coreConfig.PickSingleFolderAsync();
                if (folder is null)
                {
                    ChangeCoreState(CoreState.Unloaded);
                    return;
                }

                ChangeCoreState(CoreState.Configured);
                return;
            }

            var selectedFolder = await coreConfig.GetFolderDataById(folderId);
            if (selectedFolder is null)
            {
                ChangeCoreState(CoreState.NeedsSetup);

                selectedFolder = await coreConfig.PickSingleFolderAsync();
                if (selectedFolder is null)
                {
                    // User canceled folder picking.
                    ChangeCoreState(CoreState.Unloaded);
                    return;
                }

                ChangeCoreState(CoreState.Configured);
                return;
            }

            await coreConfig.SetupMetadataScannerAsync(selectedFolder);
            ChangeCoreState(CoreState.Loaded);
        }

        internal void ChangeCoreState(CoreState state)
        {
            CoreState = state;
            CoreStateChanged?.Invoke(this, state);
        }

        internal void ChangeInstanceDescriptor(string instanceDescriptor)
        {
            InstanceDescriptor = instanceDescriptor;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);
        }
    }
}
