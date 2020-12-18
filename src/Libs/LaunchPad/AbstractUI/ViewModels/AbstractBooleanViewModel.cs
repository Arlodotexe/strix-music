using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="AbstractBooleanUIElement"/>.
    /// </summary>
    public class AbstractBooleanViewModel : AbstractUIViewModelBase<AbstractBooleanUIElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBooleanViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractBooleanViewModel(AbstractBooleanUIElement model)
            : base(model)
        {
        }
    }
}
