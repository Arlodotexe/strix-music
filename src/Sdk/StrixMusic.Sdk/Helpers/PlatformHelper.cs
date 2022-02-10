namespace StrixMusic.Sdk.Helpers
{
    /// <summary>
    /// Static helpers related to the current platform.
    /// </summary>
    public static class PlatformHelper
    {
        /// <summary>
        /// Gets the current running platform.
        /// </summary>
        public static Platform Current { get; internal set; }
    }
}
