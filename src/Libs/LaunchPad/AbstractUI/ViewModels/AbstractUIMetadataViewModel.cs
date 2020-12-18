using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A wrapping ViewModel for <see cref="AbstractUIMetadata"/>.
    /// </summary>
    public class AbstractUIMetadataViewModel : AbstractUIViewModelBase<AbstractUIMetadata>
    {
        /// <summary>
        /// Creates a new instance of <see cref=" AbstractUIMetadataViewModel"/>.
        /// </summary>
        /// <param name="model"></param>
        public AbstractUIMetadataViewModel(AbstractUIMetadata model)
            : base(model)
        {
        }
    }
}