using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    //TODO: This repo is not complete.

    /// <summary>
    /// The service that helps in interacting with the saved file core track information.
    /// </summary>
    public class PlaylistRepository
    {
        private const string PLAYLIST_DATA_FILENAME = "PlaylistData.bin";

        private readonly List<PlaylistMetadata> _allPlaylistMetadata = new List<PlaylistMetadata>();
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private IFolderData? _folderData;
        private IFileSystemService? _fileSystemService;
        private string? _pathToMetadataFile;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new instance for <see cref="PlaylistRepository"/>.
        /// </summary>
        public PlaylistRepository()
        {
            _playlistMetadataScanner = new PlaylistMetadataScanner();

            _fileSystemService = Ioc.Default.GetService<IFileSystemService>();
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
            _folderData = rootFolder;
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
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            Guard.IsNotNull(_pathToMetadataFile, nameof(_pathToMetadataFile));
            Guard.IsNotNull(_folderData, nameof(_folderData));
            
            var fileData = await _folderData.CreateFileAsync(PLAYLIST_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
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
