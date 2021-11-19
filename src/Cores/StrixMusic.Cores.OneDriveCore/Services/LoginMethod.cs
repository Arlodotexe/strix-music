namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// The methods used to login to OneDrive.
    /// </summary>
    public enum LoginMethod
    {
        /// <summary>
        /// Login is disabled.
        /// </summary>
        None,

        /// <summary>
        /// A popup window is used for login.
        /// </summary>
        Interactive,

        /// <summary>
        /// The user is asked to visit a website and enter a code.
        /// </summary>
        DeviceCode,
    }
}
