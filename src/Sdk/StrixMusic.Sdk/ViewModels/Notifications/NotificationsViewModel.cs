using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// Manages the notifications coming from the <see cref="INotificationService"/>.
    /// </summary>
    [Bindable(true)]
    public sealed class NotificationsViewModel : ObservableObject, IDisposable
    {
        private readonly INotificationService _notificationService;
        private bool _isHandled;

        /// <summary>
        /// Creates a new instance of <see cref="NotificationsViewModel"/>.
        /// </summary>
        /// <param name="notificationService"></param>
        public NotificationsViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;

            AttachEvents();
        }

        /// <summary>
        /// The currently display notifications.
        /// </summary>
        public ObservableCollection<NotificationViewModel> Notifications { get; set; } = new ObservableCollection<NotificationViewModel>();

        /// <summary>
        /// Gets or sets whether or not notifications are handled and shouldn't be displayed.
        /// </summary>
        public bool IsHandled
        {
            get => _isHandled;
            set => SetProperty(ref _isHandled, value);
        }

        private void AttachEvents()
        {
            _notificationService.NotificationRaised += NotificationService_NotificationRaised;
            _notificationService.NotificationDismissed += NotificationService_NotificationDismissed;
        }

        private void DetachEvents()
        {
            _notificationService.NotificationRaised -= NotificationService_NotificationRaised;
            _notificationService.NotificationDismissed -= NotificationService_NotificationDismissed;
        }

        private void NotificationService_NotificationDismissed(object sender, Notification e)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                var relevantNotification = Notifications.FirstOrDefault(x => ReferenceEquals(x.Model, e));
                if (relevantNotification is null)
                    return;

                Notifications.Remove(relevantNotification);
            });
        }

        private void NotificationService_NotificationRaised(object sender, Notification e)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Notifications.Add(new NotificationViewModel(e));
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachEvents();
        }
    }
}