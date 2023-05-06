using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// Provides storage for playlist metadata.
/// </summary>
internal interface IPlaylistRepository : IMetadataRepository<PlaylistMetadata>
{
}