using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Interfaces.Storage;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <summary>
    /// A service that stores file in a cache folder.
    /// </summary>
    public abstract class CacheServiceBase : IFileSystemService
    {
        private readonly IFileSystemService _fileSystemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheServiceBase"/> class.
        /// </summary>
        /// <param name="fileSystemService">The file system service to use for caching.</param>
        protected CacheServiceBase(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        /// <inheritdoc />
        public IFolderData RootFolder => _fileSystemService.RootFolder;

        /// <inheritdoc/>
        public Task Init() => _fileSystemService.Init();

        /// <inheritdoc/>
        public Task<IFolderData?> PickFolder() => _fileSystemService.PickFolder();

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData?>> GetPickedFolders() => _fileSystemService.GetPickedFolders();

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder) => _fileSystemService.RevokeAccess(folder);

        /// <inheritdoc/>
        public Task<bool> FileExistsAsync(string path) => _fileSystemService.FileExistsAsync(path);

        /// <inheritdoc/>
        public Task<bool> DirectoryExistsAsync(string path) => _fileSystemService.DirectoryExistsAsync(path);

        /// <inheritdoc/>
        public Task<IFolderData> CreateDirectoryAsync(string folderName) => _fileSystemService.CreateDirectoryAsync(folderName);

        /// <summary>
        /// A unique identifier for this instance of the cache that sections off the data.
        /// </summary>
        public abstract string Id { get; protected set; }
    }
}