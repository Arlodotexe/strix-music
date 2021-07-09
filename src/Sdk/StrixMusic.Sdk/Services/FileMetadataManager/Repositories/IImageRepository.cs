using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <summary>
    /// Provides storage for image metadata.
    /// </summary>
    public interface IImageRepository : IMetadataRepository<ImageMetadata>
    {
        /// <summary>
        /// Gets an <see cref="IReadOnlyList{T}"/> of <see cref="ImageMetadata"/> by album ID.
        /// </summary>
        /// <param name="albumId">The ID of the album to get the images of.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ImageMetadata>> GetImagesByAlbumIdAsync(string albumId, int offset, int limit);

        /// <summary>
        /// Gets an <see cref="IReadOnlyList{T}"/> of <see cref="ImageMetadata"/> by artist ID.
        /// </summary>
        /// <param name="artistId">The ID of the artist to get the images of.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ImageMetadata>> GetImagesByArtistIdAsync(string artistId, int offset, int limit);

        /// <summary>
        /// Gets an <see cref="IReadOnlyList{T}"/> of <see cref="ImageMetadata"/> by track ID.
        /// </summary>
        /// <param name="trackId">The ID of the track to get the images of.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ImageMetadata>> GetImagesByTrackIdAsync(string trackId, int offset, int limit);
    }
}
