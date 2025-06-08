using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

/// <summary>
/// Handles discovery of files in a given folder.
/// </summary>
public interface IFolderScanner : IDisposable
{
    /// <summary>
    /// The root folder to scan for files.
    /// </summary>
    IFolder RootFolder { get; }

    /// <summary>
    /// Scans a folder and all subfolders for files.
    /// </summary>
    /// <returns>All discovered files from the given folder and its subfolders.</returns>
    IAsyncEnumerable<IChildFile> ScanFolderAsync(CancellationToken cancellationToken = default);
        
    /// <summary>
    /// A collection of known files.
    /// </summary>
    ObservableCollection<IChildFile> KnownFiles { get; }

    /// <summary>
    /// Gets a known file by its ID.
    /// </summary>
    /// <param name="fileId">The ID of the file to retrieve.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    /// <returns>The found file.</returns>
    /// <exception cref="System.IO.FileNotFoundException">Thrown if the given fileId is not found.</exception>
    Task<IChildFile> GetKnownFileByIdAsync(string fileId, CancellationToken cancellationToken = default);
}
