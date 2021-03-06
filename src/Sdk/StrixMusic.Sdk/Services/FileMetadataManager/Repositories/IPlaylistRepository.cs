using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    ///<inheritdoc/>
    public interface IPlaylistRepository : IMetadataRepository<PlaylistMetadata>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IReadOnlyList<PlaylistMetadata>> GetPlaylists(int offset, int limit);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playListId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IReadOnlyList<PlaylistMetadata>> GetPlaylistsByTrackId(string playListId, int offset, int limit);
    }
}
