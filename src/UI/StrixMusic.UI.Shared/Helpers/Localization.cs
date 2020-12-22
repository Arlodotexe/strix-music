using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace StrixMusic.Helpers
{
    /// <summary>
    /// A helper class for retrieving localized  
    /// </summary>
    public static class Localization
    {
        /// <summary>
        /// Where to find the key in resources
        /// </summary>
        public enum StringContext
        {
            StrixStartup,
            MusicCommon,
            Resources,
        }

        private static ResourceLoader StrixStartup { get; } = ResourceLoader.GetForCurrentView("StrixStartup");

        private static ResourceLoader MusicCommon { get; } = ResourceLoader.GetForCurrentView("MusicCommon");

        private static ResourceLoader Resources { get; } = ResourceLoader.GetForCurrentView("Resources");

        /// <summary>
        /// Gets a localized string.
        /// </summary>
        /// <param name="context">Where to find the key.</param>
        /// <param name="key">The key to retrieve a string for.</param>
        /// <returns>A localized string.</returns>
        public static string GetLocalizedString(StringContext context, string key)
        {
            switch (context)
            {
                case StringContext.StrixStartup:
                    return StrixStartup.GetString(key);
                case StringContext.MusicCommon:
                    return MusicCommon.GetString(key);
            }

            return Resources.GetString("ResourceError");
        }
    }
}
