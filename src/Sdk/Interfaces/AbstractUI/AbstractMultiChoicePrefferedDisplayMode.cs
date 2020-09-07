namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// The preferred display mode for a <see cref="IAbstractDataList"/>.
    /// </summary>
    /// <remarks>The UI may choose not to respect this.</remarks>
    public enum AbstractMultiChoicePrefferedDisplayMode
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
