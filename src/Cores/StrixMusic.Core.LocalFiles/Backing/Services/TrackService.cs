using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Core.LocalFileCore.Backing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    // Note: This is not ready for review yet.

    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class TrackService
    {
        private IFolderData? _folderData;
        private IFileSystemService _fileSystemService;

        /// <summary>
        /// Creates a new instance for <see cref="TrackService"/>.
        /// </summary>
        /// <param name="fileSystemService"></param>
        public TrackService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
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
        public async Task<IReadOnlyList<TrackMetadata>> GetTrackMetaData(int offset, int limit)
        {
            var lst = new List<TrackMetadata>();

            var files = await _folderData.GetFilesAsync();

            foreach (var item in files)
            {
                var details = await item.Properties.GetMusicPropertiesAsync();

                var trackMetaData = new TrackMetadata()
                {
                    Id = Guid.NewGuid().ToString(),
                    TrackNumber = Convert.ToInt32(details.TrackNumber),
                    Description = details.Title,
                    Name = details.Title,
                    Genres = details.Genre.ToList(),
                };

                lst.Add(trackMetaData);
            }

            return lst;
        }
    }
}
