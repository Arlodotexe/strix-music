using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces.Storage
{
    /// <summary>
    /// Holds information about a folder
    /// </summary>
    public interface IFolderData
    {
        /// <summary>
        /// The files contained in this folder.
        /// </summary>
        public IReadOnlyList<IFileData> Files { get; }

        /// <summary>
        /// The folders contained in this folder.
        /// </summary>
        public IReadOnlyList<IFolderData> Folders { get; }

        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The path to the folder.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The total number of files in this folder and all subfolders.
        /// </summary>
        public int TotalFileCount { get; }

        /// <summary>
        /// The parent folder that contains this file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the parent folder.</returns>
        Task<IFolderData> GetParentAsync();

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
        /// Scans and populates the immediate contents of the folder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ScanAsync();

        /// <summary>
        /// Scans and populates the contents of the folder and all subfolders.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task DeepScanAsync();

        /// <summary>
        /// Deletes the folder and all contents
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Delete();

        /// <summary>
        /// Calls the underlying filesystem to ensure that the folder exists.
        /// </summary>
        /// <remarks>This was created because the local cache folder does not exist by default on WebAssembly.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task EnsureExists();
    }
}
