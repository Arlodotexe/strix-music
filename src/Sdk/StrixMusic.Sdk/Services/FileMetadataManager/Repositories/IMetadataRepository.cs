using System;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// A repository that provides access to metadata scanned from a file.
    /// </summary>
    public interface IMetadataRepository : IAsyncInit, IDisposable
    {
        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to save data in.</param>
        public void SetDataFolder(IFolderData rootFolder);
    }
}