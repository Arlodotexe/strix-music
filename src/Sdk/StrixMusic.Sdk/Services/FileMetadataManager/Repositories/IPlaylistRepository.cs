using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <summary>
    /// Provides storage for playlist metadata.
    /// </summary>
    public interface IPlaylistRepository : IMetadataRepository<PlaylistMetadata>
    {
    }
}
