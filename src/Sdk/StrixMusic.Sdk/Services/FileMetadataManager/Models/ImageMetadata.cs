using System;

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

        /// <summary>
        /// The link to the file for this image.
        /// </summary>
        public Uri? Uri { get; set; }

        /// <summary>
        /// The width of this image.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The height of this image.
        /// </summary>
        public int? Height { get; set; }
    }
}