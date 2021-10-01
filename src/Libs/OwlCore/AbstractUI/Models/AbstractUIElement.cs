namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// A base class for all AbstractUI elements.
    /// </summary>
    public abstract class AbstractUIElement : AbstractUIBase
    {
        /// <summary>
        /// Creates a new instance of a <see cref="AbstractUIElement"/>.
        /// </summary>
        /// <param name="id"></param>
        protected AbstractUIElement(string id)
            : base(id)
        {
        }
    }
}
