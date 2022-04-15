using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractStorage;
using Windows.Storage;
using Windows.Storage.FileProperties;
using FileAccessMode = OwlCore.AbstractStorage.FileAccessMode;

namespace StrixMusic.Sdk.WinUI.Models
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
        public string Id => Path;

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

            Guard.IsNotNull(storageFile, nameof(storageFile));

            return new FolderData(storageFile);
        }

        /// <inheritdoc/>
        public Task Delete()
        {
            return StorageFile.DeleteAsync().AsTask();
        }

        /// <inheritdoc />
        public async Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read)
        {
            var stream = await StorageFile.OpenAsync((Windows.Storage.FileAccessMode)accessMode);

            return stream.AsStream();
        }

        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes)
        {
            return FileIO.WriteBytesAsync(StorageFile, bytes).AsTask();
        }

        /// <inheritdoc />
        public async Task<Stream> GetThumbnailAsync(OwlCore.AbstractStorage.ThumbnailMode thumbnailMode, uint requiredSize)
        {
            var thumbnail = await StorageFile.GetThumbnailAsync((Windows.Storage.FileProperties.ThumbnailMode)thumbnailMode, requiredSize);

            return thumbnail.AsStream();
        }
    }

}
