using System;

namespace OwlCore.Exceptions
{
    /// <summary>
    /// Used when a UI element was expected, but not found.
    /// </summary>
    public class UIElementNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="UIElementNotFoundException"/>.
        /// </summary>
        /// <param name="elementName">The expected element</param>
        public UIElementNotFoundException(string elementName)
            : base($"The UI element \"{elementName}\" was not found")
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="UIElementNotFoundException"/>.
        /// </summary>
        /// <param name="elementName">The expected element</param>
        /// <param name="innerException"><inheritdoc/></param>
        public UIElementNotFoundException(string elementName, Exception innerException)
            : base($"The UI element \"{elementName}\" was not found", innerException)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="UIElementNotFoundException"/>.
        /// </summary>
        public UIElementNotFoundException()
        {
        }
    }
}
