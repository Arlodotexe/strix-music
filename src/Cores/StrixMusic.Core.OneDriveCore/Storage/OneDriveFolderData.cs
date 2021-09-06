using OwlCore.AbstractStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.OneDriveCore.Storage
{
    /// <summary>
    /// Implementation of <see cref="IFolderData"/> for OneDrive.
    /// </summary>
    public class OneDriveFolderData : IFolderData
    {
        /// <summary>
        /// OneDrive folder id.
        /// </summary>
        public string OneDriveFolderId { get; }

        ///<inheritdoc />
        public string Name { get; }

        ///<inheritdoc />
        public string Path { get; }

        ///<inheritdoc />
        public Task<IFileData> CreateFileAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IFolderData> CreateFolderAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task EnsureExists()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IFileData> GetFileAsync(string name)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IEnumerable<IFileData>> GetFilesAsync()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IFolderData> GetFolderAsync(string name)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IFolderData> GetParentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
