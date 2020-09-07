using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Interfaces.Storage;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc/>
    public class FileSystemService : IFileSystemService
    {
        /// <inheritdoc/>
        public event EventHandler<IFolderData> FolderScanStarted;

        /// <inheritdoc/>
        public event EventHandler<IFolderData> FolderDeepScanCompleted;

        /// <inheritdoc/>
        public event EventHandler<IFolderData> FolderScanCompleted;

        /// <inheritdoc/>
        public event EventHandler<FileScanStateEventArgs> FileScanStarted;

        /// <inheritdoc/>
        public event EventHandler<FileScanStateEventArgs> FileScanCompleted;

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
        public Task Init()
        {
            throw new NotImplementedException();
        }
    }
}