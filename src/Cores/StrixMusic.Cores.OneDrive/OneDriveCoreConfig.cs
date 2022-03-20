using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Components;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Cores.OneDrive.Storage;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.FileMetadata;

namespace StrixMusic.Cores.OneDrive
{
    ///  <inheritdoc/>
    public sealed class OneDriveCoreConfig : ICoreConfig
    {
        private readonly OneDriveCore _sourceCore;
        private readonly ILogger<OneDriveCoreConfig> _logger;
        private readonly AbstractBoolean _useTagLibScannerToggle;
        private readonly AbstractBoolean _useFilePropsScannerToggle;
        private Notification? _scannerRequiredNotification;

        private bool _isConfigServicesSetup;

        private GraphServiceClient? _graphClient;
        private IFolderData? _rootFolder;

        private OneDriveCoreSettings _settings;
        private INotificationService _notificationService;
        private AuthenticationManager? _authenticationManager;
        private readonly ILauncher _launcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreConfig"/> class.
        /// </summary>
        public OneDriveCoreConfig(OneDriveCore sourceCore)
        {
            _sourceCore = sourceCore;
            _settings = sourceCore.Settings;
            _notificationService = sourceCore.NotificationService;
            _launcher = sourceCore.Launcher;

            _logger = Ioc.Default.GetRequiredService<ILogger<OneDriveCoreConfig>>();

            _useTagLibScannerToggle = new AbstractBoolean("useTagLibScannerToggle", "Use TagLib")
            {
                Subtitle = "TagLib is more accurate and supports more formats, but is slower (not recommended).",
            };

            _useFilePropsScannerToggle = new AbstractBoolean("useFilePropsScannerToggle", "Use file properties")
            {
                Subtitle = "File properties are very fast, but provide less data.",
            };
        }

        private void AttachEvents()
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.PropertyChanged += OnSettingChanged;
            _useTagLibScannerToggle.StateChanged += UseTagLibScannerToggleOnStateChanged;
            _useFilePropsScannerToggle.StateChanged += UseFilePropsScannerToggleOnStateChanged;
        }

