namespace StrixMusic
{
    /// <summary>
    /// Holds important data needed to initialize the app in a production environment.
    /// </summary>
    /// <remarks> Preconfigured for development. Add another partial and use a "!DEBUG" compilation conditional to configure for release mode.
    /// <para /> IMPORTANT:
    /// <para /> You will be prompted in-app for the missing data, do not modify this file.
    /// </remarks>
    internal static partial class Secrets
    {
#if DEBUG
        /// <summary>
        /// The <see cref="OneDriveCoreSettings.ClientId"/> for <see cref="OneDriveCore"/>.
        /// </summary>
        public static string OneDriveClientId { get; } = string.Empty;

        /// <summary>
        /// The <see cref="OneDriveCoreSettings.TenantId"/> for <see cref="OneDriveCore"/>.
        /// </summary>
        public static string OneDriveTenantId { get; } = string.Empty;

        /// <summary>
        /// The <see cref="OneDriveCoreSettings.RedirectUri"/> for <see cref="OneDriveCore"/>.
        /// </summary>
        public static string OneDriveRedirectUri { get; } = string.Empty;
#endif
    }
}
