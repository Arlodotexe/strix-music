using OwlCore.AbstractUI.Models;
using System.ComponentModel;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A wrapping ViewModel for <see cref="AbstractUIMetadata"/>.
    /// </summary>
    [Bindable(true)]
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