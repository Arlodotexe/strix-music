using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.Storage;
using StrixMusic.Models;
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
            var fileScanTasks = new List<Func<Task>>();

            await RecursiveFolderScan(folderData, fileScanTasks);

            // This executes all the tasks in parallel, and has a huge speed boost on SSDs.
            // Change to using a normal foreach for slower disks.
            await Task.WhenAll(fileScanTasks.Select(Task.Run));

            FolderDeepScanCompleted?.Invoke(this, folderData);

            _registeredFolders.Add(folderData);
            return folderData;
        }

        private async Task ScanFilesInFolder(IFolderData folder)
        {
            foreach (var file in folder.Files)
            {
                if (file is FileData fileData)
                {
                    FileScanStarted?.Invoke(this, new FileScanStateEventArgs(folder, file));
                    await fileData.StartMediaScan();
                    FileScanCompleted?.Invoke(this, new FileScanStateEventArgs(folder, file));
                }
            }
        }

        /// <summary>
        /// Deep scans a folder structure.
        /// </summary>
        /// <param name="folderData">The folder to scan. Will be populated with subfolders.</param>
        /// <param name="fileScanTasks">A list containing the tasks for scanning a file. Await the tasks to scan the files.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task RecursiveFolderScan(IFolderData folderData, List<Func<Task>> fileScanTasks)
        {
            FolderScanStarted?.Invoke(this, folderData);

            if (!(folderData is FolderData folderDataInstance))
                throw new InvalidCastException($"{nameof(folderData)} is not an instance of {nameof(FolderData)}");

            await folderDataInstance.ScanFolder();

            fileScanTasks.Add(() => ScanFilesInFolder(folderData));

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
        public event EventHandler<IFolderData>? FolderScanStarted;

        /// <inheritdoc />
        public event EventHandler<IFolderData>? FolderScanCompleted;

        /// <inheritdoc />
        public event EventHandler<FileScanStateEventArgs>? FileScanStarted;

        /// <inheritdoc />
        public event EventHandler<FileScanStateEventArgs>? FileScanCompleted;

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

            if (pickedFolder == null)
                return;

            if (_registeredFolders.Any(x => x.Path == pickedFolder.Path))
                return;

            var storageToken = StorageApplicationPermissions.FutureAccessList.Add(pickedFolder, pickedFolder.Path);

            _ = StorageFolderToFolderData(pickedFolder);
        }

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder)
        {
            if (_registeredFolders.Contains(folder) && folder is FolderData folderData)
            {
                var targetFutureAccessListFile = StorageApplicationPermissions.FutureAccessList.Entries
                    .FirstOrDefault(x => x.Metadata == folderData.StorageFolder.Path);

                StorageApplicationPermissions.FutureAccessList.Remove(targetFutureAccessListFile.Token);

                _registeredFolders.Remove(folder);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task Init()
        {
            var persistentAccessEntries = StorageApplicationPermissions.FutureAccessList.Entries;

            if (persistentAccessEntries == null || !persistentAccessEntries.Any())
                return;

            var folderScanTasks = new List<Func<Task>>();
            foreach (var accessEntry in persistentAccessEntries)
            {
                var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(accessEntry.Token);
                if (folder == null)
                    return;

                folderScanTasks.Add(() => StorageFolderToFolderData(folder));
            }

            await Task.WhenAll(folderScanTasks.Select(Task.Run));
        }
    }
}
