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
    public class Notification : IDisposable
    {
        private readonly MemberRemote _memberRemote;

        /// <summary>
        /// Raised when the Notification is dismissed.
        /// </summary>
        public event EventHandler? Dismissed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="abstractUIElementGroup">The <see cref="AbstractUIElementGroup"/> to display for the notification content.</param>
        public Notification(AbstractUIElementGroup abstractUIElementGroup)
        {
            AbstractUIElementGroup = abstractUIElementGroup;
            _memberRemote = new MemberRemote(this, abstractUIElementGroup.Id);
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
        [RemoteMethod]
        public void Dismiss()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _memberRemote.Dispose();
        }
    }
}
