using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.LocalFiles.Backing.Models
{
    /// <summary>
    /// Holds an instance of <see cref="RelatedMetadata"/>.
    /// </summary>
    public class RelatedMetadata
    {
        /// <summary>
        /// Holds an id of <see cref="RelatedMetadata"/>.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Holds an a <see cref="TrackMetadata"/>.
        /// </summary>
        public TrackMetadata? TrackMetadata { get; set; }

        /// <summary>
        /// Holds an a <see cref="AlbumMetadata"/>.
        /// </summary>
        public AlbumMetadata? AlbumMetadata { get; set; }

        /// <summary>
        /// Holds an a <see cref="ArtistMetadata"/>.
        /// </summary>
        public ArtistMetadata? ArtistMetadata { get; set; }

    }
}
