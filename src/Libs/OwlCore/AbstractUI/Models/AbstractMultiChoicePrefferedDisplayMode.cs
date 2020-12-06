namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// The preferred display mode for a <see cref="AbstractDataList"/>.
    /// </summary>
    /// <remarks>The UI may choose not to respect this.</remarks>
    public enum AbstractMultiChoicePreferredDisplayMode
    {
        /// <summary>
        /// Displays items in a dropdown menu.
        /// </summary>
        Dropdown,

        /// <summary>
        /// Displays items as multiple radio buttons.
        /// </summary>
        RadioButtons,
    }
}
