namespace StrixMusic.Cores.Storage.FileMetadata.Models;

/// <summary>
/// Contains information that describes an image, scanned from a single file.
/// </summary>
public sealed class ImageMetadata : IFileMetadata
{
    /// <summary>
    /// The unique identifier for this image.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The mime type of the image, if known. A hint to help optimize image rendering.
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// The width of this image.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// The height of this image.
    /// </summary>
    public int? Height { get; set; }
}