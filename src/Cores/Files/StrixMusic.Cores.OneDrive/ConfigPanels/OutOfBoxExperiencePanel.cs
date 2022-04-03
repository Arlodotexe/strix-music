using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Components;
using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive.ConfigPanels
{
    internal class OutOfBoxExperiencePanel : AbstractUICollection, IDisposable
    {
        private readonly OneDriveCoreSettings _settings;
        private readonly INotificationService? _notificationService;

        private readonly AbstractButton _continueButton;
        private readonly AbstractButton _cancelButton;
        private readonly AbstractUICollection _actionButtons;
        private readonly CancellationTokenSource _oobeCancellationSource;

        public OutOfBoxExperiencePanel(OneDriveCoreSettings settings, INotificationService? notificationService = null, CancellationToken cancellationToken = default)
            : base(nameof(OutOfBoxExperiencePanel))
        {
            _oobeCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _settings = settings;
            _notificationService = notificationService;

            _continueButton = new AbstractButton("continueButton", "Continue", type: AbstractButtonType.Confirm);
            _cancelButton = new AbstractButton("cancelButton", "Cancel", type: AbstractButtonType.Cancel);
            _actionButtons = new AbstractUICollection("actionButtons", PreferredOrientation.Horizontal)
            {
                _cancelButton,
                _continueButton,
            };

            AttachEvents();
        }

        private void AttachEvents()
        {
            _cancelButton.Clicked += OnCancelClicked;
        }

        private void DetachEvents()
        {
            _cancelButton.Clicked -= OnCancelClicked;
        }

        public async Task ExecuteSettingsStageAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object?>();
            using var onCanceledCleanupRegistration = _oobeCancellationSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

            Clear();

            _continueButton.Clicked += ContinueButtonOnClicked;

            var generalConfig = new GeneralCoreConfigPanel(_settings, _notificationService);
            Add(generalConfig);
            Add(_actionButtons);

            await taskCompletionSource.Task;

            Clear();

            generalConfig.Dispose();
            _continueButton.Clicked -= ContinueButtonOnClicked;

            void ContinueButtonOnClicked(object sender, EventArgs e) => taskCompletionSource.SetResult(null);
        }

        public async Task ExecuteCustomAppKeyStageAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object?>();
            using var onCanceledCleanupRegistration = _oobeCancellationSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

            Clear();

            var appIdentityKeysMissing = string.IsNullOrWhiteSpace(_settings.ClientId) ||
                                         string.IsNullOrWhiteSpace(_settings.TenantId) ||
                                         string.IsNullOrWhiteSpace(_settings.RedirectUri);

            // If we have valid keys already, ask before displaying.
            if (!appIdentityKeysMissing)
            {
                var askFirstCompletionSource = new TaskCompletionSource<bool>();
                using var askFirstCleanupRegistration = _oobeCancellationSource.Token.Register(() => askFirstCompletionSource.TrySetCanceled());

                var yesButton = new AbstractButton($"{nameof(ExecuteCustomAppKeyStageAsync)}YesButton", "Yes");
                var noButton = new AbstractButton($"{nameof(ExecuteCustomAppKeyStageAsync)}NoButton", "Skip");

                yesButton.Clicked += YesButtonOnClicked;
                noButton.Clicked += NoButtonOnClicked;

                var yesOrNoButtons = new AbstractUICollection($"{nameof(ExecuteCustomAppKeyStageAsync)}YesOrNo", PreferredOrientation.Horizontal)
                {
                    Title = "Use custom application identity?",
                    Subtitle = "(Optional) Register your own app identity with Microsoft, and use it to log into this app. If unsure, skip this step.",
                };

                yesOrNoButtons.Add(noButton);
                yesOrNoButtons.Add(yesButton);

                Add(yesOrNoButtons);

                var shouldDisplayCustom = await askFirstCompletionSource.Task;
                if (!shouldDisplayCustom)
                {
                    return;
                }

                void YesButtonOnClicked(object sender, EventArgs e) => askFirstCompletionSource.SetResult(true);
                void NoButtonOnClicked(object sender, EventArgs e) => askFirstCompletionSource.SetResult(false);
            }
            else
            {
                var missingAppIdentityNoticeCompletionSource = new TaskCompletionSource<bool>();
                using var missingIdentityCleanupRegistration = _oobeCancellationSource.Token.Register(() => missingAppIdentityNoticeCompletionSource.TrySetCanceled());

                var okButton = new AbstractButton($"{nameof(ExecuteCustomAppKeyStageAsync)}Ok", "Ok", type: AbstractButtonType.Confirm);

                okButton.Clicked += OkButtonClicked;

                var button = new AbstractUICollection($"{nameof(ExecuteCustomAppKeyStageAsync)}OkButton", PreferredOrientation.Horizontal)
                {
                    Title = "Supply an application identity",
                    Subtitle = "The default application identity is missing. Enter the details for any valid Azure app identity to log into this app.",
                };

                button.Add(okButton);

                Add(button);

                await missingAppIdentityNoticeCompletionSource.Task;

                okButton.Clicked -= OkButtonClicked;

                void OkButtonClicked(object sender, EventArgs e) => missingAppIdentityNoticeCompletionSource.SetResult(true);
            }

            Clear();

            _continueButton.Clicked += ContinueButtonOnClicked;

            var appKeyConfigPanel = new OneDriveAppKeyConfigPanel(_settings);
            Add(appKeyConfigPanel);
            Add(_actionButtons);

            await taskCompletionSource.Task;

            _oobeCancellationSource.Token.ThrowIfCancellationRequested();

            Clear();

            _continueButton.Clicked -= ContinueButtonOnClicked;
            appKeyConfigPanel.Dispose();
            _settings.UserHasSeenAuthClientKeysSettings = true;

            void ContinueButtonOnClicked(object sender, EventArgs e) => taskCompletionSource.SetResult(null);
        }

        public async Task<IFolderData?> ExecuteFolderPickerStageAsync(IFolderData rootFolder, string? displayName, string? emailAddress)
        {
            var taskCompletionSource = new TaskCompletionSource<IFolderData?>();
            _oobeCancellationSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

            Clear();

            var fileExplorer = new AbstractFolderExplorer(rootFolder);
            fileExplorer.NavigationFailed += OnNavigationFailed;
            fileExplorer.FolderSelected += OnFolderSelected;
            fileExplorer.Canceled += OnFileExplorerCanceled;

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                fileExplorer.Title = $"{displayName}'s OneDrive";
            }
            else
            {
                fileExplorer.Title = "OneDrive";

                if (!string.IsNullOrWhiteSpace(emailAddress))
                    fileExplorer.Subtitle = emailAddress;
            }

            await fileExplorer.InitAsync(_oobeCancellationSource.Token);
            _oobeCancellationSource.Token.ThrowIfCancellationRequested();

            Add(fileExplorer);

            var result = await taskCompletionSource.Task;
            if (result is not null)
                _settings.SelectedFolderId = result.Id ?? ThrowHelper.ThrowArgumentException<string>();

            _oobeCancellationSource.Token.ThrowIfCancellationRequested();

            fileExplorer.NavigationFailed -= OnNavigationFailed;
            fileExplorer.FolderSelected -= OnFolderSelected;
            fileExplorer.Canceled -= OnFileExplorerCanceled;
            fileExplorer.Dispose();

            return result;

            void OnNavigationFailed(object sender, AbstractFolderExplorerNavigationFailedEventArgs e)
            {
                // Notify the user of a problem, but don't complete the task or change state unless the
                // user explicitly cancels the operation or fixes the problem (like a network issue)
                if (e.Exception is ServiceException)
                    _notificationService?.RaiseNotification("Connection lost", "We weren't able to reach OneDrive to load that folder.");
                else
                    _notificationService?.RaiseNotification("Couldn't open folder", $"An error occurred while opening the folder{(e.Exception is not null ? $" ({e.Exception.GetType()})" : "")}");
            }

            void OnFolderSelected(object sender, IFolderData e) => taskCompletionSource.SetResult(e);
            void OnFileExplorerCanceled(object sender, EventArgs e) => _cancelButton.Click();
        }

        public void DisplayInteractiveLoginStageAsync()
        {
            Clear();

            Add(new AbstractProgressIndicator("interactiveLoginIndeterminate", true)
            {
                Title = "Logging in...",
            });

            Add(_cancelButton);
        }

        public DeviceCodeLoginPanel DisplayDeviceCodeLoginStageAsync()
        {
            Clear();

            var deviceCodeLoginPanel = new DeviceCodeLoginPanel();

            Add(deviceCodeLoginPanel);

            Add(_cancelButton);

            return deviceCodeLoginPanel;
        }

        private void OnCancelClicked(object sender, EventArgs e) => _oobeCancellationSource.Cancel();

        /// <inheritdoc />
        public void Dispose() => DetachEvents();
    }
}
