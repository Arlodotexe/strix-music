using OwlCore.AbstractUI.Models;

namespace OwlCore.Uno.AbstractUI.ViewModels
{
    /// <summary>
    /// A wrapping ViewModel for <see cref="AbstractUIMetadata"/>.
    /// </summary>
    public class AbstractUIMetadataViewModel : AbstractUIViewModelBase
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