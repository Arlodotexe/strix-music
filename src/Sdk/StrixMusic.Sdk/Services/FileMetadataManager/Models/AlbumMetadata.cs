using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// Contains information that describes an album, scanned from one or more files.
    /// </summary>
    public class AlbumMetadata : IFileMetadata
    {
        /// <inheritdoc />
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
        /// The path to the cover art for this album.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// The unique identifier(s) for this album's track(s).
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
        /// The release date of this album.
        /// </summary>
        public DateTime? DatePublished { get; set; }

        /// <summary>
        /// The genres of this album.
        /// </summary>
        public List<string>? Genres { get; set; }
    }
}
