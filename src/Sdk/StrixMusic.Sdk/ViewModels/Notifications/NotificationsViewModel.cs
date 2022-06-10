// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// Manages the notifications coming from an instance of <see cref="INotificationService"/>.
    /// </summary>
    [Bindable(true)]
    public sealed class NotificationsViewModel : ObservableObject, IDisposable
    {
        private readonly SynchronizationContext _syncContext;
        private readonly INotificationService _notificationService;
        private bool _isHandled;

        /// <summary>
        /// Creates a new instance of <see cref="NotificationsViewModel"/>.
        /// </summary>
        /// <param name="notificationService"></param>
        public NotificationsViewModel(INotificationService notificationService)
        {
            _syncContext = SynchronizationContext.Current;
            _notificationService = notificationService;

            AttachEvents();
        }

        /// <summary>
        /// The currently display notifications.
        /// </summary>
        public ObservableCollection<NotificationViewModel> Notifications { get; set; } = new();

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

        private void NotificationService_NotificationDismissed(object sender, Notification e) => _syncContext.Post(_ =>
        {
            var relevantNotification = Notifications.FirstOrDefault(x => ReferenceEquals(x.Model, e));
            if (relevantNotification is null)
                return;

            Notifications.Remove(relevantNotification);
        }, null);

        private void NotificationService_NotificationRaised(object sender, Notification e) => _syncContext.Post(_ =>
        {
            Notifications.Add(new NotificationViewModel(e));
        }, null);

        /// <inheritdoc />
        public void Dispose() => DetachEvents();
    }
}
