using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// The metadata associated with an album.
    /// </summary>
    public class AlbumMetadata
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
        /// The unique identifier(s) for this albums's track(s).
        /// </summary>
        public List<string>? TrackIds { get; set; }

        /// <summary>
        /// The unique identifier(s) for this album's artist(s).
        /// </summary>
        public List<string>? ArtistIds { get; set; }

        /// <summary>
        /// The total duration of this album.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// The total number of tracks in this album.
        /// </summary>
        public int? TotalTracksCount { get; set; }

        /// <summary>
        /// The total number of artists of this album.
        /// </summary>
        public int? TotalArtistsCount { get; set; }

        /// <summary>
        /// The release date of this album.
        /// </summary>
        public DateTime? DatePublished { get; set; }

        /// <summary>
        /// The genres of this album.
        /// </summary>
        public List<string>? Genres { get; set; }
    }
}
