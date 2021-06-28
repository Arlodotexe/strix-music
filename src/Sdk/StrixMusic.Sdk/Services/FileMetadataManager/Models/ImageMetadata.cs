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
        /// Defines standard image sizes used by the application.
        /// </summary>
        public enum ImageSize
        {
            /// <summary>
            /// A 64x64 image.
            /// </summary>
            Tiny,

            /// <summary>
            /// A 128x128 image.
            /// </summary>
            Small,

            /// <summary>
            /// A 256x256 image.
            /// </summary>
            Medium,

            /// <summary>
            /// A 512x512 image.
            /// </summary>
            Large,

            /// <summary>
            /// A 1024x1024 image.
            /// </summary>
            ExtraLarge
        }
    }
}
