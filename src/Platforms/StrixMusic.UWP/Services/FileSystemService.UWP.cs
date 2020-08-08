using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strix_Music.Models;
using StrixMusic.CoreInterfaces.Interfaces.Storage;
using StrixMusic.UWP.Models;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace StrixMusic.Services.StorageService
{
    /// <inheritdoc cref="IFileSystemService" />
    public class FileSystemService : IFileSystemService
    {
        /// <summary>
        /// Recursively converts and processes a <see cref="StorageFolder"/> to an instance of <see cref="IFolderData"/>.
        /// </summary>
        /// <param name="storageFolder">The storage file to process.</param>
        /// <returns>An instance of <see cref="IFolderData"/> containing all subfolder and file data.</returns>
        public async Task<IFolderData> StorageFolderToFolderData(StorageFolder storageFolder)
        {
            var folderData = new FolderData(storageFolder);

            var fileScanTasks = new List<Task>();
            await RecursiveFolderScan(folderData, fileScanTasks);

            Task.WaitAll(fileScanTasks.ToArray(), System.Threading.CancellationToken.None);

            FolderDeepScanCompleted?.Invoke(this, folderData);

            _registeredFolders.Add(folderData);
            return folderData;
        }

        private async Task ScanFilesInFolder(IFolderData folder, IProgress<IFolderData> progress)
        {
            foreach (var file in folder.Files)
            {
                if (file is FileData fileData)
                {
                    FileScanStarted?.Invoke(this, fileData);
                    await fileData.StartMediaScan();
                    FileScanCompleted?.Invoke(this, fileData);
                    progress.Report(folder);
                }
            }
        }

        /// <summary>
        /// Deep scans a folder structure.
        /// </summary>
        /// <param name="folderData">The folder to scan. Will be populated with subfolders.</param>
        /// <param name="fileScanTasks">A list containing the tasks for scanning a file. Await the tasks to scan the files.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task RecursiveFolderScan(IFolderData folderData, List<Task> fileScanTasks)
        {
            var progress = (IProgress<IFolderData>)new Progress<IFolderData>();

            FolderScanStarted?.Invoke(this, (Progress<IFolderData>)progress);

            if (!(folderData is FolderData folderDataInstance))
                throw new InvalidCastException($"{nameof(folderData)} is not an instance of {nameof(FolderData)}");

            await folderDataInstance.ScanFolder();

            progress.Report(folderData);

            fileScanTasks.Add(ScanFilesInFolder(folderData, progress));

            FolderScanCompleted?.Invoke(this, folderData);

            foreach (var subFolder in folderData.Folders)
            {
                if (subFolder is FolderData subfolderData)
                {
                    await RecursiveFolderScan(subFolder, fileScanTasks);
                }
            }
        }

        private readonly List<IFolderData> _registeredFolders;

        /// <inheritdoc />
        public event EventHandler<Progress<IFolderData>>? FolderScanStarted;

        /// <inheritdoc />
        public event EventHandler<IFolderData>? FolderScanCompleted;

        /// <inheritdoc />
        public event EventHandler<IFileData>? FileScanStarted;

        /// <inheritdoc />
        public event EventHandler<IFileData>? FileScanCompleted;

        /// <inheritdoc />
        public event EventHandler<IFolderData>? FolderDeepScanCompleted;

        /// <summary>
        /// Constructs a new <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService()
        {
            _registeredFolders = new List<IFolderData>();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData>> GetPickedFolders()
        {
            return Task.FromResult<IReadOnlyList<IFolderData>>(_registeredFolders.ToArray());
        }

        /// <inheritdoc/>
        public async Task PickFolder()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add(".mp3");
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            picker.ViewMode = PickerViewMode.List;

            var pickedFolder = await picker.PickSingleFolderAsync();

            StorageApplicationPermissions.FutureAccessList.Add(pickedFolder);

            _ = StorageFolderToFolderData(pickedFolder);
        }

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder)
        {
            if (_registeredFolders.Contains(folder) && folder is FolderData folderData)
            {
                StorageApplicationPermissions.FutureAccessList.Add(folderData.StorageFolder);
                _registeredFolders.Remove(folder);
            }

            return Task.CompletedTask;
        }
    }
}
