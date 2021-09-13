namespace StrixMusic.Cores.LocalFiles.Services
{
    /// <summary>
    /// The setting keys for <see cref="LocalFilesCore"/>.
    /// </summary>
    public static class LocalFilesCoreSettingsKeys
    {
        /// <summary>
        /// The folder that the user has chosen to scan for this core instance.
        /// </summary>
        public static readonly string FolderPath = string.Empty;

        /// <summary>
        /// If true, the app will not initialize the metadata repos and previously scanned data will not be loaded.
        /// </summary>
        public static readonly bool InitWithEmptyMetadataRepos = false;
    }
}