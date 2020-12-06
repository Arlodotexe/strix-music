namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// The preferred display mode for a <see cref="AbstractDataList"/>.
    /// </summary>
    /// <remarks>The UI may choose not to respect this.</remarks>
    public enum AbstractDataListPreferredDisplayMode
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
