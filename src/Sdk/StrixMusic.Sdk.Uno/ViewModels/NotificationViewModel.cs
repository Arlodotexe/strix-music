using Windows.UI.Xaml.Controls;
using LaunchPad.AbstractUI.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Uno.ViewModels
{
    /// <summary>
    /// A view model wrapper for the <see cref="Model"/> class.
    /// </summary>
    public class NotificationViewModel : ObservableObject
    {
        private AbstractUIElementGroupViewModel _abstractUIElementGroupView;

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

            _abstractUIElementGroupView = new AbstractUIElementGroupViewModel(model.AbstractUIElementGroup);
            DismissCommand = new RelayCommand(model.Dismiss);
        }

        public AbstractUIElementGroupViewModel AbstractUIElementGroup
        {
            get => _abstractUIElementGroupView;
            set { _abstractUIElementGroupView = value; OnPropertyChanged(nameof(AbstractUIElementGroup)); }
        }

        /// <summary>
        /// Handles dismissing the notification via a bindable command.
        /// </summary>
        public IRelayCommand DismissCommand { get; set; }
    }
}