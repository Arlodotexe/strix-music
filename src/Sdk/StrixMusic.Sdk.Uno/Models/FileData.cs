using System;
using System.IO;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using Windows.Storage;
using FileAccessMode = OwlCore.AbstractStorage.FileAccessMode;

namespace StrixMusic.Sdk.Uno.Models
{
    /// <inheritdoc cref="IFileData"/>
    public class FileData : IFileData
    {
        /// <summary>
        /// The underlying <see cref="StorageFile"/> instance in use.
        /// </summary>
        internal StorageFile StorageFile { get; }

        /// <summary>
        /// Creates a new instance of <see cref="FileData" />.
        /// </summary>
        /// <param name="storageFile">The <see cref="StorageFile"/> to wrap.</param>
        public FileData(StorageFile storageFile)
        {
            StorageFile = storageFile;
            Properties = new FileDataProperties(storageFile);
        }

        /// <inheritdoc/>
        public string Path => StorageFile.Path;

        /// <inheritdoc/>
        public string Name => StorageFile.Name;

        /// <inheritdoc/>
        public string DisplayName => StorageFile.DisplayName;

        /// <inheritdoc/>
        public string FileExtension => StorageFile.FileType;

        /// <inheritdoc/>
        public IFileDataProperties Properties { get; set; }

        /// <inheritdoc/>
        public async Task<IFolderData> GetParentAsync()
        {
            var storageFile = await StorageFile.GetParentAsync();

            return new FolderData(storageFile);
        }

        /// <inheritdoc/>
        public async Task Delete()
        {
            await StorageFile.DeleteAsync();
        }

        /// <inheritdoc />
        public async Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read)
        {
            var stream = await StorageFile.OpenAsync((Windows.Storage.FileAccessMode)accessMode);

            return stream.AsStream();
        }
    }

}
