using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.Storage;

namespace StrixMusic.Services.StorageService
{
    /// <summary>
    /// Provides safe interactions with the file system.
    /// </summary>
    public interface IFileSystemService
    {
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
        /// <returns>A <see cref="Task{T}"/> representing the asyncronous operation.</returns>
        Task RevokeAccess(IFolderData folder);
    }
}
