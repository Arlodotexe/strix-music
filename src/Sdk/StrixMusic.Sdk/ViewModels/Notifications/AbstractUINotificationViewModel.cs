using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// A view model for <see cref="AbstractUICollection"/> being used as a Notification.
    /// </summary>
    [Bindable(true)]
    public class AbstractUINotificationViewModel : AbstractUICollectionViewModel
    {
        private const int MAX_BUTTONS = 3;
        private readonly AbstractUICollection _model;
        private ObservableCollection<AbstractButtonViewModel> _buttons;

        /// <inheritdoc />
        public AbstractUINotificationViewModel(AbstractUICollection model)
            : base(model)
        {
            _model = model;
            _buttons = new ObservableCollection<AbstractButtonViewModel>();

            EvaluateExpectedElements();
        }

        private void EvaluateExpectedElements()
        {
            System.Diagnostics.Debug.WriteLine($"Evaluating notification elements ({_model.Items.Count} total)");

            // TODO: Track one ProgressBar, 2 or 3 buttons, and TBD Elements.
            foreach (var value in _model.Items)
            {
                if (value is AbstractProgress progressUIElement)
                {
                    ProgressBarViewModel = new AbstractProgressViewModel(progressUIElement);
                }
                else if (value is AbstractButton buttonElement && ButtonViewModels.Count < MAX_BUTTONS)
                {
                    ButtonViewModels.Add(new AbstractButtonViewModel(buttonElement));
                }
                else
                {
                    UnhandledItems.Add(value);
                }
            }

            System.Diagnostics.Debug.WriteLine($"Finished evaluating notification elements ({ButtonViewModels.Count} total)");
        }

        /// <summary>
        /// A list of <see cref="AbstractUIBase"/>s that may appear in a notification that weren't explicitly handled.
        /// </summary>
        public ObservableCollection<AbstractUIBase> UnhandledItems { get; set; } = new ObservableCollection<AbstractUIBase>();

        /// <summary>
        /// A <see cref="AbstractProgress"/> that may appear in a notification.
        /// </summary>
        public AbstractProgressViewModel? ProgressBarViewModel { get; set; }

        /// <summary>
        /// A list of <see cref="AbstractButton"/>s that may appear in a notification
        /// </summary>
        public ObservableCollection<AbstractButtonViewModel> ButtonViewModels
        {
            get => _buttons; 
            set
            {
                System.Diagnostics.Debug.WriteLine($"Set {nameof(ButtonViewModels)} to {value}");
                _buttons = value;
            }
        }
    }
}
