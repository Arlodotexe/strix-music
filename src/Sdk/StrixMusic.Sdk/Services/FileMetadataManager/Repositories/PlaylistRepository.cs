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
        private const string PLAYLIST_DATA_FILENAME = "Playlists.bin";
        private readonly IList<PlaylistMetadata> _playListMetadatas;
        private readonly IFileSystemService? _fileSystemService;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private string? _pathToMetadataFile;
        private IFolderData? _folderData;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new instance for <see cref="TrackRepository"/>.
        /// </summary>
        ///  <param name="fileMetadataScanner">The file scanner instance to use when </param>
        public PlaylistRepository(FileMetadataScanner fileMetadataScanner)
        {
            _fileMetadataScanner = fileMetadataScanner;
            _fileSystemService = Ioc.Default.GetService<IFileSystemService>();

            _playListMetadatas = new List<PlaylistMetadata>();
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
        /// Add a new <see cref="PlaylistMetadata"/>. It helps to filter out the duplicates while adding.
        /// </summary>
        /// <param name="playlistMetadata">The metadata to be added.</param>
        /// <returns>If true, track is added otherwise false.</returns>
        public bool AddOrSkipPlayListsMetadata(PlaylistMetadata? playlistMetadata)
        {
            lock (_playListMetadatas)
            {
                if (playlistMetadata == null)
                    return false;

                if (!_playListMetadatas?.Any(c =>
                    c.Id?.Equals(playlistMetadata.Id ?? string.Empty, StringComparison.OrdinalIgnoreCase) ??
                    false) ?? false)
                {
                    _playListMetadatas?.Add(playlistMetadata);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to work in.</param>
        public void SetDataFolder(IFolderData rootFolder)
        {
            _pathToMetadataFile = $"{rootFolder.Path}\\{PLAYLIST_DATA_FILENAME}";
            _folderData = rootFolder;
        }

        /// <summary>
        /// Gets the <see cref="PlaylistMetadata"/> by specific <see cref="PlaylistMetadata"/> id. 
        /// </summary>
        /// <param name="id">The id of the corresponding <see cref="PlaylistMetadata"/></param>
        /// <returns>If found return <see cref="PlaylistMetadata"/> otherwise returns null.</returns>
        public async Task<PlaylistMetadata?> GetPlayListsMetadataById(string id)
        {
            var allPlayLists = await GetPlaylistsMetadata(0, -1);

            if (allPlayLists.Count > 0)
            {
                return allPlayLists.FirstOrDefault(c => c.Id == id);
            }

            return null;
        }

        /// <summary>
        /// Gets all <see cref="TrackMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IReadOnlyList<PlaylistMetadata>> GetPlaylistsMetadata(int offset, int limit)
        {
            var allPlaylists = await _fileMetadataScanner.GetUniquePlaylistMetadata();

            if (limit == -1)
            {
                return allPlaylists;
            }
            else
            {
                var filteredPlaylists = allPlaylists.Skip(offset).Take(limit).ToList();

                return filteredPlaylists;
            }
        }

        /// <summary>
        /// Gets the playlists by track Id.
        /// </summary>
        /// <param name="playListId">The playlist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{PlaylistMetadata}"/>></returns>
        public async Task<IReadOnlyList<PlaylistMetadata>> GetPlaylistsByTrackId(string playListId, int offset, int limit)
        {
            var filteredPlaylists = new List<PlaylistMetadata>();

            var playlists = await GetPlaylistsMetadata(0, -1);

            foreach (var item in playlists)
            {
                if (item.TrackIds?.Contains(playListId) ?? false)
                {
                    filteredPlaylists.Add(item);
                }
            }

            return filteredPlaylists.Skip(offset).Take(limit).ToList();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            await Task.Run(ScanForPlaylists);
            IsInitialized = true;
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
