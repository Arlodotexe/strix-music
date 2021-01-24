using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Uno.Services.NotificationService
{
    /// <summary>
    /// A Service for handling notifications between the Cores and Shell.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly List<Notification> _notifications;
        private int _activeNotifications;

        /// <summary>
        /// Raised when a new notification needs to be displayed.
        /// </summary>
        public event EventHandler<Notification>? NotificationRaised;

        /// <summary>
        /// Raised when the user dismisses a notification.
        /// </summary>
        public event EventHandler<Notification>? NotificationDismissed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            _notifications = new List<Notification>();
        }

        /// <summary>
        /// The maximum number of notification that will be raised at once. Dismiss a notification to show remaining notifications in the queue.
        /// </summary>
        public int MaxActiveNotifications { get; set; } = 1;

        /// <summary>
        /// If true, then app will not handle displaying the notifications.
        /// </summary>
        public bool IsHandled { get; set; }

        /// <inheritdoc/>
        public Notification RaiseNotification(string title, string message)
        {
            string NewGuid() => Guid.NewGuid().ToString();
            var eg = new AbstractUIElementGroup(NewGuid(), PreferredOrientation.Vertical)
            {
                Title = title,
                Items = new List<AbstractUIElement>()
                {
                    new AbstractTextBox(NewGuid(), message),
                },
            };
            return RaiseNotification(eg);
        }

        /// <inheritdoc/>
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

                DismissNotification(index);
            }
        }

        /// <summary>
        /// Dismisses the top notification and raises the next notification.
        /// </summary>
        public void DismissNotification(int? index = null)
        {
            var targetNotification = _notifications.ElementAtOrDefault(index ?? _notifications.Count - 1);

            Guard.IsNotNull(targetNotification, nameof(targetNotification));

            _notifications.Remove(targetNotification);
            NotificationDismissed?.Invoke(this, targetNotification);
            targetNotification.Dismissed -= Notification_Dismissed;

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
    }
}
