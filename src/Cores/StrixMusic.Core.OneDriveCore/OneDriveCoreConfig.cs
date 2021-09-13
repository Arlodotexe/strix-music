using System;
using System.Collections.Generic;
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
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive
{
    ///  <inheritdoc/>
    public sealed class OneDriveCoreConfig : ICoreConfig
    {
        private GraphServiceClient? _graphClient;
        private IFolderData? _rootFolder;
        private readonly TaskCompletionSource<object?> _filePickerTaskCompletionSource = new TaskCompletionSource<object?>();

        private ISettingsService? _settingsService;
        private AuthenticationManager? _authenticationManager;

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

        public void SetupAbstractUIForFirstSetup()
        {
            var showAdvanced = new AbstractBoolean("showAdvanced", "Show advanced");
            var continueButton = new AbstractButton("continueButton", "Continue");

            showAdvanced.StateChanged += OnShowAdvancedClicked;
            continueButton.Clicked += OnContinueClicked;

            var initialSettings = BuildSimpleSettings();
            SaveAbstractUI(initialSettings);

            void OnContinueClicked(object sender, EventArgs e)
            {
                continueButton.Clicked -= OnContinueClicked;
                showAdvanced.StateChanged -= OnShowAdvancedClicked;

                // Detaches any events related to advanced settings.
                if (showAdvanced.State)
                    showAdvanced.State = false;
            }

            void OnShowAdvancedClicked(object sender, bool newState)
            {
                var ui = newState ?
                    BuildAdvancedSettings() :
                    BuildSimpleSettings();

                SaveAbstractUI(ui);
            }

            void SaveAbstractUI(AbstractUICollection collection)
            {
                AbstractUIElements = collection.IntoList();
                AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
            }

            AbstractUICollection BuildSimpleSettings()
            {
                return new AbstractUICollection("SettingsGroup")
                {
                    new AbstractRichTextBlock("IntroText", "To get set up with OneDrive, you'll need to log in with your Microsoft account."),
                    showAdvanced,
                    continueButton,
                };
            }

            AbstractUICollection BuildAdvancedSettings()
            {
                Guard.IsNotNull(showAdvanced, nameof(showAdvanced));
                Guard.IsNotNull(continueButton, nameof(continueButton));

                var clientIdTb = new AbstractTextBox("ClientId", string.Empty, "Enter client id");
                var tenantTb = new AbstractTextBox("Tenant Id", string.Empty, "Enter tenant id");
                var redirectUriTb = new AbstractTextBox("Redirect Uri", string.Empty, "Enter redirect uri (if any)");

                clientIdTb.ValueChanged += OnTextBoxChanged;
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
                    showAdvanced.StateChanged -= OnAdvancedTurnedOff;
                }

                return new AbstractUICollection("SettingsGroup")
                {
                    new AbstractRichTextBlock("IntroText", "To get set up with OneDrive, you'll need to log in with your Microsoft account."),
                    showAdvanced,
                    clientIdTb,
                    tenantTb,
                    redirectUriTb,
                    continueButton,
                };
            }
        }

        /// <summary>
        /// Logs the user in.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task LoginAsync()
        {
            Guard.IsNotNull(Services, nameof(Services));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            Guard.IsNotNull(_graphClient, nameof(_graphClient));

            var notificationService = Services.GetRequiredService<INotificationService>();

            var clientId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId));
            var tenantId = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId));
            var redirectUri = await _settingsService.GetValue<string>(nameof(OneDriveCoreSettingsKeys.RedirectUri));

            _authenticationManager = new AuthenticationManager(clientId, tenantId, redirectUri);
            _graphClient = await _authenticationManager.GenerateGraphToken();

            if (_graphClient is null)
            {
                notificationService.RaiseNotification("Error", "OneDrive encountered an error while logging in.");
                return;
            }

            SourceCore.Cast<OneDriveCore>().ChangeInstanceDescriptor(_authenticationManager.EmailAddress ?? string.Empty);

            // Get root OneDrive folder.
            var driveItem = await _graphClient.Drive.Root.Request().Expand("children").GetAsync();
            _rootFolder = new OneDriveFolderData(_graphClient, driveItem);

            await InitAbstractFolderExplorer(_rootFolder);

            // Wait until the user has picked a file.
            await _filePickerTaskCompletionSource.Task;

            SourceCore.Cast<OneDriveCore>().ChangeCoreState(CoreState.Configured);
            SourceCore.Cast<OneDriveCore>().ChangeCoreState(CoreState.Loaded);

            AbstractUIElements = new AbstractUICollection(string.Empty).IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task InitAbstractFolderExplorer(IFolderData folder)
        {
            var fileExplorer = new AbstractFolderExplorer(folder);
            await fileExplorer.InitAsync();

            Guard.IsNotNull(fileExplorer.AbstractUI, nameof(fileExplorer.AbstractUI));

            AbstractUIElements = fileExplorer.AbstractUI.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);

            fileExplorer.FolderSelected += FolderExplorerService_FolderSelected;
            fileExplorer.DirectoryChanged += FileExplorer_DirectoryChanged;
        }

        private void FileExplorer_DirectoryChanged(object sender, IFolderData e)
        {
            var fileExplorer = (AbstractFolderExplorer)sender;

            Guard.IsNotNull(fileExplorer.AbstractUI, nameof(fileExplorer.AbstractUI));

            AbstractUIElements = fileExplorer.AbstractUI.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FolderExplorerService_FolderSelected(object sender, IFolderData e)
        {
            Guard.IsNotNull(_settingsService, nameof(OneDriveCoreSettingsService));
            var fileExplorer = (AbstractFolderExplorer)sender;
            fileExplorer.FolderSelected -= FolderExplorerService_FolderSelected;

            _settingsService.SetValue<string>(nameof(OneDriveCoreSettingsKeys.FolderPath), e.Path);

            // Let listeners know that the user has picked a file.
            _filePickerTaskCompletionSource.SetResult(null);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
