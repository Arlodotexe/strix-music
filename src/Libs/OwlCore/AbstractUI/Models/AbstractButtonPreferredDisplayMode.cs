namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// The preferred display mode for <see cref="AbstractButton"/>.
    /// </summary>
    /// <remarks>The UI may choose not to respect this.</remarks>
    public enum AbstractButtonPreferredDisplayMode
    {
        /// <summary>
        /// Requests a conventional button style.
        /// </summary>
        Button,

        /// <summary>
        /// Requests a hyperlink format.
        /// </summary>
        Hyperlink
    }
}