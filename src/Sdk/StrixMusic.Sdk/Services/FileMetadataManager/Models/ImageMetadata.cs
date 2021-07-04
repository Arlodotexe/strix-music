namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// Contains information that describes an image, scanned from a single file.
    /// </summary>
    public class ImageMetadata : IFileMetadata
    {
        /// <summary>
        /// The unique identifier for this image.
        /// </summary>
        public string? Id { get; set; }
    }
}
