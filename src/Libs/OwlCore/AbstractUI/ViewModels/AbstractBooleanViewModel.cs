using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="AbstractBooleanUIElement"/>.
    /// </summary>
    public class AbstractBooleanViewModel : AbstractUIViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBooleanViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractBooleanViewModel(AbstractTextBox model)
            : base(model)
        {
        }
    }
}
