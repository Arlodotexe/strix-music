using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Extensions;
using StrixMusic.Core.LocalFiles.MetadataScanner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class PlaylistService
    {
        private readonly string _playlistMetadataCacheFileName = "PlaylistMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly IFileSystemService _fileSystemService;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance for <see cref="PlaylistService"/>.
        /// </summary>
        /// <param name="fileSystemService"></param>
        public PlaylistService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _pathToMetadatafile = $"{_fileSystemService.RootFolder.Path}\\{_playlistMetadataCacheFileName}";
            _playlistMetadataScanner = new PlaylistMetadataScanner();
        }

        /// <summary>
        /// Get all <see cref="PlaylistMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<PlaylistMetadata>> GetPlaylistMetadata(int offset, int limit)
        {
            if (!File.Exists(_pathToMetadatafile))
                throw new FileNotFoundException(_pathToMetadatafile);

            var bytes = File.ReadAllBytes(_pathToMetadatafile);
            var playlistMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<PlaylistMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(playlistMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitAsync()
        {
            var folders = await _fileSystemService.GetPickedFolders();
            Guard.IsNotNull(folders, nameof(folders));
            _folderData = folders.ToList().FirstOrDefault();
            Guard.IsNotNull(_folderData, nameof(_folderData));
        }


        /// <summary>
        /// Create or Update <see cref="PlaylistMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="PlaylistMetadata"/> collection.</returns>
        public async Task CreateOrUpdatePlaylistMetadata()
        {
            if (_folderData is null)
                return;

            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            var playlistMetadataLst = new List<PlaylistMetadata>();

            var files = await _folderData.RecursiveDepthFileSearchAsync();

            foreach (var item in files)
            {
                var metadata = await _playlistMetadataScanner.ScanPlaylistMetadata(item);
                if (metadata is null)
                    continue;

                playlistMetadataLst.Add(metadata);
            }

            var bytes = MessagePackSerializer.Serialize(playlistMetadataLst, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadatafile, bytes);
        }
    }
}
