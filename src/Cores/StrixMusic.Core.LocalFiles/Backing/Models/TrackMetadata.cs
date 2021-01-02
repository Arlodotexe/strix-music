using MessagePack;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace StrixMusic.Core.LocalFileCore.Backing.Models
{
    /// <summary>
    /// Represents the metadata associated with a track.
    /// </summary>
    public class TrackMetadata
    {
        /// <summary>
        /// The unique identifier for this track.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The unique identifier for this track's album.
        /// </summary>
        public string? AlbumId { get; set; }

        /// <summary>
        /// The unique identifier(s) for this track's artist(s).
        /// </summary>
        public List<string> ArtistIds { get; set; }

        /// <summary>
        /// The display name of this track.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The duration of this track.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// The track number of this track in the album.
        /// </summary>
        public int? TrackNumber { get; set; }

        /// <summary>
        /// The disk this track is present on.
        /// </summary>
        public int? DiscNumber { get; set; }

        /// <summary>
        /// The language of this track.
        /// </summary>
        public CultureInfo? Language { get; set; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        public ICoreLyrics? Lyrics { get; set; }

        /// <summary>
        /// The external link associated with this track.
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// The description of this track.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The genres of this track.
        /// </summary>
        public List<string>? Genres { get; set; }
    }
}
