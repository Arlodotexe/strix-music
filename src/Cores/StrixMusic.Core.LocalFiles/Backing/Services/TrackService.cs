using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    // Note: This is in progress. This is not ready for review yet.

    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class TrackService : IAsyncInit
    {
        private readonly string _trackMetaCacheFileName = "TrackMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetafile;
        private IFolderData? _folderData;
        private IFileSystemService _fileSystemService;

        /// <summary>
        /// Creates a new instance for <see cref="TrackService"/>.
        /// </summary>
        /// <param name="fileSystemService"></param>
        public TrackService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _pathToMetafile = $"{_fileSystemService.RootFolder.Path}\\{_trackMetaCacheFileName}";
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
        /// Get all <see cref="TrackMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<TrackMetadata>> GetTrackMetadata(int offset, int limit)
        {
            if (!File.Exists(_pathToMetafile))
                throw new FileNotFoundException(_pathToMetafile);

            var bytes = File.ReadAllBytes(_pathToMetafile);
            var trackMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<TrackMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<TrackMetadata>>(trackMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="TrackMetadata"/> infromation in files.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>m
        public async Task CreateOrUpdateTrackMetadata()
        {
            if (_folderData is null)
                return;

            if (!await _fileSystemService.FileExistsAsync(_pathToMetafile))
                File.Create(_pathToMetafile).Close(); // creates the file and closes the filestream.

            var trackMetadataLst = new List<TrackMetadata>();

            var files = await _folderData.GetFilesAsync();

            foreach (var item in files)
            {
                var details = await item.Properties.GetMusicPropertiesAsync();

                if (details is null)
                    continue;

                var trackMetadata = new TrackMetadata()
                {
                    Id = Guid.NewGuid().ToString(),
                    TrackNumber = details.TrackNumber,
                    Description = details.Title,
                    Title = details.Title,
                    Genres = details.Genre.ToList(),
                };

                trackMetadataLst.Add(trackMetadata);
            }

            var bytes = MessagePackSerializer.Serialize(trackMetadataLst, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetafile, bytes);
        }
    }
}
