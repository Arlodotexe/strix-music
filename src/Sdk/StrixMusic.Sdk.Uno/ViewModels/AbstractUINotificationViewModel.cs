using LaunchPad.AbstractUI.ViewModels;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Uno.ViewModels
{
    /// <summary>
    /// A view model for <see cref="AbstractUIElementGroup"/> being used as a Notification.
    /// </summary>
    public class AbstractUINotificationViewModel : AbstractUIElementGroupViewModel
    {
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
            }
        }

        /// <summary>
        /// A <see cref="AbstractProgressUIElement"/> that may appear in a notification.
        /// </summary>
        public AbstractProgressUIElementViewModel? ProgressBarViewModel { get; set; }
    }
}
