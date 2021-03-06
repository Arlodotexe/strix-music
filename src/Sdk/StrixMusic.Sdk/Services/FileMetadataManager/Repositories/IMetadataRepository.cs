using System;
using System.Collections.Generic;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// A repository that provides access to metadata scanned from a file.
    /// </summary>
    public interface IMetadataRepository<TFileMetadata> : IAsyncInit, IDisposable
        where TFileMetadata : IFileMetadata
    {
        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to save data in.</param>
        public void SetDataFolder(IFolderData rootFolder);

        /// <summary>
        /// Raised when metadata is updated.
        /// </summary>
        public event EventHandler<IEnumerable<TFileMetadata>>? MetadataUpdated;

        /// <summary>
        /// Raised metadata is removed.
        /// </summary>
        public event EventHandler<IEnumerable<TFileMetadata>>? MetadataRemoved;

        /// <summary>
        /// Raised when new metadata is added.
        /// </summary>
        public event EventHandler<IEnumerable<TFileMetadata>>? MetadataAdded;
    }
}