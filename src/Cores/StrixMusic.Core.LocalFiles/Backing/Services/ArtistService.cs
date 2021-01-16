using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
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
    /// Service to retreive artist records.
    /// </summary>
    public class ArtistService
    {
        private readonly string _ArtistMetadataCacheFileName = "TrackMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly IFileSystemService _fileSystemService;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance for <see cref="TrackService"/>.
        /// </summary>
        /// <param name="fileSystemService"></param>
        public ArtistService(IFileSystemService fileSystemService, FileMetadataScanner fileMetadataScanner)
        {
            _fileSystemService = fileSystemService;
            _pathToMetadatafile = $"{_fileSystemService.RootFolder.Path}\\{_ArtistMetadataCacheFileName}";
            _fileMetadataScanner = fileMetadataScanner;
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
        /// Gets all <see cref="ArtistMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ArtistService>> GetArtistMetadata(int offset, int limit)
        {
            if (!File.Exists(_pathToMetadatafile))
                throw new FileNotFoundException(_pathToMetadatafile);

            var bytes = File.ReadAllBytes(_pathToMetadatafile);
            var ArtistMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<ArtistService>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<ArtistService>>(ArtistMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="ArtistMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="ArtistMetadata"/> collection.</returns>
        public async Task CreateOrUpdateArtistMetadata()
        {
            if (_folderData is null)
                return;

            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            // NOTE: Make sure you have already scanned the filemetadata.
            var metadata = _fileMetadataScanner.GetUniqueArtistMetadataToCache();

            var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadatafile, bytes);
        }
    }
}
