using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.LocalFiles.Services
{
    /// <summary>
    /// The setting keys for <see cref="LocalFilesCore"/>.
    /// </summary>
    public class LocalFilesCoreSettingsKeys : SettingsKeysBase
    {
        /// <summary>
        /// The folder that the user has chosen to scan for this core instance.
        /// </summary>
        public static string FolderPath => string.Empty;

        /// <summary>
        /// If true, the app will not initialize the metadata repos and previously scanned data will not be loaded.
        /// </summary>
        public static bool InitWithEmptyMetadataRepos => false;

        /// <summary>
        /// If the core should use TagLib to scan file metadata.
        /// </summary>
        public static bool ScanWithTagLib => true;

        /// <summary>
        /// If the core should use file properties to scan metadata. 
        /// </summary>
        public static bool ScanWithFileProperties => false;

        /// <inheritdoc />
        public override object GetDefaultValue(string settingKey)
        {
            return settingKey switch
            {
                nameof(FolderPath) => FolderPath,
                nameof(InitWithEmptyMetadataRepos) => InitWithEmptyMetadataRepos,
                nameof(ScanWithTagLib) => ScanWithTagLib,
                nameof(ScanWithFileProperties) => ScanWithFileProperties,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(settingKey)),
            };
        }
    }
}