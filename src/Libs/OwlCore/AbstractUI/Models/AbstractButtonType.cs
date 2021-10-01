namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Indicates an intended purpose for an <see cref="AbstractButton"/>.
    /// </summary>
    public enum AbstractButtonType
    {
        /// <summary>
        /// A generic button, could serve any purpose.
        /// </summary>
        Generic,

        /// <summary>
        /// Indicates a confirmation action.
        /// </summary>
        Confirm,

        /// <summary>
        /// Indicates that this button is a non-affirmative action, such as delete or cancel.
        /// </summary>
        Cancel,
    }
}