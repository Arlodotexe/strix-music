using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using OwlCore.Remoting;

namespace StrixMusic.Sdk.Services.Notifications
{
    /// <summary>
    /// A remoting-enabled wrapper for <see cref="INotificationService"/>.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    // TODO Implement INotificationService and figure out async compatability
    public sealed class RemoteNotificationService : IDisposable
    {
        private readonly INotificationService? _notificationService;
        private readonly MemberRemote _memberRemote;
        private readonly List<MemberRemote> _notificationMemberRemotes;

        /// <summary>
        /// Increments when a notification is displayed from this service, to provide unique IDs for remoting.
        /// </summary>
        private int _notificationTick;

        /// <inheritdoc/>
        public int MaxActiveNotifications => _notificationService?.MaxActiveNotifications ?? 0;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteNotificationService"/>, wrapping an existing implementation.
        /// </summary>
        /// <param name="id">A consistent, unique identifier for synchronizing an instance of this service remotely.</param>
        /// <param name="notificationService">The notification service to wrap around.</param>
        public RemoteNotificationService(string id, INotificationService notificationService)
            : this(id)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteNotificationService"/>.
        /// </summary>
        /// <param name="id">A consistent, unique identifier for synchronizing an instance of this service remotely.</param>
        public RemoteNotificationService(string id)
        {
            _memberRemote = new MemberRemote(this, $"{nameof(RemoteNotificationService)}.{id}");
            _notificationMemberRemotes = new List<MemberRemote>();
        }

        private void AttachEvents()
        {
            if (_notificationService is null)
                return;

            _notificationService.NotificationRaised += OnNotificationRaised;
            _notificationService.NotificationDismissed += OnNotificationDismissed;
        }

        private void DetachEvents()
        {
            if (_notificationService is null)
                return;

            _notificationService.NotificationRaised -= OnNotificationRaised;
            _notificationService.NotificationDismissed -= OnNotificationDismissed;
        }

        private void OnNotificationRaised(object sender, Notification e) => OnNotificationRaised_Internal(e);

        private void OnNotificationDismissed(object sender, Notification e) => OnNotificationDismissed_Internal(e);

        [RemoteMethod]
        private void OnNotificationRaised_Internal(Notification notification)
        {
            NotificationRaised?.Invoke(this, notification);
        }

        [RemoteMethod]
        private void OnNotificationDismissed_Internal(Notification notification)
        {
            NotificationDismissed?.Invoke(this, notification);

            var target = _notificationMemberRemotes.FirstOrDefault(x => x.Id == notification.AbstractUICollection.Id);
            if (!(target is null))
                _notificationMemberRemotes.Remove(target);

            target?.Dispose();
        }

        /// <inheritdoc cref="INotificationService.NotificationRaised"/>
        public event EventHandler<Notification>? NotificationRaised;

        /// <inheritdoc cref="INotificationService.NotificationDismissed"/>
        public event EventHandler<Notification>? NotificationDismissed;

        /// <inheritdoc/>
        [RemoteMethod]
        public async Task<Notification> RaiseNotification(string title, string message = "")
        {
            if (_memberRemote.Mode == RemotingMode.Client)
            {
                Guard.IsNotNull(_notificationService, nameof(_notificationService));
                var notification = _notificationService.RaiseNotification(title, message);

                _notificationMemberRemotes.Add(new MemberRemote(notification, notification.AbstractUICollection.Id));

                await _memberRemote.PublishDataAsync($"{_notificationTick++}", notification);

                return notification;
            }
            else if (_memberRemote.Mode == RemotingMode.Host)
            {
                var data = await _memberRemote.ReceiveDataAsync<Notification>($"{_notificationTick++}");

                Guard.IsNotNull(data, nameof(data));
                return data;
            }
            else
            {
                return ThrowHelper.ThrowNotSupportedException<Notification>();
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async Task<Notification> RaiseNotification(AbstractUICollection elementGroup)
        {
            if (_memberRemote.Mode == RemotingMode.Client)
            {
                Guard.IsNotNull(_notificationService, nameof(_notificationService));
                var notification = _notificationService.RaiseNotification(elementGroup);

                _notificationMemberRemotes.Add(new MemberRemote(notification, notification.AbstractUICollection.Id));

                await _memberRemote.PublishDataAsync($"{_notificationTick++}", notification);

                return notification;
            }
            else if (_memberRemote.Mode == RemotingMode.Host)
            {
                var data = await _memberRemote.ReceiveDataAsync<Notification?>($"{_notificationTick++}");

                Guard.IsNotNull(data, nameof(data));
                return data;
            }
            else
            {
                return ThrowHelper.ThrowNotSupportedException<Notification>();
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _memberRemote.Dispose();
            DetachEvents();
        }
    }
}
