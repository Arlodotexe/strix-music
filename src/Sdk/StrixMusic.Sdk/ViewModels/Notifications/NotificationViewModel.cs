using System;
using System.ComponentModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// A view model wrapper for the <see cref="Model"/> class.
    /// </summary>
    [Bindable(true)]
    public sealed class NotificationViewModel : ObservableObject, IDisposable
    {
        private AbstractUINotificationViewModel _abstractUINotificationViewModel;
        private bool _disposedValue;

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

            _abstractUINotificationViewModel = new AbstractUINotificationViewModel(model.AbstractUIElementGroup);
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

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NotificationViewModel"/> class.
        /// </summary>
        ~NotificationViewModel()
         {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
         }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}