using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Events;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    ///<inheritdoc/>
    public interface IPlaylistRepository : IMetadataRepository<PlaylistMetadata>
    {
        /// <summary>
        /// Gets all <see cref="PlaylistMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IReadOnlyList<PlaylistMetadata>> GetPlaylists(int offset, int limit);

        /// <summary>
        /// Gets all <see cref="PlaylistMetadata"/> by playlist id.
        /// </summary>
        /// <param name="id">The playlist id</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<PlaylistMetadata?> GetPlaylistById(string id);

        /// <summary>
        /// Adds a playlist to the repo, or updates an existing playlist.
        /// </summary>
        /// <param name="playlistMetadata">The playlist metadata to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddOrUpdatePlaylist(PlaylistMetadata playlistMetadata);

        /// <summary>
        /// Removes an playlist from the repository.
        /// </summary>
        /// <param name="playlistMetadata">The playlist metadata to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="playlistMetadata"/> does not exist in the repository.</exception>
        Task RemovePlaylist(PlaylistMetadata playlistMetadata);
    }
}
