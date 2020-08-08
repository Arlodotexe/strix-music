using System;
using System.Collections.Generic;

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
    }
}
