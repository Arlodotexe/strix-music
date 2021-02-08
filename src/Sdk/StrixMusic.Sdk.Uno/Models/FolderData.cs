using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using OwlCore.AbstractStorage;
using CreationCollisionOption = OwlCore.AbstractStorage.CreationCollisionOption;
using Windows.Storage.Search;

namespace StrixMusic.Sdk.Uno.Models
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
        public async Task<IEnumerable<IFileData>> GetFilesAsync()
        {
            var files = await StorageFolder.GetFilesAsync();

            return files.Select(x => new FileData(x));
        }

        /// <inheritdoc />
        public Task DeleteAsync() => StorageFolder.DeleteAsync().AsTask();

        /// <inheritdoc/>
        public async Task<IFolderData> GetParentAsync()
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
            var storageFolder = await StorageFolder.CreateFolderAsync(desiredName, (Windows.Storage.CreationCollisionOption)options);

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
            var storageFile = await StorageFolder.CreateFileAsync(desiredName, (Windows.Storage.CreationCollisionOption)options);

            return new FileData(storageFile);
        }

        /// <inheritdoc />
        public async Task<bool> RemoveFileIfExistsAsync(string path)
        {
            try
            {
                var res = await StorageFolder.GetFileAsync(path);
                await res.DeleteAsync();

                return res != null;
            }
            catch (Exception)
            {
                return false;
            }
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

        /// <inheritdoc />
        public async Task RemoveAllFiles()
        {
            var files = await StorageFolder.GetFilesAsync();

            Parallel.ForEach(files, file =>
           {
               _ = file.DeleteAsync();
           });
        }
    }
}
