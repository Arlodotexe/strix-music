using StrixMusic.Sdk.Data;

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
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Should be externally settable but not accessible as part of normal usage.")]
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
