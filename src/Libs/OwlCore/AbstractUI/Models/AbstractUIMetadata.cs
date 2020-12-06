namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Abstracted metadata that can be displayed inside of certain <see cref="AbstractUIElement"/>s.
    /// </summary>
    public class AbstractUIMetadata : AbstractUIBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIMetadata"/>.
        /// </summary>
        /// <param name="id"></param>
        public AbstractUIMetadata(string id)
            : base(id)
        {
        }
    }
}
