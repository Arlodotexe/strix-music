using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.OneDrive.ConfigPanels;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Cores.OneDrive.Storage;
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

        private AbstractUICollection _configPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        /// <param name="settingsStorage">A folder abstraction where this core can persist settings data beyond the lifetime of the application.</param>
        /// <param name="metadataStorage">A folder abstraction where this core can persist metadata for scanned files.</param>
        /// <param name="notificationService">A service that can notify the user with interactive UI or messages.</param>
        public OneDriveCore(string instanceId, IFolderData settingsStorage, IFolderData metadataStorage, INotificationService notificationService)
            : base(instanceId)
        {
            _metadataStorage = metadataStorage;
            NotificationService = notificationService;
            Settings = new OneDriveCoreSettings(settingsStorage);
            _configPanel = new AbstractUICollection(string.Empty);

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

        /// <inheritdoc />
        public override AbstractUICollection AbstractConfigPanel => _configPanel;

        /// <summary>
        /// The message handler to use or requests (wherever possible).
        /// </summary>
        public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();

        /// <summary>
        /// The settings for this core instance.
        /// </summary>
        public OneDriveCoreSettings Settings { get; }

        /// <summary>
        /// The method that should be used for login.
        /// </summary>
        public LoginMethod LoginMethod { get; set; }

        /// <summary>
        /// Gets a service that can notify the user with interactive UI or generic messages.
        /// </summary>
        internal INotificationService NotificationService { get; }

        /// <summary>
        /// Raised when the user requests to visit an external web page for OneDrive login.
        /// </summary>
        public event EventHandler<Uri>? LoginNavigationRequested;

        /// <inheritdoc/>
        public override event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public override event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc />
        public override event EventHandler? AbstractConfigPanelChanged;

        /// <inheritdoc/>
        public async override Task InitAsync(CancellationToken cancellationToken = default)
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;

            ChangeCoreState(CoreState.Loading);

            _logger.LogInformation("Getting setting values");
            await Settings.LoadAsync();

            using var onCancelledRegistration = cancellationTokenSource.Token.Register(() => ChangeCoreState(CoreState.Unloaded));

            if (string.IsNullOrWhiteSpace(Settings.ClientId) ||
                string.IsNullOrWhiteSpace(Settings.TenantId) ||
                string.IsNullOrWhiteSpace(Settings.RedirectUri) ||
                !Settings.UserHasSeenAuthClientKeysSettings)
            {
                ChangeCoreState(CoreState.NeedsConfiguration);

                var oobePanel = new OutOfBoxExperiencePanel(Settings, NotificationService, cancellationToken);
                _configPanel = oobePanel;
                AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

                await oobePanel.ExecuteCustomAppKeyStageAsync();

                Settings.UserHasSeenAuthClientKeysSettings = true;
                await Settings.SaveAsync();
                return;
            }

            if (!Settings.UserHasSeenGeneralOobeSettings)
            {
                ChangeCoreState(CoreState.NeedsConfiguration);

                var oobePanel = new OutOfBoxExperiencePanel(Settings, NotificationService, cancellationToken);
                _configPanel = oobePanel;
                AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

                await oobePanel.ExecuteSettingsStageAsync();

                Settings.UserHasSeenGeneralOobeSettings = true;
                await Settings.SaveAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(Settings.AccountIdentifier))
            {
                ChangeCoreState(CoreState.NeedsConfiguration);
                try
                {
                    Settings.AccountIdentifier = await AcquireLoginAsync(cancellationToken);
                    await Settings.SaveAsync();
                }
                catch (HttpRequestException)
                {
                    RaiseFailedConnectionState();
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

                return;
            }

            if (string.IsNullOrWhiteSpace(Settings.SelectedFolderId))
            {
                ChangeCoreState(CoreState.NeedsConfiguration);
                var folder = await AcquireUserSelectedFolderAsync(cancellationToken);
                if (folder is null)
                {
                    cancellationTokenSource.Cancel();
                    return;
                }

                Settings.SelectedFolderId = folder.Id ?? string.Empty;
                await Settings.SaveAsync();

                return;
            }

            _logger.LogInformation("Fully configured, setting state.");
            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loading);

            var authManager = new AuthenticationManager(Settings.ClientId, Settings.TenantId, Settings.RedirectUri)
            {
                HttpMessageHandler = HttpMessageHandler,
            };

            var authenticationToken = await authManager.TryAcquireCachedTokenAsync(Settings.AccountIdentifier, cancellationToken);
            Guard.IsNotNull(authenticationToken, nameof(authenticationToken));

            InstanceDescriptor = authenticationToken.Account.Username;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);

            var graphClient = authManager.CreateGraphClient(authenticationToken.AccessToken);

            var driveItem = await graphClient.Drive.Items[Settings.SelectedFolderId].Request().GetAsync(cancellationToken);
            Guard.IsNotNull(driveItem, nameof(driveItem));

            var folderToScan = new OneDriveFolderData(graphClient, driveItem);
            FileMetadataManager = new FileMetadataManager(folderToScan, _metadataStorage, NotificationService);

            // Scanning file contents are possible but extremely slow over the network.
            // The Graph API supplies music metadata from file properties, which is much faster.
            // Use the user's preferences.
            var scanTypes = MetadataScanTypes.None;

            if (Settings.ScanWithTagLib)
                scanTypes |= MetadataScanTypes.TagLib;

            if (Settings.ScanWithFileProperties)
                scanTypes |= MetadataScanTypes.FileProperties;

            FileMetadataManager.ScanTypes = scanTypes;
            FileMetadataManager.DegreesOfParallelism = 8;

            await FileMetadataManager.InitAsync(cancellationToken);
            _ = FileMetadataManager.ScanAsync(cancellationToken);
            ChangeCoreState(CoreState.Loaded);

            _logger.LogInformation("Post config task: setting up generic config UI.");
            var doneButton = new AbstractButton($"{nameof(GeneralCoreConfigPanel)}DoneButton", "Done");
            doneButton.Clicked += GeneralConfigPanelDoneButtonClicked;

            var ui = new GeneralCoreConfigPanel(Settings, NotificationService);
            ui.Add(doneButton);

            _configPanel = ui;
            AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

            _logger.LogInformation("Initializing library");
            await Library.Cast<FilesCoreLibrary>().InitAsync();

            void RaiseFailedConnectionState()
            {
                NotificationService?.RaiseNotification("Connection failed", "We weren't able to contact OneDrive");

                ChangeInstanceDescriptor("Login failed");
            }
        }

        private void GeneralConfigPanelDoneButtonClicked(object sender, EventArgs e)
        {
            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loaded);
        }

        private async Task<IFolderData?> AcquireUserSelectedFolderAsync(CancellationToken cancellationToken)
        {
            var authManager = new AuthenticationManager(Settings.ClientId, Settings.TenantId, Settings.RedirectUri)
            {
                HttpMessageHandler = HttpMessageHandler,
            };

            var authenticationToken = await authManager.TryAcquireCachedTokenAsync(Settings.AccountIdentifier, cancellationToken);
            if (authenticationToken is null)
                return null;

            var graphClient = authManager.CreateGraphClient(authenticationToken.AccessToken);

            var user = await graphClient.Users.Request().GetAsync(cancellationToken);

            var driveItem = await graphClient.Drive.Root.Request().Expand("children").GetAsync(cancellationToken);

            var rootFolder = new OneDriveFolderData(graphClient, driveItem);
            var oobePanel = new OutOfBoxExperiencePanel(Settings, NotificationService, cancellationToken);

            _configPanel = oobePanel;
            AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

            return await oobePanel.ExecuteFolderPickerStageAsync(rootFolder, user.FirstOrDefault()?.DisplayName, Settings.AccountIdentifier);
        }

        private async Task<string> AcquireLoginAsync(CancellationToken cancellationToken)
        {
            var authManager = new AuthenticationManager(Settings.ClientId, Settings.TenantId, Settings.RedirectUri)
            {
                HttpMessageHandler = HttpMessageHandler,
            };

            var authenticationToken = await authManager.TryAcquireCachedTokenAsync(Settings.AccountIdentifier, cancellationToken);
            if (authenticationToken is null)
            {
                var oobePanel = new OutOfBoxExperiencePanel(Settings, NotificationService, cancellationToken);
                _configPanel = oobePanel;
                AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

                if (LoginMethod == LoginMethod.Interactive)
                {
                    oobePanel.DisplayInteractiveLoginStageAsync();
                    authenticationToken = await authManager.TryAcquireTokenViaInteractiveLoginAsync(cancellationToken);
                }
                else if (LoginMethod == LoginMethod.DeviceCode)
                {
                    var deviceCodePanel = oobePanel.DisplayDeviceCodeLoginStageAsync();

                    deviceCodePanel.AuthenticateButton.Clicked += AuthenticateButtonOnClicked;

                    authenticationToken = await authManager.TryAcquireTokenViaDeviceCodeLoginAsync(x =>
                    {
                        deviceCodePanel.VerificationUri = new Uri(x.VerificationUrl);
                        deviceCodePanel.Code = x.UserCode;
                        return Task.CompletedTask;
                    }, cancellationToken);

                    deviceCodePanel.AuthenticateButton.Clicked -= AuthenticateButtonOnClicked;

                    void AuthenticateButtonOnClicked(object sender, EventArgs e) => LoginNavigationRequested?.Invoke(this, deviceCodePanel.VerificationUri ?? ThrowHelper.ThrowArgumentNullException<Uri>(nameof(deviceCodePanel.VerificationUri)));
                }
                else
                    throw new ArgumentOutOfRangeException();
            }

            Guard.IsNotNull(authenticationToken, nameof(authenticationToken));

            Guard.IsNotNullOrWhiteSpace(authenticationToken.Account.Username, nameof(authenticationToken.Account.Username));
            return authenticationToken.Account.HomeAccountId.Identifier;
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
