using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Handles discovery of files in a given folder.
    /// </summary>
    public interface IFileScanner : IDisposable
    {
        /// <summary>
        /// The root folder to scan for files.
        /// </summary>
        IFolderData RootFolder { get; }

        /// <summary>
        /// Scans a folder and all subfolders for files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Contains all discovered files from the given folder and its subfolders.</returns>
        Task<IEnumerable<IFileData>> ScanFolder(IFolderData rootFolder);

        /// <summary>
        /// Scans a folder and all subfolders for files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Contains all discovered files from the given folder and its subfolders.</returns>
        Task<IEnumerable<IFileData>> ScanFolder(IFolderData rootFolder, CancellationToken cancellationToken);

        /// <summary>
        /// Raised when all files are found.
        /// </summary>
        event EventHandler<IEnumerable<IFileData>>? FileDiscoveryCompleted;

        /// <summary>
        /// Raised when a file is discovered.
        /// </summary>
        event EventHandler<IEnumerable<IFileData>>? FilesDiscovered;

        /// <summary>
        /// Raised when a folder is discovered.
        /// </summary>
        event EventHandler<IEnumerable<IFolderData>>? FoldersDiscovered;

        /// <summary>
        /// Raised file discovery starts
        /// </summary>
        event EventHandler? FileDiscoveryStarted;
    }
}