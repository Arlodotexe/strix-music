using System.Collections.Generic;

namespace StrixMusic.Cores.Storage.FileMetadata.Models;

/// <summary>
/// Holds metadata scanned from a single file.
/// </summary>
public sealed class FileMetadata
{
    /// <summary>
    /// Creates a new instance of <see cref="FileMetadata"/>.
    /// </summary>
    /// <param name="id">The unique identifier for the storable item.</param>
    public FileMetadata(string id)
    {
        Id = id;
    }

    /// <summary>
    /// The unique identifier for this file.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The track information for this file.
    /// </summary>
    public TrackMetadata? TrackMetadata { get; set; }

    /// <summary>
    /// Album information for this file.
    /// </summary>
    public AlbumMetadata? AlbumMetadata { get; set; }

    /// <summary>
    /// The artists who created the <see cref="AlbumMetadata"/>.
    /// </summary>
    public List<ArtistMetadata>? AlbumArtistMetadata { get; set; }

    /// <summary>
    /// The artists who created the <see cref="TrackMetadata"/>.
    /// </summary>
    public List<ArtistMetadata>? TrackArtistMetadata { get; set; }

    /// <summary>
    /// The metadata for the playlist.
    /// </summary>
    public PlaylistMetadata? PlaylistMetadata { get; set; }

    /// <summary>
    /// Image metadata for this file.
    /// </summary>
    public List<ImageMetadata>? ImageMetadata { get; set; }
}