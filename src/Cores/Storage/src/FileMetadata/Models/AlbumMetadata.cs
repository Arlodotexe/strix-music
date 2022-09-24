using System;
using System.Collections.Generic;

namespace StrixMusic.Cores.Storage.FileMetadata.Models;

/// <summary>
/// Contains information that describes an album, scanned from one or more files.
/// </summary>
public sealed class AlbumMetadata : IFileMetadata
{
    /// <summary>
    /// The unique identifier for this album.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The title of this album.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The description of this album.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The unique identifer(s) for this album's image(s).
    /// </summary>
    public HashSet<string>? ImageIds { get; set; }

    /// <summary>
    /// The unique identifier(s) for this album's track(s).
    /// </summary>
    public HashSet<string>? TrackIds { get; set; }

    /// <summary>
    /// The unique identifier(s) for this album's artist(s).
    /// </summary>
    public HashSet<string>? ArtistIds { get; set; }

    /// <summary>
    /// The total duration of this album.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// The release date of this album.
    /// </summary>
    public DateTime? DatePublished { get; set; }

    /// <summary>
    /// The genres of this album.
    /// </summary>
    public HashSet<string>? Genres { get; set; }
}