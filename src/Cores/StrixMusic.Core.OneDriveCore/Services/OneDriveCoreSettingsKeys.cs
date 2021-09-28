using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// Holds all settings service keys for <see cref="OneDriveCoreSettingsService"/>.
    /// </summary>
    public class OneDriveCoreSettingsKeys : SettingsKeysBase
    {
        /// <summary>
        /// If the core should use TagLib to scan file metadata.
        /// </summary>
        public static bool ScanWithTagLib => false;

        /// <summary>
        /// If the core should use file properties to scan metadata.
        /// </summary>
        public static bool ScanWithFileProperties => true;

        /// <summary>
        /// The MS Graph ID of the user-selected folder for this core instance.
        /// </summary>
        public static string SelectedFolderId => string.Empty;

        /// <summary>
        /// Tenant ID for authenticating the user against a registered MS Graph application.
        /// </summary>
        public static string TenantId => string.Empty;

        /// <summary>
        /// Redirect URI used for authenticating the user against a registered MS Graph application.
        /// </summary>
        public static string RedirectUri => string.Empty;

        /// <summary>
        /// Client ID of a registered MS Graph application that the user will be authenticated against.
        /// </summary>
        public static string ClientId => string.Empty;

        /// <inheritdoc />
        public override object GetDefaultValue(string settingKey)
        {
            return settingKey switch
            {
                nameof(ScanWithTagLib) => ScanWithTagLib,
                nameof(ScanWithFileProperties) => ScanWithFileProperties,
                nameof(SelectedFolderId) => SelectedFolderId,
                nameof(TenantId) => TenantId,
                nameof(RedirectUri) => RedirectUri,
                nameof(ClientId) => ClientId,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(settingKey)),
            };
        }
    }
}
