using Windows.ApplicationModel.Resources;

namespace StrixMusic.Sdk.Uno.Services.Localization
{
    /// <summary>
    /// A service for getting localized strings from <see cref="ResourceLoader"/> providers.
    /// </summary>
    public sealed class LocalizationResourceLoader
    {
        /// <summary>
        /// The resource loader for strings related to startup.
        /// </summary>
        public ResourceLoader? Startup { get; init; }

        /// <summary>
        /// The resource loader for strings related to the SuperShell.
        /// </summary>
        public ResourceLoader? SuperShell { get; init; }

        /// <summary>
        /// The resource loader for common strings.
        /// </summary>
        public ResourceLoader? Common { get; init; }

        /// <summary>
        /// The resource loader for time-related strings.
        /// </summary>
        public ResourceLoader? Time { get; init; }

        /// <summary>
        /// The resource loader for time-related strings.
        /// </summary>
        public ResourceLoader? Music { get; init; }

        /// <summary>
        /// The resource loader for Quips.
        /// </summary>
        public ResourceLoader? Quips { get; init; }
    }
}
