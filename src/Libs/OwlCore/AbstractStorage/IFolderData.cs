using System.Collections.Generic;
using System.Threading.Tasks;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Represents a folder that resides on a file system.
    /// </summary>
    public interface IFolderData
    {
        /// <summary>
        /// An optional, consistent, unique identifier for this file.
        /// </summary>
        public string? Id { get; }

        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The path to the folder.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The parent folder that contains this file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the parent folder.</returns>
        Task<IFolderData?> GetParentAsync();

        /// <summary>
        /// Creates a new subfolder with the specified name in the current folder.
        /// </summary>
        /// <param name="desiredName">The name of the new subfolder to create in the current folder.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is a <see cref="IFolderData"/> that represents the created folder.</returns>
        Task<IFolderData> CreateFolderAsync(string desiredName);

        /// <summary>
        /// Creates a new subfolder with the specified name in the current folder.
        /// </summary>
        /// <param name="desiredName">The name of the new subfolder to create in the current folder.</param>
        /// <param name="options">One of the enumeration values that determines how to handle the collision if a subfolder with the specified desiredName already exists in the current folder.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is a <see cref="IFolderData"/> that represents the created folder.</returns>
        Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options);

        /// <summary>
        /// Creates a new file with the specified name in the current folder.
        /// </summary>
        /// <param name="desiredName">The name of the new file to create in the current folder.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is a <see cref="IFileData"/> that represents the created file.</returns>
        Task<IFileData> CreateFileAsync(string desiredName);

        /// <summary>
        /// Creates a new file with the specified name in the current folder.
        /// </summary>
        /// <param name="desiredName">The name of the new file to create in the current folder.</param>
        /// <param name="options">One of the enumeration values that determines how to handle the collision if a file with the specified desiredName already exists in the current folder.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is a <see cref="IFileData"/> that represents the created file.</returns>
        Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options);

        /// <summary>
        /// Gets an existing folder by name.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If the file was found, an <see cref="IFolderData" /> for the folder is returned, otherwise <see langword="null" />.</returns>
        Task<IFolderData?> GetFolderAsync(string name);

        /// <summary>
        /// Gets an existing file by name.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If the file was found, an <see cref="IFileData" /> for the folder is returned, otherwise <see langword="null" />.</returns>
        Task<IFileData?> GetFileAsync(string name);

        /// <summary>
        /// Gets the folders in this directory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the folders in this directory.</returns>
        public Task<IEnumerable<IFolderData>> GetFoldersAsync();

        /// <summary>
        /// Gets the files in this directory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the folders in this directory.</returns>
        public Task<IEnumerable<IFileData>> GetFilesAsync();

        /// <summary>
        /// Permanently deletes the folder and all contents
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task DeleteAsync();

        /// <summary>
        /// Calls the underlying filesystem to ensure that the folder exists.
        /// </summary>
        /// <remarks>This was created because the local cache folder does not exist by default on WebAssembly.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task EnsureExists();
    }
}
