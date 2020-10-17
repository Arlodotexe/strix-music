using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Uno.Models;
using Windows.Storage;

// ReSharper disable once CheckNamespace
namespace OwlCore.AbstractStorage
{
    /// <inheritdoc/>
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

        /// <summary>
        /// Constructs a new <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService(StorageFolder rootFolder)
        {
            _registeredFolders = new List<IFolderData>();

            RootFolder = new FolderData(rootFolder);
        }

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
        public async Task<IFolderData> CreateDirectoryAsync(string folderName)
        {
            var folderData = await RootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            return folderData;
        }

        /// <inheritdoc/>
        public async Task Init()
        {
            await RootFolder.EnsureExists();
        }
    }
}