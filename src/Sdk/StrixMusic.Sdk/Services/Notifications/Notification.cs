using System;
using OwlCore.AbstractUI.Models;
using OwlCore.Remoting;
using OwlCore.Remoting.Attributes;

namespace StrixMusic.Sdk.Services.Notifications
{
    /// <summary>
    /// A Notification displayed by a Shell.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public sealed class Notification
    {
        /// <summary>
        /// Raised when the Notification is dismissed.
        /// </summary>
        public event EventHandler? Dismissed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="abstractUIElementGroup">The <see cref="AbstractUICollection"/> to display for the notification content.</param>
        public Notification(AbstractUICollection abstractUIElementGroup)
        {
            AbstractUICollection = abstractUIElementGroup;
        }

        /// <summary>
        /// The <see cref="OwlCore.AbstractUI.Models.AbstractUICollection"/> to be displayed for the notification.
        /// </summary>
        public AbstractUICollection AbstractUICollection { get; }

        /// <summary>
        /// If true, the notification is being displayed to the user.
        /// </summary>
        internal bool IsDisplayed { get; set; }

        /// <summary>
        /// Raises the <see cref="Dismissed"/> event for the Core.
        /// </summary>
        [RemoteMethod]
        public void Dismiss()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
