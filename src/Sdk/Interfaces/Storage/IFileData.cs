using System;
using System.IO;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces.Storage
{
    /// <summary>
    /// Holds information about a file
    /// </summary>
    public interface IFileData
    {
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

        /// <inheritdoc cref="IMusicFileProperties"/>
        public IMusicFileProperties MusicProperties { get; set; }

        /// <summary>
        /// The parent folder that contains this file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the parent folder.</returns>
        public Task<IFolderData> GetParentAsync();

        /// <summary>
        /// Opens and returns a stream to the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is a stream to the file.</returns>
        public Task<Stream> GetStreamAsync();

        /// <summary>
        /// Populates the <see cref="MusicProperties"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ScanMediaDataAsync();

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Delete();
    }
}
