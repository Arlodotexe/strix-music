namespace StrixMusic.Sdk
{
    /// <summary>
    /// The various targets that can be requested for top level app navigation.
    /// </summary>
    public enum AppNavigationTarget
    {
        /// <summary>
        /// Displays the SuperShell.
        /// </summary>
        SuperShell,

        /// <summary>
        /// Displays settings for the current shell. If no shell is loaded or the shell doesn't respond to the request, the SuperShell will show instead.
        /// </summary>
        Settings,

        /// <summary>
        /// TODO: Not implemented.
        /// </summary>
        Debug,
    }
}