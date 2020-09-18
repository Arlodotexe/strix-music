using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Interfaces.Storage;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <summary>
    /// Provides safe interactions with the file system.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Defines the root folder where new files and folders are created.
        /// </summary>
        IFolderData RootFolder { get; }

        /// <summary>
        /// Initializes the service, performing first time setup tasks.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Init();

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
    }
}
