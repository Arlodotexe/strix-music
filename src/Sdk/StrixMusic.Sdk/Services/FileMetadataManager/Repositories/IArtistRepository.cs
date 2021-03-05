using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// Provides storage for artist metadata
    /// </summary>
    public interface IArtistRepository : IMetadataRepository<ArtistMetadata>
    {
        /// <summary>
        /// Adds an artist to the repo, or updates an existing artist.
        /// </summary>
        /// <param name="artistMetadata">The artist metadata to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddOrUpdateArtist(ArtistMetadata artistMetadata);

        /// <summary>
        /// Removes an artist from the repository.
        /// </summary>
        /// <param name="artistMetadata">The artist metadata to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="artistMetadata"/> does not exist in the repository.</exception>
        public Task RemoveArtist(ArtistMetadata artistMetadata);

        /// <summary>
        /// Gets the <see cref="ArtistMetadata"/> by specific <see cref="ArtistMetadata"/> id. 
        /// </summary>
        /// <param name="id">The id of the corresponding <see cref="ArtistMetadata"/></param>
        /// <returns>If found return <see cref="ArtistMetadata"/> otherwise returns null.</returns>
        public Task<ArtistMetadata?> GetArtistById(string id);

        /// <summary>
        /// Gets all <see cref="TrackMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ArtistMetadata>> GetArtists(int offset, int limit);

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

        /// <summary>
        /// Raised when an artist is added or removed from an album.
        /// </summary>
        public event CollectionChangedEventHandler<(ArtistMetadata Artist, AlbumMetadata Album)>? AlbumItemsChanged;

        /// <summary>
        /// Raised when a track is added or removed from an album.
        /// </summary>
        public event CollectionChangedEventHandler<(ArtistMetadata Artist, TrackMetadata Track)>? TracksChanged;
    }
}