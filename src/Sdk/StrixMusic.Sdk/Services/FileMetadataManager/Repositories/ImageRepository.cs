using OwlCore.AbstractStorage;

using StrixMusic.Sdk.Services.FileMetadataManager.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <inheritdoc/>
    public class ImageRepository : IImageRepository
    {
        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataUpdated;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataRemoved;
        
        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataAdded;

        /// <inheritdoc/>
        public Task<int> GetItemCount()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task InitAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetDataFolder(IFolderData rootFolder)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        ~ImageRepository()
        {
            Dispose(false);
        }
    }
}
