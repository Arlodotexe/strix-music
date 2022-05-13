using OwlCore.AbstractStorage;
using OwlCore.Services;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// A container for <see cref="OneDriveCore"/> settings.
    /// </summary>
    public sealed class OneDriveCoreSettings : SettingsBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="OneDriveCoreSettings"/>.
        /// </summary>
        public OneDriveCoreSettings(IFolderData folder)
            : base(folder, NewtonsoftStreamSerializer.Singleton)
        {
        }

        /// <summary>
        /// If true, the user has had the opportunity to tweak core settings during the out of box experience.
        /// </summary>
        public bool UserHasSeenGeneralOobeSettings
        {
            get => GetSetting(() => false);
            set => SetSetting(value);
        }

        /// <summary>
        /// If true, the user has had the opportunity to supply custom application keys from Azure to authenticate their Microsoft account against.
        /// </summary>
        public bool UserHasSeenAuthClientKeysSettings
        {
            get => GetSetting(() => false);
            set => SetSetting(value);
        }

        /// <summary>
        /// If the core should use TagLib to scan file metadata.
        /// </summary>
        public bool ScanWithTagLib
        {
            get => GetSetting(() => false);
            set => SetSetting(value);
        }

        /// <summary>
        /// If the core should use file properties to scan metadata.
        /// </summary>
        public bool ScanWithFileProperties
        {
            get => GetSetting(() => true);
            set => SetSetting(value);
        }

        /// <summary>
        /// The MS Graph ID of the user-selected folder for this core instance.
        /// </summary>
        public string SelectedFolderId
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// A unique identifier for the account logged in to this core instance.
        /// </summary>
        public string AccountIdentifier
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// Client ID of a registered MS Graph application that the user will be authenticated against.
        /// </summary>
        public string ClientId
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// Tenant ID for authenticating the user against a registered MS Graph application.
        /// </summary>
        public string TenantId
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// Redirect URI used for authenticating the user against a registered MS Graph application.
        /// </summary>
        public string RedirectUri
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }
    }
}
