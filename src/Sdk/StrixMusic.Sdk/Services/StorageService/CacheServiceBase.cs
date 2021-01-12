using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc/>
    public abstract class CacheServiceBase : IFileSystemService
    {
        private IFileSystemService? _cacheStorageService;

        /// <inheritdoc />
        public IFolderData RootFolder => _cacheStorageService?.RootFolder ??
                                         ThrowHelper.ThrowInvalidOperationException<IFolderData>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task<IFolderData?> PickFolder() => _cacheStorageService?.PickFolder() ??
                                                  ThrowHelper.ThrowInvalidOperationException<Task<IFolderData?>>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData?>> GetPickedFolders() => _cacheStorageService?.GetPickedFolders() ??
                                                                       ThrowHelper.ThrowInvalidOperationException<Task<IReadOnlyList<IFolderData?>>>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder) => _cacheStorageService?.RevokeAccess(folder) ??
                                                        ThrowHelper.ThrowInvalidOperationException<Task>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task<bool> FileExistsAsync(string path) => _cacheStorageService?.FileExistsAsync(path) ??
                                                          ThrowHelper.ThrowInvalidOperationException<Task<bool>>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task<bool> DirectoryExistsAsync(string path) => _cacheStorageService?.DirectoryExistsAsync(path) ??
                                                               ThrowHelper.ThrowInvalidOperationException<Task<bool>>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task<IFolderData> CreateDirectoryAsync(string folderName) => _cacheStorageService?.CreateDirectoryAsync(folderName) ??
                                                                            ThrowHelper.ThrowInvalidOperationException<Task<IFolderData>>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <inheritdoc/>
        public Task<IFolderData?> GetFolderFromPathAsync(string path) => _cacheStorageService?.GetFolderFromPathAsync(path) ??
                                                                         ThrowHelper.ThrowInvalidOperationException<Task<IFolderData?>>($"Tried to use {nameof(_cacheStorageService)} before it was initialized.");

        /// <summary>
        /// A unique identifier for this instance of the cache that sections off the data.
        /// </summary>
        public abstract string Id { get; protected set; }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            var sharedFactory = Ioc.Default.GetRequiredService<ISharedFactory>();
            _cacheStorageService = await sharedFactory.CreateFileSystemServiceForCache(Id);

            await _cacheStorageService.InitAsync();
        }
    }
}