using OwlCore.AbstractStorage;
using StrixMusic.Core.OneDriveCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.OneDriveCore.Storage
{
    /// <summary>
    /// Implementation of <see cref="IFolderData"/> for OneDrive.
    /// </summary>
    public class OneDriveFolderData : IFolderData
    {
        private OneDriveCoreStorageService _oneDriveStorageService;

        /// <summary>
        /// Holds the children of the folder.
        /// </summary>
        public IEnumerable<IFolderData> ChildrenCache { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="OneDriveFolderData"/>.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <param name="path">The web url of the folder.</param>
        /// <param name="oneDriveFolderId">The id of the folder.</param>
        public OneDriveFolderData(OneDriveCoreStorageService oneDriveCoreStorageService, string name, string path, string oneDriveFolderId, bool isRoot = false)
        {
            Name = name;
            Path = path;
            OneDriveFolderId = oneDriveFolderId;
            IsRoot = isRoot;

            _oneDriveStorageService = oneDriveCoreStorageService;
        }

        /// <summary>
        /// Flag that indicates whether a folder is at the root or not.
        /// </summary>
        public bool IsRoot { get; set; }

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
            var folder = ChildrenCache.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(folder);
        }

        ///<inheritdoc />
        public async Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            if (ChildrenCache == null)
            {
                ChildrenCache = await _oneDriveStorageService.GetFolderChildren(OneDriveFolderId, IsRoot);

                return ChildrenCache;
            }

            return ChildrenCache;
        }

        ///<inheritdoc />
        public Task<IFolderData> GetParentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
