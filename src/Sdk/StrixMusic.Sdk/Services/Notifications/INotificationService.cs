using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Services.Notifications
{
    /// <summary>
    /// A Service for handling notifications from Cores
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Enqueue a notification for the Shell to display.
        /// </summary>
        /// <param name="title">The title for the notification.</param>
        /// <param name="message">The body content for the notification.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Method raises event")]
        Notification RaiseNotification(string title, string message = "");

        /// <summary>
        /// Enqueue a notification for the Shell to display.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Method raises event")]
        Notification RaiseNotification(AbstractUIElementGroup elementGroup);
    }
}
