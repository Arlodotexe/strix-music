using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// Provides storage for artist metadata.
/// </summary>
internal interface IArtistRepository : IMetadataRepository<ArtistMetadata>
{
    /// <summary>
    /// A unique identifier for this repo.
    /// </summary>
    public string Id { get; }
        
    /// <summary>
    /// Gets the filtered artist by album ids.
    /// </summary>
    /// <param name="albumId">The artist Id.</param>
    /// <param name="offset">The starting index for retrieving items.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <returns>The filtered <see cref="IReadOnlyList{T}"/>></returns>
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
