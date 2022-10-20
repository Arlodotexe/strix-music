using Windows.ApplicationModel.Resources;

namespace StrixMusic.Sdk.WinUI.Globalization
{
    /// <summary>
    /// A service for getting localized strings from <see cref="ResourceLoader"/> providers.
    /// </summary>
    public static class LocalizationResources
    {
        /// <summary>
        /// The resource loader for common strings.
        /// </summary>
        public static ResourceLoader? Common => ResourceLoader.GetForCurrentView(nameof(Common));

        /// <summary>
        /// The resource loader for time-related strings.
        /// </summary>
        public static ResourceLoader? Time => ResourceLoader.GetForCurrentView(nameof(Time));

        /// <summary>
        /// The resource loader for generic music-related strings.
        /// </summary>
        public static ResourceLoader? Music => ResourceLoader.GetForCurrentView(nameof(Music));
    }
}
