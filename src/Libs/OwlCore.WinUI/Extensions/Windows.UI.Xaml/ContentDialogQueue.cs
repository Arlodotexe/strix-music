using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Indicates how a <see cref="ContentDialog"/> should be shown.
    /// </summary>
    public enum ShowType
    {
        /// <summary>
        /// If a dialog is already active, wait for it to close before displaying this.
        /// </summary>
        QueueNext,

        /// <summary>
        /// If a dialog is already active, wait for it and all queued dialogs to close before displaying this.
        /// </summary>
        QueueLast,

        /// <summary>
        /// If a dialog is already being displayed, then store it, close it, and show this new dialog. When closed, restore the interrupted dialog.
        /// </summary>
        Interrupt,
    }

    /// <summary>
    /// Advanced helpers for dialog boxes.
    /// </summary>
    public static class DialogExtensions
    {
        // The last item in the queue should always be showed next.
        private readonly static ObservableCollection<ContentDialog> _queue = new();
        private static ContentDialog? _interruptedDialog;

        /// <summary>
        /// The currently displayed dialog, if any.
        /// </summary>
        public static ContentDialog? CurrentDialog { get; private set; }

        /// <summary>
        /// Begins an asynchronous operation to show the dialog.
        /// </summary>
        /// <param name="contentDialog">The dialog to show.</param>
        /// <param name="showType">Indicates how this dialog should be shown.</param>
        /// <returns>A <see cref="Task"/> that completes once this dialog has been shown and closed.</returns>
        public static async Task<ContentDialogResult> ShowAsync(this ContentDialog contentDialog, ShowType showType)
        {
            // To interrupt a showing dialog
            if (showType == ShowType.Interrupt && CurrentDialog is not null)
            {
                Guard.IsNull(_interruptedDialog);

                _interruptedDialog = CurrentDialog;
                _interruptedDialog.Hide();

                // This needs to happen before ShowAsync because if you open two "Inturrupt" dialogs,
                // It will yield to a race condition as ShowAsync waits for a response from opened dialog.
                CurrentDialog = contentDialog;
                _interruptedDialog = null;

                return await contentDialog.ShowAsync();
            }

            // To show a dialog for after the current one closes.
            if (showType == ShowType.QueueNext && CurrentDialog is not null)
            {
                _queue.Add(contentDialog);
                await WhenNextInQueueAsync(contentDialog);
                _queue.Remove(contentDialog);
            }

            // To show a dialog when all queued dialogs close
            if (showType == ShowType.QueueLast && CurrentDialog is not null)
            {
                _queue.InsertOrAdd(0, contentDialog);
                await WhenNextInQueueAsync(contentDialog);
                _queue.Remove(contentDialog);
            }

            Guard.IsNull(CurrentDialog);
            CurrentDialog = contentDialog;

            return await CurrentDialog.ShowAsync();
        }

        private static Task WhenNextInQueueAsync(ContentDialog contentDialog)
        {
            var taskCompletionSource = new TaskCompletionSource<object?>();
            _queue.CollectionChanged += (_, _) =>
            {
                if (_queue.LastOrDefault() == contentDialog)
                    taskCompletionSource.SetResult(null);
            };

            return taskCompletionSource.Task;
        }
    }
}
