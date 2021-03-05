namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// A common interface for all types of metadata extracted from a file.
    /// </summary>
    public interface IFileMetadata
    {
        /// <summary>
        /// The unique identifier for this item.
        /// </summary>
        public string? Id { get; set; }
    }
}