using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive
{
    /// <inheritdoc/>
    public sealed class OneDriveCore : FilesCore
    {
        private readonly IFolderData _metadataStorage;
        private readonly AbstractButton _completeGenericSetupButton;
        private readonly ILogger<OneDriveCore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        /// <param name="settingsStorage">A folder abstraction where this core can persist settings data beyond the lifetime of the application.</param>
        /// <param name="metadataStorage">A folder abstraction where this core can persist metadata for scanned files.</param>
        /// <param name="notificationService">A service that can notify the user with interactive UI or messages.</param>
        /// <param name="launcher">Enables launching a URI in the default system application.</param>
        public OneDriveCore(string instanceId, IFolderData settingsStorage, IFolderData metadataStorage, INotificationService notificationService, ILauncher launcher)
            : base(instanceId)
        {
            _metadataStorage = metadataStorage;
            Launcher = launcher;
            NotificationService = notificationService;
            Settings = new OneDriveCoreSettings(settingsStorage);
            CoreConfig = new OneDriveCoreConfig(this);

            _logger = Ioc.Default.GetRequiredService<ILogger<OneDriveCore>>();
            _completeGenericSetupButton = new AbstractButton(Guid.NewGuid().ToString(), "OK");
            _completeGenericSetupButton.Clicked += CompleteGenericSetupButton_Clicked;
        }

        /// <inheritdoc/>
        public override CoreMetadata Registration { get; } = Metadata;

        /// <summary>
        /// The metadata that identifies this core before instantiation.
        /// </summary>
        public static CoreMetadata Metadata { get; } = new CoreMetadata(id: nameof(OneDriveCore),
                                                                        displayName: "OneDrive",
                                                                        logoUri: new Uri("ms-appx:///Assets/Cores/OneDrive/Logo.svg"),
                                                                        sdkVer: Version.Parse("0.0.0.0"));
        /// <inheritdoc/>
        public override string InstanceDescriptor { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override ICoreConfig CoreConfig { get; protected set; }

        /// <summary>
        /// The message handler to use or requests (wherever possible).
        /// </summary>
        public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();

        /// <summary>
        /// The settings for this core instance.
        /// </summary>
        public OneDriveCoreSettings Settings { get; }

        /// <summary>
        /// Gets a service that can notify the user with interactive UI or generic messages.
        /// </summary>
        internal INotificationService NotificationService { get; }

        /// <summary>
        /// Interface that enables launching a URI. 
        /// </summary>
        internal ILauncher Launcher { get; }

        /// <inheritdoc/>
        public override event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public override event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc/>
        public async override Task InitAsync()
        {
#warning TODO: Pass cancellationToken from InitAsync when implemented
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);

            ChangeCoreState(CoreState.Loading);

            if (CoreConfig is not OneDriveCoreConfig coreConfig)
                return;

            _logger.LogInformation("Getting setting values");
            await Settings.LoadAsync();
            var clientId = Settings.ClientId;
            var tenantId = Settings.TenantId;
            var folderId = Settings.SelectedFolderId;
            var firstSetupComplete = Settings.IsFirstSetupComplete;

            _logger.LogInformation("Setting up configuration services");
            await coreConfig.SetupConfigurationServices();

            // Step 1: Settings OOBE
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(tenantId) || !firstSetupComplete)
            {
                _logger.LogInformation("Resetting all settings");
                var files = await Settings.Folder.GetFilesAsync();
                await files.InParallel(x => x.Delete());
                await Settings.LoadAsync();

                _logger.LogInformation("Triggering setup UI");
                ChangeCoreState(CoreState.NeedsSetup);

                _logger.LogInformation("Creating OOBE");
                var oobeUI = coreConfig.CreateOutOfBoxSetupAsync();

                var actionButtons = (AbstractUICollection)oobeUI.First(x => x is AbstractUICollection { Id: "actionButtons" });
                var confirmButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Confirm });
                var cancelButton = (AbstractButton)actionButtons.First(x => x is AbstractButton { Type: AbstractButtonType.Cancel });

                var oobeCompletionSemaphore = new SemaphoreSlim(0, 1);

                confirmButton.Clicked += OnConfirmClicked;
                cancelButton.Clicked += OnCancelClicked;

                _logger.LogInformation("Displaying OOBE");
                coreConfig.SaveAbstractUI(oobeUI);

                _logger.LogInformation("Waiting for completion");
                await oobeCompletionSemaphore.WaitAsync(cancellationTokenSource.Token);

                return;

                async void OnConfirmClicked(object sender, EventArgs e)
                {
                    confirmButton.Clicked -= OnConfirmClicked;
                    cancelButton.Clicked -= OnCancelClicked;

                    Settings.IsFirstSetupComplete = true;
                    await Settings.SaveAsync();
                    await InitAsync();
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
                    NotificationService.RaiseNotification("Login failed", "An error occurred and we weren't able to log you into OneDrive.");

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
                await Settings.LoadAsync();
                ChangeCoreState(CoreState.Unloaded);
                return;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(x => x is HttpRequestException))
                {
                    ex.Handle(x =>
                    {
                        if (x is not HttpRequestException)
                            return false;

                        RaiseFailedConnectionState();
                        return true;

                    });

                    cancellationTokenSource.Cancel();
                    return;
                }
            }

            // Step 3: Folder picking
            if (string.IsNullOrWhiteSpace(folderId))
            {
                ChangeCoreState(CoreState.NeedsSetup);

                _logger.LogInformation("No folder selected, opening picker.");
                var folder = await coreConfig.PickSingleFolderAsync();
                if (folder is null)
                {
                    // User canceled folder picking.
                    ChangeCoreState(CoreState.Unloaded);
                    return;
                }

                await InitAsync();
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

                await InitAsync();
                return;
            }

            _logger.LogInformation("Setting up metadata scanner.");

            FileMetadataManager = new FileMetadataManager(selectedFolder, _metadataStorage, NotificationService);
            await coreConfig.SetupMetadataScannerAsync();

            _logger.LogInformation("Fully configured, setting state.");
            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loaded);

            _logger.LogInformation("Post config task: setting up generic config UI.");
            var genericConfig = coreConfig.CreateGenericConfig();
            genericConfig.Add(_completeGenericSetupButton);

            coreConfig.SaveAbstractUI(genericConfig);

            _logger.LogInformation("Initializing library");
            await Library.Cast<FilesCoreLibrary>().InitAsync();

            void RaiseFailedConnectionState()
            {
                NotificationService?.RaiseNotification("Connection failed", "We weren't able to contact OneDrive");

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