        private void DetachEvents()
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.PropertyChanged -= OnSettingChanged;
            _useTagLibScannerToggle.StateChanged -= UseTagLibScannerToggleOnStateChanged;
            _useFilePropsScannerToggle.StateChanged -= UseFilePropsScannerToggleOnStateChanged;
        }

        /// <inheritdoc />
        public AbstractUICollection AbstractUIElements { get; private set; } = new(string.Empty);

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc />
        public ICore SourceCore => _sourceCore;

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        public async Task SetupConfigurationServices()
        {
            _logger.LogInformation("Setting up configuration services");

            // _isConfigServicesSetup is a flag that tells us if setup is happening or needs to happen.
            // AttachEvents and DetachEvents here are only for changed settings that are reflected in abstractui.
            // TODO: improve this flow. The event sub/unsub shouldn't be loosely tied to a bool like this.
            if (_isConfigServicesSetup)
                DetachEvents();

            _isConfigServicesSetup = false;

            _logger.LogInformation("Getting initial setting states");
            await _settings.LoadAsync();
            _useFilePropsScannerToggle.State = _settings.ScanWithFileProperties;
            _useTagLibScannerToggle.State = _settings.ScanWithTagLib;

            AttachEvents();
            _isConfigServicesSetup = true;
        }

        public AbstractUICollection CreateGenericConfig()
        {
            var metadataScanType = new AbstractUICollection("metadataScanType")
            {
                _useTagLibScannerToggle,
                _useFilePropsScannerToggle,
            };

            metadataScanType.Title = "Scanner type";
            metadataScanType.Subtitle = "Requires restart.";

            var genericConfig = new AbstractUICollection("GenericConfig")
            {
                Title = "OneDrive settings",
            };

            genericConfig.Add(metadataScanType);
            
            return genericConfig;
        }

        /// <summary>
        /// Shows the out of box setup page.
        /// </summary>
        /// <remarks>
        /// This heavily uses local function scopes to avoid making a new class. To "Dispose" and detach events / prevent memory leaks, either the continue or cancel button must be "Clicked".
        /// </remarks>
        /// <returns>An <see cref="AbstractUICollection"/> containing all the interactive AbstractUI components needed for out of box setup.</returns>
        public AbstractUICollection CreateOutOfBoxSetupAsync()
        {
            var showAdvanced = new AbstractBoolean("showAdvanced", "Show advanced");
            var advancedCollection = new AbstractUICollection("advancedSettings")
            {
                Title = "Advanced",
            };

            advancedCollection.Add(showAdvanced);
            
            var continueButton = new AbstractButton("continueButton", "Continue", type: AbstractButtonType.Confirm);
            var cancelButton = new AbstractButton("cancelButton", "Cancel", type: AbstractButtonType.Cancel);
            var actionButtons = new AbstractUICollection("actionButtons", PreferredOrientation.Horizontal)
            {
                cancelButton,
                continueButton,
            };

            var ui = new AbstractUICollection("SettingsGroup")
            {
                CreateGenericConfig(),
#if DEBUG
                advancedCollection,
#endif
                actionButtons,
            };

            showAdvanced.StateChanged += OnShowAdvancedClicked;
            continueButton.Clicked += OnNavigationButtonClicked;
            cancelButton.Clicked += OnNavigationButtonClicked;

            void OnNavigationButtonClicked(object sender, EventArgs e) => DetachOutOfBoxUIEvents();

            void DetachOutOfBoxUIEvents()
            {
                Guard.IsNotNull(cancelButton, nameof(cancelButton));
                Guard.IsNotNull(showAdvanced, nameof(showAdvanced));

                continueButton.Clicked -= OnNavigationButtonClicked;
                cancelButton.Clicked -= OnNavigationButtonClicked;

                // Detaches any events related to advanced settings.
                if (showAdvanced.State)
                    showAdvanced.State = false;

                showAdvanced.StateChanged -= OnShowAdvancedClicked;
            }

            void OnShowAdvancedClicked(object sender, bool newState)
            {
                if (showAdvanced.State)
                    InjectAdvancedSettings();
                else
                    RemoveAdvancedSettings();

                SaveAbstractUI(ui);
            }

            void RemoveAdvancedSettings()
            {
                Guard.IsNotNull(advancedCollection, nameof(advancedCollection));

                foreach (var item in advancedCollection.ToArray())
                {
                    if (item != showAdvanced)
                        ((ICollection<AbstractUIElement>)advancedCollection).Remove(item);
                }
            }

            void InjectAdvancedSettings()
            {
                Guard.IsNotNull(showAdvanced, nameof(showAdvanced));
                Guard.IsNotNull(continueButton, nameof(continueButton));
                Guard.IsNotNull(advancedCollection, nameof(advancedCollection));

                var clientIdTb = new AbstractTextBox("ClientId", string.Empty, "Enter custom client id");
                var tenantTb = new AbstractTextBox("Tenant Id", string.Empty, "Enter custom tenant id");
                var redirectUriTb = new AbstractTextBox("Redirect Uri", string.Empty, "Enter custom redirect uri (if any)");

                clientIdTb.ValueChanged += OnTextBoxChanged;
                tenantTb.ValueChanged += OnTextBoxChanged;
                redirectUriTb.ValueChanged += OnTextBoxChanged;
                showAdvanced.StateChanged += OnAdvancedTurnedOff;

                advancedCollection.Add(clientIdTb);
                advancedCollection.Add(tenantTb);
                advancedCollection.Add(redirectUriTb);

                async void OnTextBoxChanged(object sender, string value)
                {
                    Guard.IsNotNull(_settings, nameof(_settings));

                    if (sender == clientIdTb)
                        _settings.ClientId = value.Trim();

                    if (sender == tenantTb)
                        _settings.TenantId = value.Trim();

                    if (sender == redirectUriTb)
                        _settings.RedirectUri = value.Trim();

                    await _settings.SaveAsync();
                }

                void OnAdvancedTurnedOff(object sender, bool newState)
                {
                    clientIdTb.ValueChanged -= OnTextBoxChanged;
                    tenantTb.ValueChanged -= OnTextBoxChanged;
                    redirectUriTb.ValueChanged -= OnTextBoxChanged;
                    showAdvanced.StateChanged -= OnAdvancedTurnedOff;
                }
            }

            return ui;
        }

        public async Task<IFolderData?> GetFolderDataById(string id)
        {
            Guard.IsNotNull(_graphClient, nameof(_graphClient));

            var driveItem = await _graphClient.Drive.Items[id].Request().GetAsync();
            if (driveItem is null)
                return null;

            return new OneDriveFolderData(_graphClient, driveItem);
        }

        /// <summary>
        /// Logs the user in.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task<bool> TryLoginAsync(CancellationToken? cancellationToken = null)
        {
            _logger.LogInformation($"Entered {nameof(TryLoginAsync)}");
            
            var clientId = _settings.ClientId;
            var tenantId = _settings.TenantId;
            var redirectUri = _settings.RedirectUri;

            _logger.LogInformation($"Initializing authentication manager");
            _authenticationManager = new AuthenticationManager(this, clientId, tenantId, redirectUri);

            _logger.LogInformation($"Trying login");
            _graphClient = await _authenticationManager.TryLoginAsync(cancellationToken ?? CancellationToken.None);

            if (_graphClient is null)
                return false;

            SourceCore.Cast<OneDriveCore>().ChangeInstanceDescriptor(_authenticationManager.EmailAddress ?? string.Empty);
            return true;
        }

        public async Task SetupMetadataScannerAsync()
        {
            Guard.IsNotNull(_sourceCore.FileMetadataManager, nameof(_sourceCore.FileMetadataManager));
            
            // Scanning file contents are possible but extremely slow over the network.
            // The Graph API supplies music metadata from file properties, which is much faster.
            // Use the user's preferences.
            var scanTypes = MetadataScanTypes.None;

            if (_settings.ScanWithTagLib)
                scanTypes |= MetadataScanTypes.TagLib;

            if (_settings.ScanWithFileProperties)
                scanTypes |= MetadataScanTypes.FileProperties;

            _sourceCore.FileMetadataManager.ScanTypes = scanTypes;
            _sourceCore.FileMetadataManager.DegreesOfParallelism = 8;

            await _sourceCore.FileMetadataManager.InitAsync();
            Task.Run(_sourceCore.FileMetadataManager.StartScan).Forget();
        }

        public async Task<IFolderData?> PickSingleFolderAsync()
        {
            _logger.LogInformation($"Started setup for folder picker");
            Guard.IsNotNull(_graphClient, nameof(_graphClient));
            Guard.IsNotNull(_authenticationManager, nameof(_authenticationManager));
            Guard.IsNotNull(_settings, nameof(_settings));
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            // Get root OneDrive folder.
            _logger.LogInformation($"Getting root folder");
            var driveItem = await _graphClient.Drive.Root.Request().Expand("children").GetAsync();

            _rootFolder = new OneDriveFolderData(_graphClient, driveItem);
            _logger.LogInformation($"Root AbstractStorage folder created {_rootFolder.Id}");

            // Setup folder explorer
            _logger.LogInformation($"Creating abstract folder explorer");
            var fileExplorer = new AbstractFolderExplorer(_rootFolder);

            if (!string.IsNullOrWhiteSpace(_authenticationManager.DisplayName))
            {
                fileExplorer.Title = $"{_authenticationManager.DisplayName}'s OneDrive";
            }
            else if (!string.IsNullOrWhiteSpace(_authenticationManager.EmailAddress))
            {
                fileExplorer.Title = $"OneDrive";
                fileExplorer.Subtitle = _authenticationManager.EmailAddress;
            }
            else
            {
                fileExplorer.Title = $"OneDrive";
            }

            _logger.LogInformation($"Initializing abstract folder explorer");
            await fileExplorer.InitAsync();

            // Show folder explorer
            _logger.LogInformation($"Displaying abstract folder explorer");
            AbstractUIElements = fileExplorer;
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);

            var taskCompletionSource = new TaskCompletionSource<IFolderData?>();

            fileExplorer.DirectoryChanged += OnDirectoryChanged;
            fileExplorer.NavigationFailed += OnNavigationFailed;

            // Wait until the user has picked a file.
            var folderSelectionCancellationTokenSource = new CancellationTokenSource();
            folderSelectionCancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(30));

            fileExplorer.FolderSelected += OnFolderSelected;
            fileExplorer.Canceled += OnFileExplorerCanceled;

            _logger.LogInformation($"Waiting for the user to pick a file.");
            var result = await taskCompletionSource.Task;
            _logger.LogInformation($"Folder picked: {result?.Id ?? "none"}");

            fileExplorer.DirectoryChanged -= OnDirectoryChanged;
            fileExplorer.NavigationFailed -= OnNavigationFailed;
            fileExplorer.Dispose();

            _settings.SelectedFolderId = (result as OneDriveFolderData)?.Id ?? string.Empty;
            await _settings.SaveAsync();

            return result;

            void OnDirectoryChanged(object sender, IFolderData e)
            {
                var folderExplorer = (AbstractFolderExplorer)sender;

                _logger.LogInformation($"Directory changed: {e.Id}");

                AbstractUIElements = folderExplorer;
                AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
            }

            void OnNavigationFailed(object sender, AbstractFolderExplorerNavigationFailedEventArgs e)
            {
                // Notify the user of a problem, but don't complete the task or change state unless the
                // user explicitly cancels the operation or fixes the problem (like a network issue)

                if (e.Exception is ServiceException)
                    _notificationService.RaiseNotification("Connection lost", "We weren't able to reach OneDrive to load that folder.");
                else
                    _notificationService.RaiseNotification("Couldn't open folder", $"An error occured while opening the folder{(e.Exception is not null ? $" ({e.Exception.GetType()})" : "")}");
            }

            void OnFolderSelected(object sender, IFolderData e) => taskCompletionSource.SetResult(e);
            void OnFileExplorerCanceled(object sender, EventArgs e) => taskCompletionSource.SetResult(null);
        }

        public void SaveAbstractUI(AbstractUICollection collection)
        {
            AbstractUIElements = collection;
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void DisplayDeviceCodeResult(DeviceCodeResult dcr, CancellationTokenSource cancellationTokenSource)
        {
            var authenticateButton = new AbstractButton("codeButton", dcr.VerificationUrl)
            {
                Title = $"Go to this URL and enter the code {dcr.UserCode}",
                IconCode = "\xE8A7"
            };

            authenticateButton.Clicked += OnAuthenticateButtonClicked;

            var cancelButton = new AbstractButton("cancelButton", "Cancel")
            {
                IconCode = "\xE711"
            };

            cancelButton.Clicked += OnCancelButtonClicked;

            AbstractUIElements = new AbstractUICollection("deviceCodeResult")
            {
                Title = "Let's login",
                Subtitle = "You'll need your phone or computer"
            };

            AbstractUIElements.Add(authenticateButton);
            AbstractUIElements.Add(cancelButton);

            SourceCore.Cast<OneDriveCore>().ChangeCoreState(CoreState.NeedsSetup);
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);

            async void OnAuthenticateButtonClicked(object sender, EventArgs e)
            {
                await _launcher.LaunchUriAsync(new Uri(dcr.VerificationUrl));
            }

            void OnCancelButtonClicked(object sender, EventArgs e)
            {
                cancelButton.Clicked -= OnCancelButtonClicked;
                authenticateButton.Clicked -= OnAuthenticateButtonClicked;

                cancellationTokenSource.Cancel();
            }
        }

        private async void UseFilePropsScannerToggleOnStateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.ScanWithFileProperties = e;
            await _settings.SaveAsync();
        }

        private async void UseTagLibScannerToggleOnStateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.ScanWithTagLib = e;
            await _settings.SaveAsync();
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            if (e.PropertyName is not (nameof(OneDriveCoreSettings.ScanWithFileProperties) or nameof(OneDriveCoreSettings.ScanWithTagLib)))
                return;

            var filePropSetting = _settings.ScanWithFileProperties;
            var tagLibSetting = _settings.ScanWithTagLib;

            if (!filePropSetting && !tagLibSetting)
            {
                _scannerRequiredNotification?.Dismiss();
                _scannerRequiredNotification = _notificationService.RaiseNotification("Whoops", "At least one metadata scanner is required.");

                if (e.PropertyName == nameof(OneDriveCoreSettings.ScanWithFileProperties))
                    _useFilePropsScannerToggle.State = true;

                if (e.PropertyName == nameof(OneDriveCoreSettings.ScanWithTagLib))
                    _useTagLibScannerToggle.State = true;
            }
        }

        public ValueTask DisposeAsync()
        {
            // TODO: Logout?
            DetachEvents();
            return default;
        }
    }
}
