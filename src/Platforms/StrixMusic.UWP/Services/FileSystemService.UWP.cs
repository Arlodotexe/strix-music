using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.Sdk.Interfaces.Storage;
using StrixMusic.Models;
using StrixMusic.UWP.Models;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc cref="IFileSystemService" />
    public class FileSystemService : IFileSystemService
    {
        private readonly List<IFolderData> _registeredFolders;

        /// <summary>
        /// Constructs a new <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService()
        {
            _registeredFolders = new List<IFolderData>();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData?>> GetPickedFolders()
        {
            return Task.FromResult<IReadOnlyList<IFolderData?>>(_registeredFolders.ToArray());
        }

        /// <inheritdoc/>
        public async Task<IFolderData?> PickFolder()
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.MusicLibrary,
                ViewMode = PickerViewMode.List,
            };

            picker.FileTypeFilter.Add("*");

            var pickedFolder = await picker.PickSingleFolderAsync();

            if (pickedFolder == null)
                return null;

            var storageToken = StorageApplicationPermissions.FutureAccessList.Add(pickedFolder, pickedFolder.Path);

            var folderData = new FolderData(pickedFolder);

            _registeredFolders.Add(folderData);

            return folderData;
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

            foreach (var accessEntry in persistentAccessEntries)
            {
                var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(accessEntry.Token);
                if (folder == null)
                    return;

                var folderData = new FolderData(folder);

                _registeredFolders.Add(folderData);
            }
        }
    }
}
