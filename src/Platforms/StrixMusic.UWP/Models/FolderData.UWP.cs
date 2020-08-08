using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.Storage;
using StrixMusic.UWP.Models;
using Uno.Extensions;
using Windows.Storage;

namespace StrixMusic.Models
{
    /// <inheritdoc/>
    public class FolderData : IFolderData
    {
        /// <summary>
        /// The underlying <see cref="StorageFolder"/> instance in use.
        /// </summary>
        internal StorageFolder StorageFolder { get; }

        private IList<IFileData> _files = new List<IFileData>();

        private IList<IFolderData> _folders = new List<IFolderData>();

        /// <summary>
        /// Constructs a new instance of <see cref="IFolderData"/>.
        /// </summary>
        public FolderData(StorageFolder folder)
        {
            StorageFolder = folder;
        }

        /// <inheritdoc/>
        public string Name => StorageFolder.Name;

        /// <inheritdoc/>
        public string Path => StorageFolder.Path;

        /// <inheritdoc />
        public IReadOnlyList<IFileData> Files => _files.ToArray();

        /// <inheritdoc />
        public IReadOnlyList<IFolderData> Folders => _folders.ToArray();

        /// <inheritdoc />
        public int TotalFileCount { get; private set; } = 0;

        /// <summary>
        /// Scans and populates the immediate contents of the folder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation.</returns>
        public async Task ScanFolder()
        {
            var files = await StorageFolder.GetFilesAsync();
            TotalFileCount = files.Count;

            var filesData = files.Select(x => new FileData(x));
            _files.AddRange(filesData);

            var folders = await StorageFolder.GetFoldersAsync();
            var foldersData = folders.Select(x => new FolderData(x));
            _folders.AddRange(foldersData);
        }
    }
}
