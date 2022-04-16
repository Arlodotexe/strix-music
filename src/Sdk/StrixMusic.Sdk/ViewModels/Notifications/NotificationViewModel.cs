// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// A view model wrapper for the <see cref="Notification"/> class.
    /// </summary>
    [Bindable(true)]
    public sealed class NotificationViewModel : ObservableObject, IDisposable
    {
        private AbstractUINotificationViewModel _abstractUINotificationViewModel;

        /// <summary>
        /// The original notification model.
        /// </summary>
        public Notification Model { get; }

        /// <summary>
        /// Creates a new instance of <see cref="NotificationViewModel"/>.
        /// </summary>
        /// <param name="model">The backing model.</param>
        public NotificationViewModel(Notification model)
        {
            Model = model;

            _abstractUINotificationViewModel = new AbstractUINotificationViewModel(model.AbstractUICollection);
            DismissCommand = new RelayCommand(model.Dismiss);
        }

        /// <summary>
        /// The view model of the Notification being show.
        /// </summary>
        public AbstractUINotificationViewModel AbstractUINotificationViewModel
        {
            get => _abstractUINotificationViewModel;
            set
            {
                _abstractUINotificationViewModel = value;
                _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(AbstractUINotificationViewModel)));
            }
        }

        /// <summary>
        /// Handles dismissing the notification via a bindable command.
        /// </summary>
        public IRelayCommand DismissCommand { get; set; }

        /// <inheritdoc/>
        public void Dispose() => _abstractUINotificationViewModel?.Dispose();
    }
}
