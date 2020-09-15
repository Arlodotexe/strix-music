using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Interfaces.Storage;
using StrixMusic.Shared.Models;
using Windows.Storage;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc/>
    public class FileSystemService : IFileSystemService
    {
        /// <inheritdoc/>
        public IFolderData RootFolder { get; }

        /// <summary>
        /// Creates a new instance of <see cref="FileSystemService"/>.
        /// </summary>
        /// <param name="rootFolder"><inheritdoc cref="RootFolder"/></param>
        public FileSystemService(StorageFolder rootFolder)
        {
            RootFolder = new FolderData(rootFolder);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService()
        {
            RootFolder = new FolderData(ApplicationData.Current.LocalFolder);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData?>> GetPickedFolders()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IFolderData?> PickFolder()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> FileExistsAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> DirectoryExistsAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IFolderData> CreateDirectoryAsync(string folderName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task Init()
        {
            // TODO
            return Task.CompletedTask;
        }
    }
}