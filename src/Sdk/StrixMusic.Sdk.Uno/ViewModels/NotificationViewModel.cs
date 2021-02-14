using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Uno.ViewModels
{
    /// <summary>
    /// A view model wrapper for the <see cref="Model"/> class.
    /// </summary>
    public class NotificationViewModel : ObservableObject
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
    }
}