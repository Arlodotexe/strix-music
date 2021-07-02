using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// Provides storage for album metadata.
    /// </summary>
    public interface IAlbumRepository : IMetadataRepository<AlbumMetadata>
    {
        /// <summary>
        /// Adds an album to the repo, or updates an existing album.
        /// </summary>
        /// <param name="albumMetadata">The album metadata to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddOrUpdateAlbum(AlbumMetadata albumMetadata);

        /// <summary>
        /// Removes an album from the repository.
        /// </summary>
        /// <param name="albumMetadata">The album metadata to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="albumMetadata"/> does not exist in the repository.</exception>
        public Task RemoveAlbum(AlbumMetadata albumMetadata);

        /// <summary>
        /// Gets the <see cref="AlbumMetadata"/> by specific <see cref="AlbumMetadata"/> id. 
        /// </summary>
        /// <param name="id">The id of the corresponding <see cref="AlbumMetadata"/>.</param>
        /// <returns>If found return <see cref="AlbumMetadata"/> otherwise returns null.</returns>
        public Task<AlbumMetadata?> GetAlbumById(string id);

        /// <summary>
        /// Gets all <see cref="TrackMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<AlbumMetadata>> GetAlbums(int offset, int limit);

        /// <summary>
        /// Gets the filtered albums by artists ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{AlbumMetadata}"/>>.</returns>
        public Task<IReadOnlyList<AlbumMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit);

        /// <summary>
        /// Raised when an artist is added or removed from an album.
        /// </summary>
        public event CollectionChangedEventHandler<(AlbumMetadata Album, ArtistMetadata Artist)>? ArtistItemsChanged;

        /// <summary>
        /// Raised when a track is added or removed from an album.
        /// </summary>
        public event CollectionChangedEventHandler<(AlbumMetadata Album, TrackMetadata Track)>? TracksChanged;
    }
}