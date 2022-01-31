using StrixMusic.Sdk.FileMetadataManagement.Models;

namespace StrixMusic.Sdk.FileMetadataManagement.Repositories
{
    /// <summary>
    /// Provides storage for playlist metadata.
    /// </summary>
    public interface IPlaylistRepository : IMetadataRepository<PlaylistMetadata>
    {
    }
}
