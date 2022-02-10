using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// Provides storage for playlist metadata.
    /// </summary>
    public interface IPlaylistRepository : IMetadataRepository<PlaylistMetadata>
    {
    }
}
