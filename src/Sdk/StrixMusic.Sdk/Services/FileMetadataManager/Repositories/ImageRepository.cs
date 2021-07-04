using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <summary>
    /// The service that helps in interacting with image information.
    /// </summary>
    public class ImageRepository : IImageRepository
    {
        private readonly SemaphoreSlim _initMutex;

        /// <summary>
        /// Creates a new instance of <see cref="ImageRepository"/>.
        /// </summary>
        public ImageRepository()
        {
            _initMutex = new SemaphoreSlim(1, 1);
        }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataUpdated;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataRemoved;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataAdded;

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            await _initMutex.WaitAsync();
            if (IsInitialized)
            {
                _initMutex.Release();
                return;
            }

            IsInitialized = true;
            _initMutex.Release();
        }

        /// <inheritdoc/>
        public Task<int> GetItemCount()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetDataFolder(IFolderData rootFolder)
        {
            throw new NotImplementedException();
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));

            ReleaseUnmanagedResources();
            if (disposing)
            {
                // TODO
            }

            IsInitialized = false;
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
