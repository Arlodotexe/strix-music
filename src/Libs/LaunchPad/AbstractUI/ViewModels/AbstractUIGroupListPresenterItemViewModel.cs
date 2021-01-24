using Windows.UI.Xaml.Controls;
using LaunchPad.AbstractUI.Controls;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A view model for the items in <see cref="AbstractUIGroupListPresenter"/>.
    /// </summary>
    public class AbstractUIGroupListPresenterItemViewModel
    {
        /// <summary>
        /// The ViewModel used for this element group.
        /// </summary>
        public AbstractUIElementGroupViewModel ViewModel { get; }

        /// <summary>
        /// The template selector used for this item.
        /// </summary>
        public DataTemplateSelector TemplateSelector { get; }

        /// <summary>
        /// Creates a  new instance of <see cref="AbstractUIGroupListPresenterItemViewModel"/>.
        /// </summary>
        /// <param name="viewModel">The view model to use in the template.</param>
        /// <param name="templateSelector">The template selector to use when displaying the abstract ui elements.</param>
        public AbstractUIGroupListPresenterItemViewModel(AbstractUIElementGroupViewModel viewModel, DataTemplateSelector templateSelector)
        {
            ViewModel = viewModel;
            TemplateSelector = templateSelector;
        }
    }
}