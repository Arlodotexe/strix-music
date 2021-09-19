namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// Indicates a type of file scan.
    /// </summary>
    public enum FileScanningType
    {
        /// <summary>
        /// No file scan.
        /// </summary>
        None,

        /// <summary>
        /// Indicating a scan of files containing raw audio.
        /// </summary>
        AudioFiles,

        /// <summary>
        /// Indicates a playlist metadata scan.
        /// </summary>
        Playlists,
    }
}