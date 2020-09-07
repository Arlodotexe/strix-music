using StrixMusic.Sdk.Interfaces.Storage;
using StrixMusic.UWP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        /// <inheritdoc/>
        public async Task ScanAsync()
        {
            Debug.WriteLine($"Scanning folder {Name}");

            var files = await StorageFolder.GetFilesAsync();
            TotalFileCount = files.Count;

            var filesData = files.Select(x => new FileData(x));
            _files.AddRange(filesData);

            var folders = await StorageFolder.GetFoldersAsync();
            var foldersData = folders.Select(x => new FolderData(x));
            _folders.AddRange(foldersData);
        }

        /// <inheritdoc/>
        public async Task DeepScanAsync()
        {
            Debug.WriteLine($"Deep scanning folder {Name}");

            if (!Folders.Any())
                await ScanAsync();

            foreach (var file in Files)
            {
                await file.ScanMediaDataAsync();
            }

            foreach (var folder in Folders)
            {
                await folder.DeepScanAsync();
            }

            Debug.WriteLine($"Deep scan finished for folder {Name}");
        }

        /// <inheritdoc/>
        public async Task<IFolderData> GetParentAsync()
        {
            var storageFolder = await StorageFolder.GetParentAsync();

            return new FolderData(storageFolder);
        }
    }
}
