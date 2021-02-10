using OwlCore.Uno.AbstractUI.ViewModels;
using OwlCore.AbstractUI.Models;
using System.Collections.ObjectModel;

namespace StrixMusic.Sdk.Uno.ViewModels
{
    /// <summary>
    /// A view model for <see cref="AbstractUIElementGroup"/> being used as a Notification.
    /// </summary>
    public class AbstractUINotificationViewModel : AbstractUIElementGroupViewModel
    {
        private const int MAX_BUTTONS = 3;
        private readonly AbstractUIElementGroup _model;

        /// <inheritdoc />
        public AbstractUINotificationViewModel(AbstractUIElementGroup model)
            : base(model)
        {
            _model = model;
            EvaluateExpectedElements();
        }

        private void EvaluateExpectedElements()
        {
            // TODO: Track one ProgressBar, 2 or 3 buttons, and TBD Elements.
            foreach (var value in _model.Items)
            {
                if (value is AbstractProgressUIElement progressUIElement)
                    ProgressBarViewModel = new AbstractProgressUIElementViewModel(progressUIElement);
                else if (value is AbstractButton buttonElement && ButtonViewModels.Count < MAX_BUTTONS)
                    ButtonViewModels.Add(new AbstractButtonViewModel(buttonElement));
                else
                    UnhandledItems.Add(value);
            }
        }

        /// <summary>
        /// A list of <see cref="AbstractUIBase"/>s that may appear in a notification that weren't explicitly handled.
        /// </summary>
        public ObservableCollection<AbstractUIBase> UnhandledItems { get; set; } = new ObservableCollection<AbstractUIBase>();

        /// <summary>
        /// A <see cref="AbstractProgressUIElement"/> that may appear in a notification.
        /// </summary>
        public AbstractProgressUIElementViewModel? ProgressBarViewModel { get; set; }

        /// <summary>
        /// A list of <see cref="AbstractButton"/>s that may appear in a notification
        /// </summary>
        public ObservableCollection<AbstractButtonViewModel> ButtonViewModels { get; set; } = new ObservableCollection<AbstractButtonViewModel>();
    }
}
