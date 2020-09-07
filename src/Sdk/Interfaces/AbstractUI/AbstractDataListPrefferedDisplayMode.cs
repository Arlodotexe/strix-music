namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// The preferred display mode for a <see cref="IAbstractDataList"/>.
    /// </summary>
    /// <remarks>The UI may choose not to respect this.</remarks>
    public enum AbstractDataListPrefferedDisplayMode
    {
        /// <summary>
        /// Displays the items in a grid.
        /// </summary>
        Grid,

        /// <summary>
        /// Displays the items in a vertical list.
        /// </summary>
        List,
    }
}
