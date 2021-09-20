using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <summary>
    /// Provides storage for image metadata.
    /// </summary>
    public interface IImageRepository : IMetadataRepository<ImageMetadata>
    {
    }
}
