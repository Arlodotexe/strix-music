namespace StrixMusic.Sdk.Helpers
{
    /// <summary>
    /// Static helpers related to the current platform.
    /// </summary>
    public class PlatformHelper
    {
        /// <summary>
        /// Sets up the platform helper.
        /// </summary>
        public PlatformHelper(Platform platform)
        {
            Current = platform;
        }

        /// <summary>
        /// Gets the current running platform.
        /// </summary>
        public static Platform Current { get; internal set; }
    }
}
