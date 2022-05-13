using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.Services;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.WinUI.Services.NotificationService
{
     /// <inheritdoc />
    public sealed class NotificationService : INotificationService
    {
        private readonly List<Notification> _pendingNotifications;
        private readonly List<Notification> _activeNotifications;

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
            _pendingNotifications = new List<Notification>();
            _activeNotifications = new List<Notification>();
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
        public int MaxActiveNotifications { get; set; } = 3;

        /// <inheritdoc/>
        public Notification RaiseNotification(string title, string message)
        {
            var id = Guid.NewGuid().ToString();
            var elementGroup = new AbstractUICollection(id)
            {
                Title = title,
                Subtitle = message,
            };

            return RaiseNotification(elementGroup);
        }

        /// <inheritdoc/>
        public Notification RaiseNotification(AbstractUICollection elementGroup)
        {
            var notification = new Notification(elementGroup);

            notification.Dismissed += Notification_Dismissed;

            _pendingNotifications.Add(notification);

            FireIfReady();
            return notification;
        }

        private void Notification_Dismissed(object sender, EventArgs e)
        {
            if (sender is Notification notification)
            {
                var index = _activeNotifications.FindIndex(x => ReferenceEquals(x, notification));

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
        public void DismissNotification(int? index = null) => DismissNotificationInternal(index);

        private void DismissNotificationInternal(int? index = null)
        {
            var targetNotification = _activeNotifications.ElementAtOrDefault(index ?? _pendingNotifications.Count - 1);

            Guard.IsNotNull(targetNotification, nameof(targetNotification));

            _activeNotifications.Remove(targetNotification);
            NotificationDismissed?.Invoke(this, targetNotification);
            targetNotification.Dismissed -= Notification_Dismissed;

            FireIfReady();
        }

        /// <summary>
        /// Fires the next notification if there is space for it.
        /// </summary>
        private void FireIfReady()
        {
            if (_pendingNotifications.Count > 0 && _activeNotifications.Count < MaxActiveNotifications)
            {
                var nextNotification = _pendingNotifications.FirstOrDefault();
                if (nextNotification == null)
                    return;

                _pendingNotifications.Remove(nextNotification);
                _activeNotifications.Add(nextNotification);
                NotificationRaised?.Invoke(this, nextNotification);
            }
        }
    }
}
