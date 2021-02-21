using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Album service for creating or getting the album metadata.
    /// </summary>
    public class AlbumRepository : IMetadataRepository
    {
        private const string ALBUM_DATA_FILENAME = "AlbumData.bin";

        private readonly IList<AlbumMetadata> _albumMetadatas;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private IFolderData? _folderData;
        private IFileSystemService? _fileSystemService;
        private string? _pathToMetadataFile;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AlbumRepository"/>.
        /// </summary>
        /// <param name="fileMetadataScanner">The file scanner instance to use when </param>
        public AlbumRepository(FileMetadataScanner fileMetadataScanner)
        {
            _fileMetadataScanner = fileMetadataScanner;
            _fileSystemService = Ioc.Default.GetService<IFileSystemService>();
            _albumMetadatas = new List<AlbumMetadata>();

            AttachEvents();
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
        /// Add a new <see cref="AlbumMetadata"/>. It helps to filter out the duplicates while adding.
        /// </summary>
        /// <param name="albumMetadata"></param>
        /// <returns>If true, track is added otherwise false.</returns>
        public bool AddOrSkipAlbumMetadata(AlbumMetadata? albumMetadata)
        {
            lock (_albumMetadatas)
            {
                if (albumMetadata == null)
                    return false;

                if (!_albumMetadatas?.Any(c =>
                    c.Title?.Equals(albumMetadata.Title ?? string.Empty, StringComparison.OrdinalIgnoreCase) ??
                    false) ?? false)
                {
                    _albumMetadatas?.Add(albumMetadata);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Sets the root folder to operate in when saving data.
        /// </summary>
        /// <param name="rootFolder">The root folder to save data in.</param>
        public void SetDataFolder(IFolderData rootFolder)
        {
            _pathToMetadataFile = $"{rootFolder.Path}\\{ALBUM_DATA_FILENAME}";
            _folderData = rootFolder;
        }

        /// <summary>
        /// Gets the <see cref="AlbumMetadata"/> by specific <see cref="AlbumMetadata"/> id. 
        /// </summary>
        /// <param name="id">The id of the corresponding <see cref="AlbumMetadata"/></param>
        /// <returns>If found return <see cref="AlbumMetadata"/> otherwise returns null.</returns>
        public async Task<AlbumMetadata?> GetAlbumMetadataById(string id)
        {
            var allAlbums = await GetAlbumMetadata(0, -1);

            if (allAlbums.Count > 0)
            {
                return allAlbums.FirstOrDefault(c => c.Id == id);
            }

            return null;
        }

        /// <summary>
        /// Gets all <see cref="TrackMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IReadOnlyList<AlbumMetadata>> GetAlbumMetadata(int offset, int limit)
        {
            Guard.IsNotNullOrWhiteSpace(_pathToMetadataFile, nameof(_pathToMetadataFile));

            var allAlbums = await _fileMetadataScanner.GetUniqueAlbumMetadata();

            if (limit == -1)
            {
                return allAlbums;
            }
            else
            {
                var filteredAlbums = allAlbums.Skip(offset).Take(limit).ToList();

                return filteredAlbums;
            }
        }

        /// <summary>
        /// Gets the filtered albums by artists ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{AlbumMetadata}"/>></returns>
        public async Task<IReadOnlyList<AlbumMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit)
        {
            var filteredAlbums = new List<AlbumMetadata>();

            var albums = await GetAlbumMetadata(offset, limit);

            foreach (var item in albums)
            {
                if (item?.ArtistIds != null && item.ArtistIds.Contains(artistId))
                {
                    filteredAlbums.Add(item);
                }
            }

            return filteredAlbums.Skip(offset).Take(limit).ToList();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            await Task.Run(ScanForAlbums);
            IsInitialized = true;
        }

        /// <summary>
        /// Create or Update <see cref="AlbumMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="AlbumMetadata"/> collection.</returns>
        private async Task ScanForAlbums()
        {
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            Guard.IsNotNull(_pathToMetadataFile, nameof(_pathToMetadataFile));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            IFileData? fileData;

            if (!await _fileSystemService.FileExistsAsync(_pathToMetadataFile))
                fileData = await _folderData.CreateFileAsync(ALBUM_DATA_FILENAME); // creates the file and closes the file stream.
            else fileData = await _folderData.GetFileAsync(ALBUM_DATA_FILENAME);

            Guard.IsNotNull(fileData, nameof(fileData));

            var metadata = await _fileMetadataScanner.GetUniqueAlbumMetadata();

            if (metadata != null && metadata.Count > 0)
            {
                var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
                await fileData.WriteAllBytesAsync(bytes);
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
        ~AlbumRepository()
        {
            Dispose(false);
        }
    }
}
