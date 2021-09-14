using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Services.AbstractUIStorageExplorers;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Cores.OneDrive.Storage;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    ///  <inheritdoc/>
    public sealed class OneDriveCoreConfig : ICoreConfig
    {
        private GraphServiceClient? _graphClient;
        private IFolderData? _rootFolder;

        private ISettingsService? _settingsService;
        private AuthenticationManager? _authenticationManager;
        private FileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreConfig"/> class.
        /// </summary>
        public OneDriveCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public IReadOnlyList<AbstractUICollection> AbstractUIElements { get; private set; } = new List<AbstractUICollection>();

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        public Task SetupConfigurationServices(IServiceCollection services)
        {
            _settingsService = new OneDriveCoreSettingsService(SourceCore.InstanceId);
            services.AddSingleton(_settingsService);

            Services = services.BuildServiceProvider();

            return Task.CompletedTask;
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
            var continueButton = new AbstractButton("continueButton", "Continue", type: AbstractButtonType.Confirm);
            var cancelButton = new AbstractButton("cancelButton", "Cancel", type: AbstractButtonType.Cancel);
            var actionButtons = new AbstractUICollection("actionButtons", PreferredOrientation.Horizontal)
            {
                cancelButton,
                continueButton,
            };

            showAdvanced.StateChanged += OnShowAdvancedClicked;
            continueButton.Clicked += OnNavigationButtonClicked;
            cancelButton.Clicked += OnNavigationButtonClicked;

            var initialSettings = BuildSimpleSettings();
            SetupTitle(initialSettings);

            void OnNavigationButtonClicked(object sender, EventArgs e) => DetachEvents();

            void DetachEvents()
            {
                continueButton.Clicked -= OnNavigationButtonClicked;
                cancelButton.Clicked -= OnNavigationButtonClicked;

                // Detaches any events related to advanced settings.
                if (showAdvanced.State)
                    showAdvanced.State = false;

                showAdvanced.StateChanged -= OnShowAdvancedClicked;
            }

            void OnShowAdvancedClicked(object sender, bool newState)
            {
                var ui = newState ?
                    BuildAdvancedSettings() :
                    BuildSimpleSettings();

                SetupTitle(ui);
                SaveAbstractUI(ui);
            }

            void SetupTitle(AbstractUICollection collection)
            {
                collection.Title = "Login to OneDrive";
                collection.Subtitle = "To get set up with OneDrive, you'll need to log in with your Microsoft account.";
            }

            AbstractUICollection BuildSimpleSettings()
            {
                return new AbstractUICollection("SettingsGroup")
                {
                    showAdvanced,
                    actionButtons,
                };
            }

            AbstractUICollection BuildAdvancedSettings()
            {
                Guard.IsNotNull(showAdvanced, nameof(showAdvanced));
                Guard.IsNotNull(continueButton, nameof(continueButton));

                var clientIdTb = new AbstractTextBox("ClientId", string.Empty, "Enter custom client id");
                var tenantTb = new AbstractTextBox("Tenant Id", string.Empty, "Enter custom tenant id");
                var redirectUriTb = new AbstractTextBox("Redirect Uri", string.Empty, "Enter custom redirect uri (if any)");

                clientIdTb.ValueChanged += OnTextBoxChanged;
                tenantTb.ValueChanged += OnTextBoxChanged;
                redirectUriTb.ValueChanged += OnTextBoxChanged;
                showAdvanced.StateChanged += OnAdvancedTurnedOff;

                async void OnTextBoxChanged(object sender, string value)
                {
                    Guard.IsNotNull(_settingsService, nameof(_settingsService));

                    if (sender == clientIdTb)
                        await _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId), value.Trim());

                    if (sender == tenantTb)
                        await _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId), value.Trim());

                    if (sender == redirectUriTb)
                        await _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.RedirectUri), value.Trim());
                }

                void OnAdvancedTurnedOff(object sender, bool newState)
                {
                    clientIdTb.ValueChanged -= OnTextBoxChanged;
                    tenantTb.ValueChanged -= OnTextBoxChanged;
                    redirectUriTb.ValueChanged -= OnTextBoxChanged;
                    showAdvanced.StateChanged -= OnAdvancedTurnedOff;
                }

                return new AbstractUICollection("SettingsGroup")
                {
                    showAdvanced,
                    clientIdTb,
                    tenantTb,
                    redirectUriTb,
                    actionButtons,
                };
            }

            return initialSettings;
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
        public async Task<bool> TryLoginAsync()
        {
            Guard.IsNotNull(Services, nameof(Services));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var notificationService = Services.GetRequiredService<INotificationService>();

            var clientId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId));
            var tenantId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId));
            var redirectUri = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.RedirectUri));

            _authenticationManager = new AuthenticationManager(clientId, tenantId, redirectUri);
            _graphClient = await _authenticationManager.GenerateGraphToken();

            if (_graphClient is null)
            {
                notificationService.RaiseNotification("Error", "OneDrive encountered an error while logging in.");
                return false;
            }

            SourceCore.Cast<OneDriveCore>().ChangeInstanceDescriptor(_authenticationManager.EmailAddress ?? string.Empty);
            return true;
        }

        public async Task SetupMetadataScannerAsync(IServiceCollection services, IFolderData folder)
        {
            _fileMetadataManager = new FileMetadataManager(SourceCore.InstanceId, folder);

            // Scanning file contents are possible but extremely slow over the network.
            // The Graph API supplies music metadata from file properties, which is much faster.
            _fileMetadataManager.ScanTypes = MetadataScanTypes.FileProperties;

            // Must be on the Core IoC for FileCore base classes to get access to it.
            services.AddSingleton<IFileMetadataManager>(_fileMetadataManager);

            Services = null;
            Services = services.BuildServiceProvider();

            await _fileMetadataManager.InitAsync();
            Task.Run(_fileMetadataManager.StartScan).Forget();
        }

        public async Task<IFolderData?> PickSingleFolderAsync()
        {
            Guard.IsNotNull(_graphClient, nameof(_graphClient));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            // Get root OneDrive folder.
            var driveItem = await _graphClient.Drive.Root.Request().Expand("children").GetAsync();
            _rootFolder = new OneDriveFolderData(_graphClient, driveItem);

            // Setup folder explorer
            var fileExplorer = new AbstractFolderExplorer(_rootFolder);
            await fileExplorer.InitAsync();

            // Show folder explorer
            AbstractUIElements = fileExplorer.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);

            fileExplorer.DirectoryChanged += OnDirectoryChanged;

            // Wait until the user has picked a file.
            var folderSelectionCancellationTokenSource = new CancellationTokenSource();
            folderSelectionCancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(30));

            var taskCompletionSource = new TaskCompletionSource<IFolderData?>();

            fileExplorer.FolderSelected += OnFolderSelected;
            fileExplorer.Canceled += OnFileExplorerCanceled;

            var result = await taskCompletionSource.Task;

            fileExplorer.DirectoryChanged -= OnDirectoryChanged;
            fileExplorer.Dispose();

            if (result is null)
                return null;

            await _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.SelectedFolderId), result.Cast<OneDriveFolderData>().OneDriveFolderId);

            void OnDirectoryChanged(object sender, IFolderData e)
            {
                var folderExplorer = (AbstractFolderExplorer)sender;

                AbstractUIElements = folderExplorer.IntoList();
                AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
            }

            return result;
            
            void OnFolderSelected(object sender, IFolderData e) => taskCompletionSource.SetResult(e);
            void OnFileExplorerCanceled(object sender, EventArgs e) => taskCompletionSource.SetResult(null);
        }

        public void SaveAbstractUI(AbstractUICollection collection)
        {
            AbstractUIElements = collection.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
