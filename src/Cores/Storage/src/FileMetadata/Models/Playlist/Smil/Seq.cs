using System.Xml.Serialization;

namespace StrixMusic.Cores.Storage.FileMetadata.Models.Playlist.Smil;

/// <summary>
/// All info regarding the playlist track. 
/// </summary>
internal class Seq
{
    /// <summary>
    /// All info regarding the playlist track.
    /// </summary>
    [XmlElement("media")]
    public Media[]? Media { get; set; }
}