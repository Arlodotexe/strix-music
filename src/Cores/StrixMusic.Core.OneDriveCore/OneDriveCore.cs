using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Cores.Files;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Cores.OneDrive
{
    /// <inheritdoc/>
    public sealed class OneDriveCore : FilesCore
    {
        private readonly AbstractButton _completeGenericSetupButton;
        private OneDriveCoreSettingsService? _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public OneDriveCore(string instanceId)
            : base(instanceId)
        {
            CoreConfig = new OneDriveCoreConfig(this);

            _completeGenericSetupButton = new AbstractButton(Guid.NewGuid().ToString(), "OK");
            _completeGenericSetupButton.Clicked += CompleteGenericSetupButton_Clicked;
        }

        /// <inheritdoc/>
        public override string CoreRegistryId => nameof(OneDriveCore);

        /// <inheritdoc/>
        public override string InstanceDescriptor { get; set; } = string.Empty;

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

            var clientId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId));
            var tenantId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId));
            var folderId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.SelectedFolderId));
            var firstSetupComplete = await _settingsService.GetValue<bool>(nameof(OneDriveCoreSettingsKeys.IsFirstSetupComplete));

            await coreConfig.SetupConfigurationServices(services);

            // Show very first OOBE and allow the user to change settings before picking a folder.
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(tenantId) || !firstSetupComplete)
            {
                await _settingsService.ResetAllAsync();
                ChangeCoreState(CoreState.NeedsSetup);

                var oobeUI = coreConfig.CreateOutOfBoxSetupAsync();

                var actionButtons = (AbstractUICollection)oobeUI.First(x => x is AbstractUICollection { Id: "actionButtons" });
                var confirmButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Confirm });
                var cancelButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Cancel });

                var oobeCompletionSemaphore = new SemaphoreSlim(0, 1);

                confirmButton.Clicked += OnConfirmClicked;
                cancelButton.Clicked += OnCancelClicked;

                coreConfig.SaveAbstractUI(oobeUI);

                await oobeCompletionSemaphore.WaitAsync();

                return;

                async void OnConfirmClicked(object sender, EventArgs e)
                {
                    confirmButton.Clicked -= OnConfirmClicked;
                    cancelButton.Clicked -= OnCancelClicked;

                    await _settingsService.SetValue<bool>(nameof(OneDriveCoreSettingsKeys.IsFirstSetupComplete), true);
                    await InitAsync(services);
                    oobeCompletionSemaphore.Release();
                }

                void OnCancelClicked(object sender, EventArgs e)
                {
                    confirmButton.Clicked -= OnConfirmClicked;
                    cancelButton.Clicked -= OnCancelClicked;

                    ChangeCoreState(CoreState.Unloaded);
                    oobeCompletionSemaphore.Release();
                }
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
                    // User canceled folder picking.
                    ChangeCoreState(CoreState.Unloaded);
                    return;
                }

                await InitAsync(services);
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

                await InitAsync(services);
                return;
            }

            await coreConfig.SetupMetadataScannerAsync(services, selectedFolder);

            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loaded);

            var genericConfig = coreConfig.CreateGenericConfig();
            genericConfig.Add(_completeGenericSetupButton);

            coreConfig.SaveAbstractUI(genericConfig);

            await Library.Cast<FilesCoreLibrary>().InitAsync();
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

        private void CompleteGenericSetupButton_Clicked(object sender, EventArgs e)
        {
            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loaded);
        }

        public override ValueTask DisposeAsync()
        {
            _completeGenericSetupButton.Clicked -= CompleteGenericSetupButton_Clicked;
            return base.DisposeAsync();
        }
    }
}
