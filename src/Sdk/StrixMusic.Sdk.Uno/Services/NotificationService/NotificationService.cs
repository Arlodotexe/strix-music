using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using OwlCore.Remoting;
using OwlCore.Remoting.Attributes;
using StrixMusic.Sdk.Services.Notifications;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.Uno.Services.NotificationService
{
    /// <summary>
    /// A Service for handling notifications between the Cores and Shell.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public class NotificationService : INotificationService
    {
        private readonly MemberRemote _memberRemote;

        private readonly List<Notification> _notifications;
        private int _activeNotifications;

        /// <inheritdoc cref="INotificationService.NotificationRaised" />
        public event EventHandler<Notification>? NotificationRaised;

        /// <inheritdoc cref="INotificationService.NotificationDismissed" />
        public event EventHandler<Notification>? NotificationDismissed;

        /// <summary>
        /// Raised when the notification margins are requested to be changed.
        /// </summary>
        public event EventHandler<Thickness>? NotificationMarginChanged;

        /// <summary>
        /// Raised when the notification alignment is requested to be changed.
        /// </summary>
        public event EventHandler<(HorizontalAlignment, VerticalAlignment)>? NotificationAlignmentChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            _notifications = new List<Notification>();
            _memberRemote = new MemberRemote(this, nameof(NotificationService));
        }

        /// <inheritdoc cref="NotificationRaised"/>
        event EventHandler<Notification>? INotificationService.NotificationRaised
        {
            add => NotificationRaised += value;
            remove => NotificationRaised -= value;
        }

        /// <inheritdoc cref="NotificationDismissed"/>
        event EventHandler<Notification>? INotificationService.NotificationDismissed
        {
            add => NotificationDismissed += value;
            remove => NotificationDismissed -= value;
        }

        /// <inheritdoc/>
        public int MaxActiveNotifications { get; set; } = 1;

        /// <inheritdoc/>
        public Notification RaiseNotification(string title, string message)
        {
            var id = Guid.NewGuid().ToString();
            var elementGroup = new AbstractUIElementGroup(id)
            {
                Title = title,
                Subtitle = message,
            };

            return RaiseNotification(elementGroup);
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Notification RaiseNotification(AbstractUIElementGroup elementGroup)
        {
            var notification = new Notification(elementGroup);

            notification.Dismissed += Notification_Dismissed;

            _notifications.Add(notification);

            FireIfReady();
            return notification;
        }

        private void Notification_Dismissed(object sender, EventArgs e)
        {
            if (sender is Notification notification)
            {
                var index = _notifications.FindIndex(x => ReferenceEquals(x, notification));

                if (index == -1)
                    return;

                DismissNotificationInternal(index);
            }
        }

        /// <summary>
        /// Request that the notification margin be changed.
        /// </summary>
        /// <param name="thickness">The margin to put around the notification panel.</param>
        public void ChangeNotificationMargins(Thickness thickness)
        {
            NotificationMarginChanged?.Invoke(this, thickness);
        }

        /// <summary>
        /// Request that the notification alignment be changed.
        /// </summary>
        /// <param name="horizontalAlignment">The new horizontal alignment.</param>
        /// <param name="verticalAlignment">The new verical alignment.</param>
        public void ChangeNotificationAlignment(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            NotificationAlignmentChanged?.Invoke(this, (horizontalAlignment, verticalAlignment));
        }

        /// <summary>
        /// Dismisses the top notification and raises the next notification.
        /// </summary>
        [RemoteMethod]
        public void DismissNotification(int? index = null) => DismissNotificationInternal(index);

        private void DismissNotificationInternal(int? index = null)
        {
            var targetNotification = _notifications.ElementAtOrDefault(index ?? _notifications.Count - 1);

            Guard.IsNotNull(targetNotification, nameof(targetNotification));

            _notifications.Remove(targetNotification);
            NotificationDismissed?.Invoke(this, targetNotification);
            targetNotification.Dismissed -= Notification_Dismissed;
            targetNotification.Dispose();

            _activeNotifications--;

            FireIfReady();
        }

        /// <summary>
        /// Fires the next notification if there is space for it.
        /// </summary>
        private void FireIfReady()
        {
            if (_notifications.Count > 0 && _activeNotifications < MaxActiveNotifications)
            {
                var nextNotification = _notifications.FirstOrDefault(x => !x.IsDisplayed);
                if (nextNotification == null)
                    return;

                nextNotification.IsDisplayed = true;
                NotificationRaised?.Invoke(this, nextNotification);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _memberRemote.Dispose();
        }
    }
}
