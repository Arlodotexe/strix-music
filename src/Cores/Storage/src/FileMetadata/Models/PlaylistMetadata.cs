﻿using System;
using System.Collections.Generic;

namespace StrixMusic.Cores.Storage.FileMetadata.Models;

/// <summary>
/// The metadata associated with a playlist.
/// </summary>
public sealed class PlaylistMetadata : IFileMetadata
{
    /// <summary>
    /// The unique identifier for this playlist. Should match the ID of the file the playlist was scanned from.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The unique identifier(s) for tracks in this playlist.
    /// </summary>
    public HashSet<string>? TrackIds { get; set; }

    /// <summary>
    /// The title of this playlist.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The total duration of this playlist.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// The description of this playlist.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The total number of tracks in this playlist.
    /// </summary>
    public int TotalTrackCount { get; set; }

    /// <summary>
    /// The total number of artists represented in this playlist.
    /// </summary>
    public int TotalArtistsCount { get; set; }

    /// <summary>
    /// The external link associated with this playlist.
    /// </summary>
    public Uri? Url { get; set; }
}
