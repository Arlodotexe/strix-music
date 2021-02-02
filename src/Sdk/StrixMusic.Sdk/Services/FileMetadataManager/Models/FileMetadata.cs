namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// Holds multiple sets of metadata scanned from a single file.
    /// </summary>
    public class FileMetadata
    {
        /// <summary>
        /// A unique identifier.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The track information for this file.
        /// </summary>
        public TrackMetadata? TrackMetadata { get; set; }

        /// <summary>
        /// Album information for this file.
        /// </summary>
        public AlbumMetadata? AlbumMetadata { get; set; }

        /// <summary>
        /// Artist information for this file.
        /// </summary>
        public ArtistMetadata? ArtistMetadata { get; set; }
    }
}
