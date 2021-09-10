using OwlCore.AbstractStorage;
using StrixMusic.Cores.OneDrive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Interception;

namespace StrixMusic.Cores.OneDrive.Storage
{
    /// <summary>
    /// Implementation of <see cref="IFolderData"/> for OneDrive.
    /// </summary>
    public class OneDriveFolderData : IFolderData
    {
        private readonly OneDriveCoreStorageService _oneDriveStorageService;
        private IEnumerable<IFolderData> _childrenCache { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="OneDriveFolderData"/>.
        /// </summary>
        /// <param name="oneDriveCoreStorageService">The service that handles graph api requests.</param>
        /// <param name="name">The name of the folder.</param>
        /// <param name="path">The web url of the folder.</param>
        /// <param name="oneDriveFolderId">The id of the folder.</param>
        public OneDriveFolderData(OneDriveCoreStorageService oneDriveCoreStorageService, string name, string path, string oneDriveFolderId, bool isRoot = false)
        {
            Name = name;
            Path = path;
            OneDriveFolderId = oneDriveFolderId;
            _childrenCache = new List<IFolderData>();

            IsRoot = isRoot;
            _oneDriveStorageService = oneDriveCoreStorageService;
        }

        /// <summary>
        /// A unique identifier returned from OneDrive api, a folder can be uniquely identified by this id. Helpful during record reads.
        /// </summary>
        public string OneDriveFolderId { get; }

        ///<inheritdoc />
        public string Name { get; }

        ///<inheritdoc />
        public string Path { get; }

        /// <summary>
        /// Flag that determines if the folder is root or not. Useful when dealing with folder explorers.
        /// </summary>
        public bool IsRoot { get; set; }

        ///<inheritdoc />
        public Task<IFileData> CreateFileAsync(string desiredName)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
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
            var folder = _childrenCache.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(folder);
        }

        ///<inheritdoc />
        public async Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            if (_childrenCache.Any())
                return _childrenCache;

            _childrenCache = await _oneDriveStorageService.GetItemsAsync(this, IsRoot);

            return _childrenCache;

        }

        ///<inheritdoc />
        public Task<IFolderData> GetParentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
