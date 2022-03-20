using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Uno.Extensions;
using StrixMusic.Sdk.Uno.Models;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

// ReSharper disable once CheckNamespace
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
namespace StrixMusic.Services
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

        /// <summary>
        /// A shared singleton instance of <see cref="FileSystemService"/>.
        /// </summary>
        public static FileSystemService Singleton { get; } = new FileSystemService();

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

#if NETFX_CORE
            StorageApplicationPermissions.FutureAccessList.Add(pickedFolder, pickedFolder.Path);
#endif
            var folderData = new FolderData(pickedFolder);

            _registeredFolders.Add(folderData);

            return folderData;
        }

        /// <inheritdoc/>
        public Task RevokeAccess(IFolderData folder)
        {
            if (!_registeredFolders.Contains(folder) || !(folder is FolderData folderData))
                return Task.CompletedTask;

#if NETFX_CORE
            var targetFutureAccessListFile = StorageApplicationPermissions.FutureAccessList.Entries
                .FirstOrDefault(x => x.Metadata == folderData.StorageFolder.Path);

            StorageApplicationPermissions.FutureAccessList.Remove(targetFutureAccessListFile.Token);
#endif

            _registeredFolders.Remove(folder);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<bool> FileExistsAsync(string path)
        {
            try
            {
                var res = await StorageFile.GetFileFromPathAsync(path);
                return res != null;
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
        public Task<IFolderData?> GetFolderFromPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException();

            var folderData = _registeredFolders.FirstOrDefault(x => x.Path == path);
            if (folderData is null)
                return Task.FromResult<IFolderData?>(null);
            else
                return Task.FromResult<IFolderData?>(folderData);

            // https://github.com/unoplatform/uno/issues/7401
            // var folderData = await StorageFolder.GetFolderFromPathAsync(path);
        }

        /// <inheritdoc/>
        public async Task<IFileData?> GetFileFromPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException();

            var file = await StorageFile.GetFileFromPathAsync(path);

            return new FileData(file);
        }

        /// <inheritdoc/>
        public async Task<IFolderData> CreateDirectoryAsync(string folderName)
        {
            var folderData = await RootFolder.CreateFolderAsync(folderName, OwlCore.AbstractStorage.CreationCollisionOption.OpenIfExists);

            return folderData;
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
#if NETFX_CORE
            var persistentAccessEntries = StorageApplicationPermissions.FutureAccessList.Entries.ToArray();

            if (persistentAccessEntries == null || !persistentAccessEntries.Any())
                return;

            foreach (var accessEntry in persistentAccessEntries)
            {
                StorageFolder? folder = null;

                try
                {
                    folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(accessEntry.Token);
                }
                catch
                {
                    // Folder may have been removed.
                    StorageApplicationPermissions.FutureAccessList.Remove(accessEntry.Token);
                }

                if (folder == null)
                    continue;

                var folderData = new FolderData(folder);

                _registeredFolders.Add(folderData);
            }
#endif

            IsInitialized = true;
        }
    }
}

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
