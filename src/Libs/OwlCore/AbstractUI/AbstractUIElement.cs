namespace OwlCore.AbstractUI
{
    /// <summary>
    /// Abstracted UI elements.
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
