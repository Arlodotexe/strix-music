using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Extensions;
using StrixMusic.Core.LocalFiles.MetadataScanner;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class TrackService
    {
        private readonly string _trackMetadataCacheFileName = "TrackMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly IFileSystemService _fileSystemService;
        private IFolderData? _folderData;
        private IReadOnlyList<TrackMetadata>? _cachedTracks;

        /// <summary>
        /// Creates a new instance for <see cref="TrackService"/>.
        /// </summary>
        /// <param name="fileSystemService"></param>
        public TrackService(IFileSystemService fileSystemService, FileMetadataScanner fileMetadataScanner)
        {
            _fileSystemService = fileSystemService;
            _pathToMetadatafile = $"{_fileSystemService.RootFolder.Path}\\{_trackMetadataCacheFileName}";
            _fileMetadataScanner = fileMetadataScanner;
        }

        /// <summary>
        /// Gets all <see cref="TrackMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<TrackMetadata>> GetTrackMetadata(int offset, int limit)
        {
            if (_cachedTracks != null && _cachedTracks.Count != 0)
                return Task.FromResult<IReadOnlyList<TrackMetadata>>(_cachedTracks.Skip(offset).Take(limit).ToList());

            if (!File.Exists(_pathToMetadatafile))
                throw new FileNotFoundException(_pathToMetadatafile);

            var bytes = File.ReadAllBytes(_pathToMetadatafile);
            var trackMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<TrackMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<TrackMetadata>>(trackMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="TrackMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        public async Task CreateOrUpdateTrackMetadata()
        {
            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            // NOTE: Make sure you have already scanned the filemetadata. 
            var metadata = _fileMetadataScanner.GetUniqueTrackMetadataToCache();

            var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadatafile, bytes);
        }

        /// <summary>
        /// Gets the filtered tracks by artist ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByArtistId(string artistId, int offset, int limit)
        {
            var filtredAlbums = new List<TrackMetadata>();

            var tracks = await GetTrackMetadata(offset, limit);

            foreach (var item in tracks)
            {
                if (item.ArtistIds != null && item.ArtistIds.Contains(artistId))
                {
                    filtredAlbums.Add(item);
                }
            }

            return filtredAlbums;
        }


        /// <summary>
        /// Gets the filtered tracks by album ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string artistId, int offset, int limit)
        {
            var filtredAlbums = new List<TrackMetadata>();

            var tracks = await GetTrackMetadata(offset, limit);

            foreach (var item in tracks)
            {
                if (item.AlbumId != null && item.AlbumId.Contains(artistId))
                {
                    filtredAlbums.Add(item);
                }
            }

            return filtredAlbums;
        }
    }
}
