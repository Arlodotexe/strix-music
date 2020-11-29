﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Services;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc/>
    public abstract class CacheServiceBase : IFileSystemService
    {
        private readonly IFileSystemService _cacheStorageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheServiceBase"/> class.
        /// </summary>
        protected CacheServiceBase()
        {
            var contextContextualServiceLocator = Ioc.Default.GetService<ContextualServiceLocator>() ?? ThrowHelper.ThrowInvalidOperationException<ContextualServiceLocator>();

            _cacheStorageService = contextContextualServiceLocator.GetServiceByContext<IFileSystemService>(typeof(CacheServiceBase));
        }

        /// <inheritdoc />
        public IFolderData RootFolder => _cacheStorageService.RootFolder;

        /// <inheritdoc/>
        public Task InitAsync() => _cacheStorageService.InitAsync();

        /// <inheritdoc/>
        public Task<IFolderData?> PickFolder() => _cacheStorageService.PickFolder();

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData?>> GetPickedFolders() => _cacheStorageService.GetPickedFolders();

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder) => _cacheStorageService.RevokeAccess(folder);

        /// <inheritdoc/>
        public Task<bool> FileExistsAsync(string path) => _cacheStorageService.FileExistsAsync(path);

        /// <inheritdoc/>
        public Task<bool> DirectoryExistsAsync(string path) => _cacheStorageService.DirectoryExistsAsync(path);

        /// <inheritdoc/>
        public Task<IFolderData> CreateDirectoryAsync(string folderName) => _cacheStorageService.CreateDirectoryAsync(folderName);

        /// <summary>
        /// A unique identifier for this instance of the cache that sections off the data.
        /// </summary>
        public abstract string Id { get; protected set; }
    }
}