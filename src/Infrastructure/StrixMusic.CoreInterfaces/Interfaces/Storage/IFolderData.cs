using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces.Storage
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
        /// Scans and populates the immediate contents of the folder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation.</returns>
        public Task ScanAsync();

        /// <summary>
        /// Scans and populates the contents of the folder and all subfolders.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation.</returns>
        public Task DeepScanAsync();
    }
}
