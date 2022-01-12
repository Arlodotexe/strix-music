namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Holds information about a download operation.
    /// </summary>
    public struct DownloadInfo : System.IEquatable<DownloadInfo>
    {
        /// <summary>
        /// A value between 0 and 65535 representing how much of this playable item has been downloaded for offline playback.
        /// </summary>
        public ushort Progress { get; }

        /// <summary>
        /// The current download state.
        /// </summary>
        public DownloadState State { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is DownloadInfo status && Equals(status);

        /// <inheritdoc />
        public override int GetHashCode() => State.GetHashCode() + Progress.GetHashCode();

        /// <inheritdoc />
        public static bool operator ==(DownloadInfo left, DownloadInfo right) => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(DownloadInfo left, DownloadInfo right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(DownloadInfo other) => other.State == State && other.Progress == Progress;
    }
}
