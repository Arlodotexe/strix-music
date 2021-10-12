using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.AbstractStorage.Scanners
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
        Task<IEnumerable<IFileData>> ScanFolderAsync();

        /// <summary>
        /// Scans a folder and all subfolders for files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Contains all discovered files from the given folder and its subfolders.</returns>
        Task<IEnumerable<IFileData>> ScanFolderAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Raised when files are discovered.
        /// </summary>
        event EventHandler<IEnumerable<IFileData>>? FilesDiscovered;

        /// <summary>
        /// Raised when folders are discovered.
        /// </summary>
        event EventHandler<IEnumerable<IFolderData>>? FoldersDiscovered;

        /// <summary>
        /// Raised file discovery starts
        /// </summary>
        event EventHandler? FileDiscoveryStarted;

        /// <summary>
        /// Raised when all files are found.
        /// </summary>
        event EventHandler<IEnumerable<IFileData>>? FileDiscoveryCompleted;
    }
}