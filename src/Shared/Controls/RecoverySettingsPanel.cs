using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace StrixMusic.Controls;

/// <summary>
/// An AbstractUI panel for performing app recovery.
/// </summary>
public class RecoverySettingsPanel : AbstractUICollection, IDisposable
{
    private readonly AbstractButton _resetButton;

    /// <summary>
    /// Creates a new instance of <see cref="RecoverySettingsPanel"/>.
    /// </summary>
    public RecoverySettingsPanel()
        : base(nameof(RecoverySettingsPanel))
    {
        Title = "App recovery";
        _resetButton = new AbstractButton("resetButton", "Reset everything");

        Add(_resetButton);
        AttachEvents();
    }

    private void AttachEvents() => _resetButton.Clicked += OnResetRequested;

    private void DetachEvents() => _resetButton.Clicked -= OnResetRequested;

    private void OnResetRequested(object? sender, EventArgs e)
    {
        var confirmButton = new AbstractButton("confirmButton", "Confirm", type: AbstractButtonType.Confirm);
        var cancelButton = new AbstractButton("cancelButton", "Cancel", type: AbstractButtonType.Cancel);

        var confirmationUI = new AbstractUICollection("confirmAppResetUI")
        {
            Title = "Are you sure?",
            Subtitle = "This will wipe all data and restart the app",
        };

        var actionButtons = new AbstractUICollection("actionButtons", PreferredOrientation.Horizontal)
        {
            confirmButton,
            cancelButton,
        };

        confirmationUI.Add(actionButtons);
        confirmButton.Clicked += OnConfirmed;
        cancelButton.Clicked += OnCancelled;

        Add(confirmationUI);

        void OnCancelled(object? sender, EventArgs e)
        {
            Remove(confirmationUI);
            
            confirmButton.Clicked -= OnConfirmed;
            cancelButton.Clicked -= OnCancelled;
        }

        async void OnConfirmed(object? sender, EventArgs e)
        {
            confirmButton.Clicked -= OnConfirmed;
            cancelButton.Clicked -= OnCancelled;

            var progressIndicator = new AbstractProgressIndicator("progressIndicator", isIndeterminate: true);

            confirmationUI.Title = "Please wait.";
            confirmationUI.Subtitle = "Wipe in progress...";
            
            confirmationUI.Remove(confirmButton);
            confirmationUI.Remove(cancelButton);
            confirmationUI.Add(progressIndicator);

            await PeformNuke(onComplete: () =>
            {
                confirmationUI.Remove(progressIndicator);

                confirmationUI.Title = "Reset complete.";
                confirmationUI.Subtitle = "Please restart the app";
            });
        }
    }

    private async Task PeformNuke(Action onComplete)
    {
        await EmptyFolder(ApplicationData.Current.LocalFolder);
        await EmptyFolder(ApplicationData.Current.LocalCacheFolder);
        await EmptyFolder(ApplicationData.Current.RoamingFolder);

        ApplicationData.Current.LocalSettings.Values.Clear();
        ApplicationData.Current.RoamingSettings.Values.Clear();

#if WINDOWS_UWP
            await ApplicationData.Current.ClearAsync();
            await CoreApplication.RequestRestartAsync(string.Empty);
#endif

        onComplete();

        async Task EmptyFolder(IStorageFolder folder)
        {
            IReadOnlyList<IStorageItem>? items = null;

            try
            {
                items = await folder.GetItemsAsync();
            }
            catch
            {
                return;
            }

            foreach (var item in items)
            {
                try
                {
                    await item.DeleteAsync();
                }
                catch
                {
                    /* ignored */
                }
            }
        }
    }

    /// <inheritdoc />
    public void Dispose() => DetachEvents();
}
