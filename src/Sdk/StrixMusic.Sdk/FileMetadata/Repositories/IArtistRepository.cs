// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// Provides storage for artist metadata.
    /// </summary>
    public interface IArtistRepository : IMetadataRepository<ArtistMetadata>
    {
        /// <summary>
        /// Gets the filtered artist by album ids.
        /// </summary>
        /// <param name="albumId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public Task<IReadOnlyList<ArtistMetadata>> GetArtistsByAlbumId(string albumId, int offset, int limit);

        /// <summary>
        /// Gets the artists by track Id.
        /// </summary>
        /// <param name="trackId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>>.</returns>
        public Task<IReadOnlyList<ArtistMetadata>> GetArtistsByTrackId(string trackId, int offset, int limit);
    }
}