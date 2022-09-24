using System.Xml.Serialization;

namespace StrixMusic.Cores.Storage.FileMetadata.Models.Playlist.Smil;

/// <summary>
/// Holds all metadata and title of the playlist.
/// </summary>
internal class Head
{
    /// <inheritdoc cref="Meta"/>
    [XmlElement("meta")]
    public Meta[]? Meta { get; set; }

    /// <inheritdoc cref="Title"/>
    [XmlElement("title")]
    public string? Title { get; set; }
}