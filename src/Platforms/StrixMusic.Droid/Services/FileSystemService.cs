using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StrixMusic.Sdk.Interfaces.Storage;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc/>
    public class FileSystemService : IFileSystemService
    {
        /// <inheritdoc/>
        public FileSystemService()
        {
        }

        /// <inheritdoc/>
        public event EventHandler<IFolderData>? FolderScanStarted;

        /// <inheritdoc/>
        public event EventHandler<IFolderData>? FolderDeepScanCompleted;

        /// <inheritdoc/>
        public event EventHandler<IFolderData>? FolderScanCompleted;

        /// <inheritdoc/>
        public event EventHandler<FileScanStateEventArgs>? FileScanStarted;

        /// <inheritdoc/>
        public event EventHandler<FileScanStateEventArgs>? FileScanCompleted;

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