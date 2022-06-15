// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;

namespace StrixMusic.Sdk.FileMetadata.Models
{
    /// <summary>
    /// Holds metadata scanned from a single file.
    /// </summary>
    public sealed class FileMetadata
    {
        /// <summary>
        /// A unique identifier.
        /// </summary>
        public string? Id { get; set; }

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
}
