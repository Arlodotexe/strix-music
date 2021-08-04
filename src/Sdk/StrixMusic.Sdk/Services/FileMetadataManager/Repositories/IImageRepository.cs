using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <summary>
    /// Provides storage for image metadata.
    /// </summary>
    public interface IImageRepository : IMetadataRepository<ImageMetadata>
    {
        /// <summary>
        /// Adds an image to the repository, or updates an existing image.
        /// </summary>
        /// <param name="imageMetadata">The image metadata to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddOrUpdateImageAsync(ImageMetadata imageMetadata);

        /// <summary>
        /// Removes an image from the repository.
        /// </summary>
        /// <param name="imageMetadata">The image metadata to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="imageMetadata"/> does not exist in the repository.</exception>
        public Task RemoveImageAsync(ImageMetadata imageMetadata);

        /// <summary>
        /// Gets an <see cref="ImageMetadata"/> corresponding to the specified ID.
        /// </summary>
        /// <param name="id">The ID of the corresponding <see cref="ImageMetadata"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<ImageMetadata> GetImageByIdAsync(string id);
    }
}
