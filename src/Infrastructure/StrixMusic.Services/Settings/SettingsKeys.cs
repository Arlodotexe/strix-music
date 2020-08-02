using StrixMusic.Services.Settings.Enums;

namespace StrixMusic.Services.Settings
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the main app.
    /// </summary>
    /// <remarks>
    /// This <see lang="class"/> is used with reflection to generate settings files.
    /// </remarks>
    public static class SettingsKeys
    {
        /// <summary>
        /// Gets the default value for <see cref="Enums.PreferredShell"/> in settings.
        /// </summary>
        public static readonly PreferredShell PreferredShell = PreferredShell.Default;
    }
}
