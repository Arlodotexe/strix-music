using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.FileMetadataManagement.Models
{
    /// <summary>
    /// Contains information that describes an artist, scanned from one or more files.
    /// </summary>
    public sealed class ArtistMetadata : IFileMetadata
    {
        /// <summary>
        /// The unique identifier for this artist.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The unique identifier(s) for <see cref="TrackMetadata"/>.
        /// </summary>
        public HashSet<string>? TrackIds { get; set; }

        /// <summary>
        /// Holds unique identifier(s) for the <see cref="AlbumMetadata"/>.
        /// </summary>
        public HashSet<string>? AlbumIds { get; set; }

        /// <summary>
        /// Holds the name of the artist.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Probably bio of the artist.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The external link associated with the artist.
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// The unique identifier(s) for this artist's image(s).
        /// </summary>
        public HashSet<string>? ImageIds { get; set; }

        /// <summary>
        /// The genres of this track.
        /// </summary>
        public HashSet<string>? Genres { get; set; }
    }
}
