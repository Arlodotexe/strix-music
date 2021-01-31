using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class TrackRepository : IMetadataRepository
    {
        private const string TRACK_DATA_FILENAME = "TrackData.bin";
        private readonly FileMetadataScanner _fileMetadataScanner;
        private string _pathToMetadataFile;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new instance for <see cref="TrackRepository"/>.
        /// </summary>
        /// <param name="fileMetadataScanner">The file scanner instance to use for getting new track data.</param>
        public TrackRepository(FileMetadataScanner fileMetadataScanner)
        {
            _fileMetadataScanner = fileMetadataScanner;
        }

        private void AttachEvents()
        {
            _fileMetadataScanner.FileMetadataAdded += FileMetadataAdded;
            _fileMetadataScanner.FileMetadataRemoved += FileMetadataRemoved;
        }

        private void DetachEvents()
        {
            _fileMetadataScanner.FileMetadataAdded -= FileMetadataAdded;
            _fileMetadataScanner.FileMetadataRemoved -= FileMetadataRemoved;
        }

        private void FileMetadataAdded(object sender, FileMetadata e)
        {
            // todo
        }

        private void FileMetadataRemoved(object sender, FileMetadata e)
        {
            // todo
        }

        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to work in.</param>
        public void SetDataFolder(IFolderData rootFolder)
        {
            _pathToMetadataFile = $"{rootFolder.Path}\\{TRACK_DATA_FILENAME}";
        }

        /// <summary>
        /// Gets all <see cref="TrackMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="limit">Get items starting at this index.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetTrackMetadata(int offset, int limit)
        {
            //if (!File.Exists(_pathToMetadatafile))
            //    throw new FileNotFoundException(_pathToMetadatafile);

            //var bytes = File.ReadAllBytes(_pathToMetadatafile);
            //var trackMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<TrackMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            // return Task.FromResult<IReadOnlyList<TrackMetadata>>(trackMetadataLst.Skip(offset).Take(limit).ToList());

            var allTracks = await _fileMetadataScanner.GetUniqueTrackMetadata();

            if (limit == -1)
            {
                return allTracks;
            }
            else
            {
                var filteredTracks = allTracks.Skip(offset).Take(limit).ToList();

                return filteredTracks;
            }
        }

        /// <summary>
        /// Gets the filtered tracks by artist ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByArtistId(string artistId, int offset, int limit)
        {
            var filteredAlbums = new List<TrackMetadata>();

            var tracks = await GetTrackMetadata(offset, limit);

            foreach (var item in tracks)
            {
                if (item.ArtistIds != null && item.ArtistIds.Contains(artistId))
                {
                    filteredAlbums.Add(item);
                }
            }

            return filteredAlbums;
        }

        /// <summary>
        /// Gets the filtered tracks by album ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string artistId, int offset, int limit)
        {
            var filteredAlbums = new List<TrackMetadata>();

            var tracks = await GetTrackMetadata(offset, -1);

            foreach (var item in tracks)
            {
                if (item?.AlbumId != null && item.AlbumId.Contains(artistId))
                {
                    filteredAlbums.Add(item);
                }
            }

            return filteredAlbums.Skip(offset).Take(limit).ToList();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            await Task.Run(ScanForTracks);
            IsInitialized = true;
        }

        /// <summary>
        /// Create or Update <see cref="TrackMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="TrackMetadata"/> collection.</returns>
        private async Task ScanForTracks()
        {
            if (!File.Exists(_pathToMetadataFile))
                File.Create(_pathToMetadataFile).Close(); // creates the file and closes the file stream.

            // NOTE: Make sure you have already scanned the file metadata. 
            var metadata = await _fileMetadataScanner.GetUniqueTrackMetadata();

            if (metadata != null && metadata.Count > 0)
            {
                var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
                File.WriteAllBytes(_pathToMetadataFile, bytes);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsInitialized)
                return;

            ReleaseUnmanagedResources();
            if (disposing)
            {
                // dispose any objects you created here
            }

            IsInitialized = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~TrackRepository()
        {
            Dispose(false);
        }
    }
}
