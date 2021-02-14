using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OwlCore.Provisos;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Provides safe interactions with the file system.
    /// </summary>
    public interface IFileSystemService : IAsyncInit
    {
        /// <summary>
        /// Defines the root folder where new files and folders are created.
        /// </summary>
        IFolderData RootFolder { get; }

        /// <summary>
        /// Prompts the user to select a folder to access. Upon selection, the folder is scanned and ingested.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<IFolderData?> PickFolder();

        /// <summary>
        /// Returns the folders that the user has granted access to.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/> containing paths pointing to the folders the user has granted access to.</returns>
        Task<IReadOnlyList<IFolderData?>> GetPickedFolders();

        /// <summary>
        /// Called when the user wants to revoke access to a folder.
        /// </summary>
        /// <param name="folder">The folder to be revoked.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation.</returns>
        Task RevokeAccess(IFolderData folder);

        /// <summary>
        /// Checks if a file at the specified path exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is a bool that is true if the file specified exists.</returns>
        Task<bool> FileExistsAsync(string path);

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns><see langword="true"/>if path refers to an existing directory; <see langword="false"/> if the directory does not exist or an error occurs when trying to determine if the specified directory exists.</returns>
        Task<bool> DirectoryExistsAsync(string path);

        /// <summary>
        /// Creates all the directories in a specified path.
        /// </summary>
        /// <param name="folderName">The directory to create.</param>
        /// <returns>An instance of <see cref="IFolderData"/> for the created directory.</returns>
        Task<IFolderData> CreateDirectoryAsync(string folderName);

        /// <summary>
        /// Gets the folder that has the specified absolute path in the file system.
        /// </summary>
        /// <param name="path">The absolute path in the file system (not the Uri) of the folder to get.</param>
        /// <returns>When this method completes successfully, it returns a <see cref="IFolderData"/> that represents the specified folder.</returns>
        /// <exception cref="FileNotFoundException">The specified folder does not exist. Check the value of path.</exception>
        /// <exception cref="UnauthorizedAccessException">You don't have permission to access the specified folder.</exception>
        /// <exception cref="ArgumentException">The path cannot be a relative path or a Uri. Check the value of path.</exception>
        Task<IFolderData?> GetFolderFromPathAsync(string path);

        /// <summary>
        /// Gets the file that has the specified absolute path in the file system.
        /// </summary>
        /// <param name="path">The absolute path in the file system (not the Uri) of the file to get.</param>
        /// <returns>When this method completes successfully, it returns a <see cref="IFileData"/> that represents the specified folder.</returns>
        /// <exception cref="FileNotFoundException">The specified file does not exist. Check the value of path.</exception>
        /// <exception cref="UnauthorizedAccessException">You don't have permission to access the specified file.</exception>
        /// <exception cref="ArgumentException">The path cannot be a relative path or a Uri. Check the value of path.</exception>
        Task<IFileData?> GetFileFromPathAsync(string path);
    }
}
