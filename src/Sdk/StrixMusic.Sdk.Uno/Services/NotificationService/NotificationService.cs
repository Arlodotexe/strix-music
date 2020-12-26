using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Services.Notifications;
using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Uno.Services.NotificationService
{
    /// <summary>
    /// A Service for handling notifications between the Cores and Shell.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private const int MAX_ACTIVE_NOTIFICATIONS = 1;

        private Queue<Notification> _notifications;
        private int _activeNotifications = 0;

        /// <summary>
        /// Raised when a new notification needs to be displayed.
        /// </summary>
        public event EventHandler<object>? NotificationRaised;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            _notifications = new Queue<Notification>();
        }

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
            _notifications.Enqueue(notification);
            return notification;
        }

        /// <summary>
        /// Dimisses the top notification and raises the next notification.
        /// </summary>
        public void DismissNotification()
        {
            _activeNotifications--;
            _notifications.Dequeue().Dismiss();
            FireIfReady();
        }

        private void FireIfReady()
        {
            if (_notifications.Count > 0 && _activeNotifications < MAX_ACTIVE_NOTIFICATIONS)
            {
                NotificationRaised?.Invoke(this, _notifications.Peek());
            }
        }
    }
}
