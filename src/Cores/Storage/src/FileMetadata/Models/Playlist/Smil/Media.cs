using System.Xml.Serialization;

namespace StrixMusic.Cores.Storage.FileMetadata.Models.Playlist.Smil;

/// <summary>
/// Holds all information related to the added track.
/// </summary>
public class Media
{
    /// <summary>
    /// The location of the track.
    /// </summary>
    [XmlAttribute("src")]
    public string? Src { get; set; }

    /// <summary>
    /// The album title of the added track.
    /// </summary>
    [XmlAttribute("albumTitle")]
    public string? AlbumTitle { get; set; }

    /// <summary>
    /// The album artist name.
    /// </summary>
    [XmlAttribute("albumArtist")]
    public string? AlbumArtist { get; set; }

    /// <summary>
    /// The track title.
    /// </summary>
    [XmlAttribute("trackTitle")]
    public string? TrackTitle { get; set; }

    /// <summary>
    /// The track artist.
    /// </summary>
    [XmlAttribute("trackArtist")]
    public string? TrackArtist { get; set; }

    /// <summary>
    /// The total track time.
    /// </summary>
    [XmlAttribute("duration")]
    public long Duration { get; set; }
}
