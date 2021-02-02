using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class PlaylistRepository : IMetadataRepository
    {
        private const string PLAYLIST_DATA_FILENAME = "PlaylistData.bin";

        private readonly List<PlaylistMetadata> _allPlaylistMetadata = new List<PlaylistMetadata>();
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private string? _pathToMetadataFile;
        private IFolderData? _rootFolder;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new instance for <see cref="PlaylistRepository"/>.
        /// </summary>
        public PlaylistRepository()
        {
            _playlistMetadataScanner = new PlaylistMetadataScanner();
        }

        private void AttachEvents()
        {
            
        }

        private void DetachEvents()
        {

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
            _rootFolder = rootFolder;
            _pathToMetadataFile = $"{rootFolder.Path}\\{PLAYLIST_DATA_FILENAME}";
        }

        /// <summary>
        /// Get all <see cref="PlaylistMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<PlaylistMetadata>> GetPlaylists(int offset, int limit)
        {
            if (!File.Exists(_pathToMetadataFile))
                throw new FileNotFoundException(_pathToMetadataFile);

            var bytes = File.ReadAllBytes(_pathToMetadataFile);
            var playlistMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<PlaylistMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(playlistMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="PlaylistMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="PlaylistMetadata"/> collection.</returns>
        private async Task ScanForPlaylists()
        {
            Guard.IsNotNull(_rootFolder, nameof(_rootFolder));

            if (!File.Exists(_pathToMetadataFile))
                File.Create(_pathToMetadataFile).Close(); // creates the file and closes the file stream.

            _allPlaylistMetadata.Clear();

            var files = await _rootFolder.RecursiveDepthFileSearchAsync();

            foreach (var item in files)
            {
                var metadata = await _playlistMetadataScanner.ScanPlaylistMetadata(item);
                if (metadata is null)
                    continue;

                _allPlaylistMetadata.Add(metadata);
            }

            var bytes = MessagePackSerializer.Serialize(_allPlaylistMetadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadataFile, bytes);
        }

        /// <summary>
        /// Initializes the repo.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitAsync()
        {
            await Task.Run(ScanForPlaylists);
            IsInitialized = true;
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
        ~PlaylistRepository()
        {
            Dispose(false);
        }
    }
}
