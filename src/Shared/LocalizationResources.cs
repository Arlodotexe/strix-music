using Windows.ApplicationModel.Resources;

namespace StrixMusic
{
    /// <summary>
    /// A helper for getting localized strings from available <see cref="ResourceLoader"/> providers.
    /// </summary>
    public static class LocalizationResources
    {
        /// <summary>
        /// The resource loader for strings related to startup.
        /// </summary>
        public static ResourceLoader Startup => ResourceLoader.GetForCurrentView(nameof(Startup));

        /// <summary>
        /// The resource loader for strings related to the SuperShell.
        /// </summary>
        public static ResourceLoader SuperShell => ResourceLoader.GetForCurrentView(nameof(SuperShell));

        /// <summary>
        /// The resource loader for Quips.
        /// </summary>
        public static ResourceLoader Quips => ResourceLoader.GetForCurrentView(nameof(Quips));
    }
}
