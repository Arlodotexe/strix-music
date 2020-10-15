namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the zune shell.
    /// </summary>
    /// <remarks>
    /// This <see lang="class"/> is used with reflection to generate settings files.
    /// </remarks>
    public static class ZuneDesktopSettingsKeys
    {
        /// <summary>
        /// Gets the default value for <see cref="BackgroundImage"/> in settings.
        /// </summary>
        public static readonly ZuneDesktopBackgroundImage BackgroundImage = new ZuneDesktopBackgroundImage();
    }
}
