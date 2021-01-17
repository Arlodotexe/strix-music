using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaunchPad.Extensions;
using StrixMusic.Sdk.Uno.Models;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

// ReSharper disable once CheckNamespace
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
namespace OwlCore.AbstractStorage
{
    /// <inheritdoc cref="IFileSystemService" />
    public sealed class FileSystemService : IFileSystemService
    {
        private readonly List<IFolderData> _registeredFolders;

        /// <summary>
        /// Constructs a new <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService()
        {
            _registeredFolders = new List<IFolderData>();
        }

        /// <summary>
        /// Constructs a new <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService(StorageFolder rootFolder)
        {
            _registeredFolders = new List<IFolderData>();

            RootFolder = new FolderData(rootFolder);
        }

        /// <inheritdoc />
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Defines the root folder where new files and folders are created.
        /// </summary>
        public IFolderData RootFolder { get; } = new FolderData(ApplicationData.Current.LocalFolder);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IFolderData?>> GetPickedFolders()
        {
            return Task.FromResult<IReadOnlyList<IFolderData?>>(_registeredFolders.ToArray());
        }

        /// <inheritdoc/>
        public async Task<IFolderData?> PickFolder()
        {
            await CoreApplication.MainView.Dispatcher.SwitchToUI();

            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.MusicLibrary,
                ViewMode = PickerViewMode.List,
            };

            picker.FileTypeFilter.Add("*");

            var pickedFolder = await picker.PickSingleFolderAsync();
            if (pickedFolder == null)
                return null;

            StorageApplicationPermissions.FutureAccessList.Add(pickedFolder, pickedFolder.Path);

            var folderData = new FolderData(pickedFolder);

            _registeredFolders.Add(folderData);

            return folderData;
        }

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder)
        {
            if (!_registeredFolders.Contains(folder) || !(folder is FolderData folderData))
                return Task.CompletedTask;

            var targetFutureAccessListFile = StorageApplicationPermissions.FutureAccessList.Entries
                .FirstOrDefault(x => x.Metadata == folderData.StorageFolder.Path);

            StorageApplicationPermissions.FutureAccessList.Remove(targetFutureAccessListFile.Token);

            _registeredFolders.Remove(folder);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<bool> FileExistsAsync(string path)
        {
            try
            {
                await StorageFile.GetFileFromPathAsync(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DirectoryExistsAsync(string path)
        {
            try
            {
                await StorageFile.GetFileFromPathAsync(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<IFolderData?> GetFolderFromPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException();

            var folderData = await StorageFolder.GetFolderFromPathAsync(path);

            return new FolderData(folderData);
        }

        /// <inheritdoc/>
        public async Task<IFolderData> CreateDirectoryAsync(string folderName)
        {
            var folderData = await RootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            return folderData;
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            await RootFolder.EnsureExists();

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

            IsInitialized = true;
        }
    }
}

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.