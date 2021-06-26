namespace StrixMusic.Sdk.Uno.Helpers
{
    /// <summary>
    /// Constants and Assembly data accessed throughout the library.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The prefix of the resources path.
        /// </summary>
        public const string ResourcesPrefix = "ms-appx:///";

        /// <summary>
        /// Localization resources constants.
        /// </summary>
        public static class Localization
        {
            /// <summary>
            /// The PRI path of the Startup resource.
            /// </summary>
            public const string StartupResource = "Startup";

            /// <summary>
            /// The PRI path of the SuperShell resource.
            /// </summary>
            public const string SuperShellResource = "SuperShell";

            /// <summary>
            /// The PRI path of the Common resource.
            /// </summary>
            public const string CommonResource = "Common";

            /// <summary>
            /// The PRI path of the Time resource.
            /// </summary>
            public const string TimeResource = "Time";

            /// <summary>
            /// The PRI path of the Music resource.
            /// </summary>
            public const string MusicResource = "Music";

            /// <summary>
            /// The PRI path of the Quips resource.
            /// </summary>
            public const string QuipsResource = "Quips";

            /// <summary>
            /// The default string in the case of a localization error.
            /// </summary>
            public const string LocalizationErrorString = "Localization Error";
        }
    }
}
