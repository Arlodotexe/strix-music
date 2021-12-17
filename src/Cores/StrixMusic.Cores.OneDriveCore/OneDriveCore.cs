using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Cores.OneDrive
{
    /// <inheritdoc/>
    public sealed class OneDriveCore : FilesCore
    {
        private readonly AbstractButton _completeGenericSetupButton;
        private readonly ILogger<OneDriveCore> _logger;
        private OneDriveCoreSettingsService? _settingsService;
        private INotificationService? _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public OneDriveCore(string instanceId)
            : base(instanceId)
        {
            CoreConfig = new OneDriveCoreConfig(this);

            _logger = Ioc.Default.GetRequiredService<ILogger<OneDriveCore>>();
            _completeGenericSetupButton = new AbstractButton(Guid.NewGuid().ToString(), "OK");
            _completeGenericSetupButton.Clicked += CompleteGenericSetupButton_Clicked;
        }

        /// <inheritdoc/>
        public override CoreMetadata Registration => OneDrive.Registration.Metadata;

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
#warning TODO: Pass cancellationToken from InitAsync when implemented
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);

            ChangeCoreState(CoreState.Loading);

            if (CoreConfig is not OneDriveCoreConfig coreConfig)
                return;

            _logger.LogInformation($"Setting up {nameof(OneDriveCoreSettingsService)}");
            _settingsService = new OneDriveCoreSettingsService(InstanceId);

            _logger.LogInformation($"Getting setting values");
            var clientId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId));
            var tenantId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId));
            var folderId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.SelectedFolderId));
            var firstSetupComplete = await _settingsService.GetValue<bool>(nameof(OneDriveCoreSettingsKeys.IsFirstSetupComplete));

            _logger.LogInformation($"Setting up configuration services");
            await coreConfig.SetupConfigurationServices(services, _settingsService);

            Guard.IsNotNull(coreConfig.Services, nameof(coreConfig.Services));

            _notificationService = coreConfig.Services.GetRequiredService<INotificationService>();

            // Step 1: Settings OOBE
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(tenantId) || !firstSetupComplete)
            {
                _logger.LogInformation($"Resetting all settings");
                await _settingsService.ResetAllAsync();

                _logger.LogInformation($"Triggering setup UI");
                ChangeCoreState(CoreState.NeedsSetup);

                _logger.LogInformation($"Creating OOBE");
                var oobeUI = coreConfig.CreateOutOfBoxSetupAsync();

                var actionButtons = (AbstractUICollection)oobeUI.First(x => x is AbstractUICollection { Id: "actionButtons" });
                var confirmButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Confirm });
                var cancelButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Cancel });

                var oobeCompletionSemaphore = new SemaphoreSlim(0, 1);

                confirmButton.Clicked += OnConfirmClicked;
                cancelButton.Clicked += OnCancelClicked;

                _logger.LogInformation($"Displaying OOBE");
                coreConfig.SaveAbstractUI(oobeUI);

                _logger.LogInformation($"Waiting for completion");
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

            // Step 2: Login
            try
            {
                var loggedIn = await coreConfig.TryLoginAsync(cancellationTokenSource.Token);
                if (!loggedIn)
                {
                    Guard.IsNotNull(_notificationService, nameof(_notificationService));
                    _notificationService.RaiseNotification("Login failed", "An error occurred and we weren't able to log you into OneDrive.");

                    ChangeInstanceDescriptor("Login failed");
                    ChangeCoreState(CoreState.Faulted);
                    return;
                }
            }
            catch (HttpRequestException)
            {
                RaiseFailedConnectionState();
                return;
            }
            catch (OperationCanceledException)
            {
                await _settingsService.ResetAllAsync();
                ChangeCoreState(CoreState.Unloaded);
                return;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(x => x is HttpRequestException))
                {
                    ex.Handle((ex) =>
                    {
                        if (ex is HttpRequestException)
                        {
                            RaiseFailedConnectionState();
                            return true;
                        }

                        return false;
                    });

                    cancellationTokenSource.Cancel();
                    return;
                }
            }

            // Step 3: Folder picking
            if (string.IsNullOrWhiteSpace(folderId))
            {
                ChangeCoreState(CoreState.NeedsSetup);

                _logger.LogInformation($"No folder selected, opening picker.");
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

            _logger.LogInformation($"Getting selected folder {folderId}");
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

            _logger.LogInformation($"Setting up metadata scanner.");
            await coreConfig.SetupMetadataScannerAsync(services, selectedFolder);

            _logger.LogInformation($"Fully configured, setting state.");
            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loaded);

            _logger.LogInformation($"Post config task: setting up generic config UI.");
            var genericConfig = coreConfig.CreateGenericConfig();
            genericConfig.Add(_completeGenericSetupButton);

            coreConfig.SaveAbstractUI(genericConfig);

            _logger.LogInformation($"Initializing library");
            await Library.Cast<FilesCoreLibrary>().InitAsync();

            void RaiseFailedConnectionState()
            {
                _notificationService?.RaiseNotification("Connection failed", "We weren't able to contact OneDrive");

                ChangeInstanceDescriptor("Login failed");
                ChangeCoreState(CoreState.Faulted);
            }
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
