using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using Windows.Storage;
using CreationCollisionOption = OwlCore.AbstractStorage.CreationCollisionOption;

namespace StrixMusic.Sdk.WinUI.Models
{
    /// <inheritdoc/>
    public class FolderData : IFolderData
    {
        /// <summary>
        /// The underlying <see cref="StorageFolder"/> instance in use.
        /// </summary>
        public StorageFolder StorageFolder { get; private set; }

        /// <summary>
        /// Constructs a new instance of <see cref="IFolderData"/>.
        /// </summary>
        public FolderData(StorageFolder folder)
        {
            StorageFolder = folder;
        }

        /// <inheritdoc/>
        public string Name => StorageFolder.Name;

        /// <inheritdoc/>
        public string Path => StorageFolder.Path;

        /// <inheritdoc/>
        public string? Id { get; set; }

        /// <inheritdoc/>
        public async Task<IEnumerable<IFileData>> GetFilesAsync()
        {
            var files = await StorageFolder.GetFilesAsync();

            return files.Select(x => new FileData(x)).ToArray();
        }

        /// <inheritdoc />
        public Task DeleteAsync() => StorageFolder.DeleteAsync().AsTask();

        /// <inheritdoc/>
        public async Task<IFolderData?> GetParentAsync()
        {
            var storageFolder = await StorageFolder.GetParentAsync();

            return new FolderData(storageFolder);
        }

        /// <inheritdoc/>
        public async Task<IFolderData> CreateFolderAsync(string desiredName)
        {
            var storageFolder = await StorageFolder.CreateFolderAsync(desiredName);

            return new FolderData(storageFolder);
        }

        /// <inheritdoc/>
        public async Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            var collisionOptions = (Windows.Storage.CreationCollisionOption)Enum.Parse(typeof(Windows.Storage.CreationCollisionOption), options.ToString());

            var storageFolder = await StorageFolder.CreateFolderAsync(desiredName, collisionOptions);

            return new FolderData(storageFolder);
        }

        /// <inheritdoc/>
        public async Task<IFileData> CreateFileAsync(string desiredName)
        {
            var storageFile = await StorageFolder.CreateFileAsync(desiredName);

            return new FileData(storageFile);
        }

        /// <inheritdoc/>
        public async Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            var collisionOptions = (Windows.Storage.CreationCollisionOption)Enum.Parse(typeof(Windows.Storage.CreationCollisionOption), options.ToString());
            var storageFile = await StorageFolder.CreateFileAsync(desiredName, collisionOptions);

            return new FileData(storageFile);
        }

        /// <inheritdoc/>
        public async Task<IFolderData?> GetFolderAsync(string name)
        {
            var folderData = await StorageFolder.GetFolderAsync(name);

            return new FolderData(folderData);
        }

        /// <inheritdoc/>
        public async Task<IFileData?> GetFileAsync(string name)
        {
            var fileData = await StorageFolder.GetFileAsync(name);

            return new FileData(fileData);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            // TODO
            var foldersData = await StorageFolder.GetFoldersAsync();

            return foldersData.Select(x => new FolderData(x));
        }

        /// <inheritdoc />
        public async Task EnsureExists()
        {
            try
            {
                _ = StorageFolder.GetFolderFromPathAsync(StorageFolder.Path);
            }
            catch
            {
                StorageFolder = await StorageFolder.CreateFolderAsync(StorageFolder.Name);
            }
        }
    }
}
