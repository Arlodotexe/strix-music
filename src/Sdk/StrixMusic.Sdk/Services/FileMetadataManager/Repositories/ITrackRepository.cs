using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// Provides storage for track metadata.
    /// </summary>
    public interface ITrackRepository : IMetadataRepository<TrackMetadata>
    {
        /// <summary>
        /// Adds an track to the repo, or updates an existing track.
        /// </summary>
        /// <param name="trackMetadata">The track metadata to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddOrUpdateTrack(TrackMetadata trackMetadata);

        /// <summary>
        /// Removes an track from the repository.
        /// </summary>
        /// <param name="trackMetadata">The track metadata to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="trackMetadata"/> does not exist in the repository.</exception>
        public Task RemoveTrack(TrackMetadata trackMetadata);

        /// <summary>
        /// Gets <see cref="TrackMetadata"/>s stored in the repository. -1 as a limit will return all tracks.
        /// </summary>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="limit">Get items starting at this index.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<TrackMetadata>> GetTracks(int offset, int limit);

        /// <summary>
        /// Gets the <see cref="TrackMetadata"/> by specific <see cref="TrackMetadata"/> id. 
        /// </summary>
        /// <param name="id">The id of the corresponding <see cref="TrackMetadata"/></param>
        /// <returns>If found return <see cref="TrackMetadata"/> otherwise returns null.</returns>
        public Task<TrackMetadata?> GetTrackById(string id);

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

        /// <summary>
        /// Raised when an artist is added or removed from an album.
        /// </summary>
        public event CollectionChangedEventHandler<(TrackMetadata Track, ArtistMetadata Artist)>? ArtistItemsChanged;
    }
}