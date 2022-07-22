using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Services;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.OneDrive.ConfigPanels;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Cores.OneDrive.Storage;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive
{
    /// <summary>
    /// Scan and play audio files from OneDrive.
    /// </summary>
    public sealed class OneDriveCore : FilesCore
    {
        private readonly IFolderData _metadataStorage;
        private readonly AbstractButton _completeGenericSetupButton;

        private AbstractUICollection _configPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        /// <param name="settingsStorage">A folder abstraction where this core can persist settings data beyond the lifetime of the application.</param>
        /// <param name="metadataStorage">A folder abstraction where this core can persist metadata for scanned files.</param>
        /// <param name="notificationService">A service that can notify the user with interactive UI or messages.</param>
        public OneDriveCore(string instanceId, IFolderData settingsStorage, IFolderData metadataStorage, INotificationService notificationService, Progress<FileScanState>? fileScanProgress)
            : this(instanceId, new OneDriveCoreSettings(settingsStorage), metadataStorage, notificationService, fileScanProgress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <remarks>
        /// This overload allows passing preconfigured settings that, if all values are valid, will allow initialization to complete without 
        /// any interaction from the user.
        /// </remarks>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        /// <param name="settings">A preconfigured instance of <see cref="OneDriveCoreSettings"/> that will be used instead of a new instance with default values.</param>
        /// <param name="metadataStorage">A folder abstraction where this core can persist metadata for scanned files.</param>
        /// <param name="notificationService">A service that can notify the user with interactive UI or messages.</param>
        public OneDriveCore(string instanceId, OneDriveCoreSettings settings, IFolderData metadataStorage, INotificationService notificationService, Progress<FileScanState>? fileScanProgress = null)
            : base(instanceId, fileScanProgress)
        {
            NotificationService = notificationService;
            _metadataStorage = metadataStorage;
            Settings = settings;
            _configPanel = new AbstractUICollection(string.Empty);

            _completeGenericSetupButton = new AbstractButton(Guid.NewGuid().ToString(), "OK");
            _completeGenericSetupButton.Clicked += CompleteGenericSetupButton_Clicked;
        }

        /// <inheritdoc/>
        public override CoreMetadata Registration { get; } = Metadata;

        /// <summary>
        /// The metadata that identifies this core before instantiation.
        /// </summary>
        public static CoreMetadata Metadata { get; } = new CoreMetadata(Id: nameof(OneDriveCore),
                                                                        DisplayName: "OneDrive",
                                                                        LogoUri: new Uri("ms-appx:///Assets/Cores/OneDrive/Logo.svg"),
                                                                        SdkVer: typeof(ICore).Assembly.GetName().Version);
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
        /// A service that can notify the user with interactive UI or messages.
        /// </summary>
        public INotificationService NotificationService { get; }

        /// <summary>
        /// Raised when the user requests to visit an external web page for OneDrive login.
        /// </summary>
        public event EventHandler<Uri>? LoginNavigationRequested;

        /// <inheritdoc cref="MsalPublicClientApplicationBuilderCreatedEventArgs" />
        public event EventHandler<MsalPublicClientApplicationBuilderCreatedEventArgs>? MsalPublicClientApplicationBuilderCreated;

        /// <inheritdoc cref="AcquireTokenInteractiveParameterBuilderCreatedEventArgs" />
        public event EventHandler<AcquireTokenInteractiveParameterBuilderCreatedEventArgs>? MsalAcquireTokenInteractiveParameterBuilderCreated;

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

            Logger.LogInformation("Getting setting values");
            await Settings.LoadAsync();

            using var onCancelledRegistration = cancellationTokenSource.Token.Register(() => ChangeCoreState(CoreState.Unloaded));

            if (string.IsNullOrWhiteSpace(Settings.ClientId) ||
                string.IsNullOrWhiteSpace(Settings.TenantId) ||
                string.IsNullOrWhiteSpace(Settings.RedirectUri) ||
                !Settings.UserHasSeenAuthClientKeysSettings)
            {
                Logger.LogInformation("Need custom app key values");
                ChangeCoreState(CoreState.NeedsConfiguration);

                var oobePanel = new OutOfBoxExperiencePanel(Settings, NotificationService, cancellationToken);
                _configPanel = oobePanel;
                AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

                await oobePanel.ExecuteCustomAppKeyStageAsync();

                Settings.UserHasSeenAuthClientKeysSettings = true;
                await Settings.SaveAsync();
            }

            if (!Settings.UserHasSeenGeneralOobeSettings)
            {
                Logger.LogInformation("Displaying general OOBE settings");
                ChangeCoreState(CoreState.NeedsConfiguration);

                var oobePanel = new OutOfBoxExperiencePanel(Settings, NotificationService, cancellationToken);
                _configPanel = oobePanel;
                AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

                await oobePanel.ExecuteSettingsStageAsync();

                Settings.UserHasSeenGeneralOobeSettings = true;
                await Settings.SaveAsync();
            }

            if (string.IsNullOrWhiteSpace(Settings.AccountIdentifier))
            {
                Logger.LogInformation("User needs to login");
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
            }

            if (string.IsNullOrWhiteSpace(Settings.SelectedFolderId))
            {
                Logger.LogInformation("User needs to pick folder");
                ChangeCoreState(CoreState.NeedsConfiguration);
                var folder = await AcquireUserSelectedFolderAsync(cancellationToken);
                if (folder is null)
                {
                    cancellationTokenSource.Cancel();
                    return;
                }

                Settings.SelectedFolderId = folder.Id ?? string.Empty;
                await Settings.SaveAsync();
            }

            Logger.LogInformation("Fully configured, setting state.");
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

            // Scanning file contents are possible but extremely slow over the network.
            // The Graph API supplies music metadata from file properties, which is much faster.
            // Use the user's preferences.
            var scanTypes = MetadataScanTypes.None;

            if (Settings.ScanWithTagLib)
                scanTypes |= MetadataScanTypes.TagLib;

            if (Settings.ScanWithFileProperties)
                scanTypes |= MetadataScanTypes.FileProperties;

            var folderToScan = new OneDriveFolderData(graphClient, driveItem);
            FileMetadataManager = new FileMetadataManager(folderToScan, _metadataStorage, FileScanProgress, degreesOfParallelism: 8);

            FileMetadataManager.ScanTypes = scanTypes;

            await FileMetadataManager.InitAsync(cancellationToken);

            var scannerTask = FileMetadataManager.ScanAsync(cancellationToken);

            if (ScannerWaitBehavior == ScannerWaitBehavior.AlwaysWait)
                await scannerTask;

            if (ScannerWaitBehavior == ScannerWaitBehavior.WaitIfNoData)
            {
                var itemCounts = await Task.WhenAll(FileMetadataManager.Tracks.GetItemCount(), FileMetadataManager.Albums.GetItemCount(), FileMetadataManager.AlbumArtists.GetItemCount(), FileMetadataManager.Playlists.GetItemCount());

                if (itemCounts.Sum() == 0)
                    await scannerTask;
            }

            ChangeCoreState(CoreState.Loaded);

            Logger.LogInformation("Post config task: setting up generic config UI.");
            var doneButton = new AbstractButton($"{nameof(GeneralCoreConfigPanel)}DoneButton", "Done");
            doneButton.Clicked += GeneralConfigPanelDoneButtonClicked;

            var ui = new GeneralCoreConfigPanel(Settings, NotificationService)
            {
                doneButton
            };

            _configPanel = ui;
            AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

            Logger.LogInformation("Initializing library");
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

            authManager.MsalAcquireTokenInteractiveParameterBuilderCreated += OnInteractiveParamBuilderCreated;
            authManager.MsalPublicClientApplicationBuilderCreated += OnPublicClientApplicationBuilderCreated;

            var authenticationToken = await authManager.TryAcquireCachedTokenAsync(Settings.AccountIdentifier, cancellationToken);
            if (authenticationToken is null)
                return null;

            var graphClient = authManager.CreateGraphClient(authenticationToken.AccessToken);

            authManager.MsalAcquireTokenInteractiveParameterBuilderCreated -= OnInteractiveParamBuilderCreated;
            authManager.MsalPublicClientApplicationBuilderCreated -= OnPublicClientApplicationBuilderCreated;

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

            authManager.MsalAcquireTokenInteractiveParameterBuilderCreated += OnInteractiveParamBuilderCreated;
            authManager.MsalPublicClientApplicationBuilderCreated += OnPublicClientApplicationBuilderCreated;

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

            authManager.MsalAcquireTokenInteractiveParameterBuilderCreated -= OnInteractiveParamBuilderCreated;
            authManager.MsalPublicClientApplicationBuilderCreated -= OnPublicClientApplicationBuilderCreated;

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

        private void OnInteractiveParamBuilderCreated(object sender, AcquireTokenInteractiveParameterBuilderCreatedEventArgs args) => MsalAcquireTokenInteractiveParameterBuilderCreated?.Invoke(this, args);

        private void OnPublicClientApplicationBuilderCreated(object sender, MsalPublicClientApplicationBuilderCreatedEventArgs args) => MsalPublicClientApplicationBuilderCreated?.Invoke(this, args);
    }
}
