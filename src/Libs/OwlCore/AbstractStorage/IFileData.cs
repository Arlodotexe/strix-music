using System.IO;
using System.Threading.Tasks;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Represents a file that resides on a file system.
    /// </summary>
    public interface IFileData
    {
        /// <summary>
        /// An optional, consistent, unique identifier for this file.
        /// </summary>
        public string? Id { get; }

        /// <summary>
        /// The path to the file.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The name of the file, without the extension.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The user-friendly name for this file.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The file extension.
        /// </summary>
        public string FileExtension { get; }

        /// <inheritdoc cref="IFileDataProperties"/>
        public IFileDataProperties Properties { get; set; }

        /// <summary>
        /// The parent folder that contains this file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the parent folder.</returns>
        public Task<IFolderData> GetParentAsync();

        /// <summary>
        /// Opens and returns a stream to the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is a stream to the file.</returns>
        public Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Delete();

        /// <summary>
        /// Writes all text on the <see cref="IFileData"/>.
        /// </summary>
        /// <returns></returns>
        public Task WriteAllBytesAsync(byte[] bytes);

        /// <summary>
        /// Retrieves an adjusted thumbnail image for the file, determined by the purpose of the thumbnail, the requested size, and the specified options.
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetThumbnailAsync(ThumbnailMode thumbnailMode, uint requiredSize);
    }
}
