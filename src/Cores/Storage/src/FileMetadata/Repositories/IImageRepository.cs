using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// Provides storage for image metadata.
/// </summary>
internal interface IImageRepository : IMetadataRepository<ImageMetadata>
{
}