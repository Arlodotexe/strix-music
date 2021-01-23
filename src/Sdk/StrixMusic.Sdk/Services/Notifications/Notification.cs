using System;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Services.Notifications
{
    /// <summary>
    /// A Notification displayed by a Shell.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Raised when the Notification is dismissed.
        /// </summary>
        public event EventHandler? Dismissed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="elementGroup">The <see cref="AbstractUIElementGroup"/> to display for the notification content.</param>
        public Notification(AbstractUIElementGroup elementGroup)
        {
            AbstractUIElementGroup = elementGroup;
        }

        /// <summary>
        /// The <see cref="AbstractUIElementGroup"/> to be displayed for the notification.
        /// </summary>
        public AbstractUIElementGroup AbstractUIElementGroup { get; }

        /// <summary>
        /// If true, the notification is being displayed to the user.
        /// </summary>
        internal bool IsDisplayed { get; set; }

        /// <summary>
        /// Raises the <see cref="Dismissed"/> event for the Core.
        /// </summary>
        public void Dismiss()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
