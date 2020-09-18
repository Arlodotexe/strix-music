namespace StrixMusic.Sdk.Services.SuperShell
{
    /// <summary>
    /// An <see langword="enum"/> that labels the various content that can be displayed in the SuperShell.
    /// </summary>
    public enum SuperShellDisplayState
    {
        /// <summary>
        /// The SuperShell is hidden.
        /// </summary>
        Hidden,

        /// <summary>
        /// The SuperShell is shown and is showing settings.
        /// </summary>
        Settings,

        /// <summary>
        /// The SuperShell is shown and is showing the debug screen.
        /// </summary>
        Debug,
    }
}
