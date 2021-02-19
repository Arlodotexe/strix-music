using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// Holds the information of artist metadata.
    /// </summary>
    public class ArtistMetadata
    {

        /// <summary>
        /// The unique identifier for the <see cref="ArtistMetadata"/>.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The unique identifier(s) for <see cref="TrackMetadata"/>.
        /// </summary>
        public List<string>? TrackIds { get; set; }

        /// <summary>
        /// Holds unique identifier(s) for the <see cref="AlbumMetadata"/>.
        /// </summary>
        public List<string>? AlbumIds { get; set; }

        /// <summary>
        /// Holds the name of the artist.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The total number of images related to the artist.
        /// </summary>
        public int TotalImageCount { get; set; }

        /// <summary>
        /// The total number of tracks for the artist.
        /// </summary>
        public int TotalTracksCount { get; set; }

        /// <summary>
        /// Probably bio of the artist.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The external link associated with the artist.
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// The picture of the artist.
        /// </summary>
        public Uri? ImagePath { get; set; }

        /// <summary>
        /// The genres of this track.
        /// </summary>
        public List<string>? Genres { get; set; }
    }
}
