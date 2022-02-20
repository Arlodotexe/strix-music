// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// Provides storage for track metadata.
    /// </summary>
    public interface ITrackRepository : IMetadataRepository<TrackMetadata>
    {
        /// <summary>
        /// Gets the filtered tracks by artist ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public Task<IReadOnlyList<TrackMetadata>> GetTracksByArtistId(string artistId, int offset, int limit);

        /// <summary>
        /// Gets the filtered tracks by album ids.
        /// </summary>
        /// <param name="albumId">The album Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string albumId, int offset, int limit);
    }
}