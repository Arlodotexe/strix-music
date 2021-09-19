namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// Holds all settings service keys for <see cref="OneDriveCoreSettingsService"/>.
    /// </summary>
    public class OneDriveCoreSettingsKeys
    {
        /// <summary>
        /// If the core should use TagLib to scan file metadata.
        /// </summary>
        public static readonly bool ScanWithTagLib = false;

        /// <summary>
        /// If the core should use file properties to scan metadata.
        /// </summary>
        public static readonly bool ScanWithFileProperties = true;

        /// <summary>
        /// The MS Graph ID of the user-selected folder for this core instance.
        /// </summary>
        public static readonly string SelectedFolderId = string.Empty;

        /// <summary>
        /// Tenant ID for authenticating the user against a registered MS Graph application.
        /// </summary>
        public static readonly string TenantId = string.Empty;

        /// <summary>
        /// Redirect URI used for authenticating the user against a registered MS Graph application.
        /// </summary>
        public static readonly string RedirectUri = string.Empty;

        /// <summary>
        /// Client ID of a registered MS Graph application that the user will be authenticated against.
        /// </summary>
        public static readonly string ClientId = string.Empty;
    }
}
