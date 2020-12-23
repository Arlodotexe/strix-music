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
            Startup,
            SuperShell,
            Common,
            Music,
            Resources,
        }

        private static ResourceLoader Startup { get; } = ResourceLoader.GetForCurrentView("Startup");

        private static ResourceLoader SuperShell { get; } = ResourceLoader.GetForCurrentView("SuperShell");

        private static ResourceLoader Common { get; } = ResourceLoader.GetForCurrentView("Common");

        private static ResourceLoader Music { get; } = ResourceLoader.GetForCurrentView("Music");

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
                case StringContext.Startup:
                    return Startup.GetString(key);
                case StringContext.SuperShell:
                    return SuperShell.GetString(key);
                case StringContext.Common:
                    return Common.GetString(key);
                case StringContext.Music:
                    return Music.GetString(key);
            }

            return Resources.GetString("ResourceError");
        }
    }
}
