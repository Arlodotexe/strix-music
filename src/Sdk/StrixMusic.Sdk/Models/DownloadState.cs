namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// The download state of any playable item.
    /// </summary>
    public enum DownloadState
    {
        /// <summary>
        /// Downloading this item is not needed or not supported.
        /// </summary>
        NotSupported,

        /// <summary>
        /// Downloading is supported, but has not been started.
        /// </summary>
        NotDownloaded,

        /// <summary>
        /// Download is in progress.
        /// </summary>
        Downloading,

        /// <summary>
        /// The download has completed successfully and the item can be played offline.
        /// </summary>
        Downloaded,

        /// <summary>
        /// An active download is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// Downloading has failed.
        /// </summary>
        Failed,
    }
}
