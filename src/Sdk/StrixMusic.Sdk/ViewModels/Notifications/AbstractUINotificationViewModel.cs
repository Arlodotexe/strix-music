using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// Wraps an <see cref="AbstractUICollection"/> intended for Notifications and parses supported elements for rendering.
    /// </summary>
    [Bindable(true)]
    public class AbstractUINotificationViewModel : AbstractUICollectionViewModel
    {
        private const int MAX_BUTTONS = 3;
        private readonly AbstractUICollection _model;

        /// <inheritdoc />
        public AbstractUINotificationViewModel(AbstractUICollection model)
            : base(model)
        {
            _model = model;

            EvaluateSupportedItems();
        }

        private void EvaluateSupportedItems()
        {
            foreach (var value in _model.Items)
            {
                if (value is AbstractProgress progressUIElement && ProgressBar is null)
                {
                    ProgressBar = new AbstractProgressViewModel(progressUIElement);
                }
                else if (value is AbstractButton buttonElement && Buttons.Count < MAX_BUTTONS)
                {
                    Buttons.Add(new AbstractButtonViewModel(buttonElement));
                }
                else
                {
                    UnsupportedItems.Add(value);
                }
            }
        }

        /// <summary>
        /// A list of <see cref="AbstractUIBase"/>s were given to the notification that aren't explicitly supported.
        /// The can be displayed in popout window if desired.
        /// </summary>
        public ObservableCollection<AbstractUIBase> UnsupportedItems { get; set; } = new ObservableCollection<AbstractUIBase>();

        /// <summary>
        /// An <see cref="AbstractProgress"/> that may appear in a notification.
        /// </summary>
        public AbstractProgressViewModel? ProgressBar { get; set; }

        /// <summary>
        /// A list of <see cref="AbstractButton"/>s that may appear in a notification
        /// </summary>
        public ObservableCollection<AbstractButtonViewModel> Buttons { get; set; } = new ObservableCollection<AbstractButtonViewModel>();
    }
}
