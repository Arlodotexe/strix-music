namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// The type of button to display.
    /// </summary>
    public enum AbstractButtonType
    {
        /// <summary>
        /// A generic button, could serve any purpose.
        /// </summary>
        Generic,

        /// <summary>
        /// Some sort of confirmation button. Likely displayed with the accent color.
        /// </summary>
        Confirm,

        /// <summary>
        /// Scary click. Likely displayed in Red.
        /// </summary>
        Delete,
    }
}