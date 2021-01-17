using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
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
    /// Album service for creating or getting the album metadata.
    /// </summary>
    public class AlbumService
    {
        private readonly string _albumMetadataCacheFileName = "AlbumMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly IFileSystemService _fileSystemService;
        private FileMetadataScanner _fileMetadataScanner;
        private IReadOnlyList<AlbumMetadata>? _cachedAlbums;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumService"/>.
        /// </summary>
        ///  /// <param name="fileSystemService">The service to access the file system.</param>
        public AlbumService(IFileSystemService fileSystemService, FileMetadataScanner fileMetadataScanner)
        {
            _fileSystemService = fileSystemService;
            _pathToMetadatafile = $"{_fileSystemService.RootFolder.Path}\\{_albumMetadataCacheFileName}";
            _fileMetadataScanner = fileMetadataScanner;
        }

        /// <summary>
        /// Gets all <see cref="AlbumMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<AlbumMetadata>> GetAlbumMetadata(int offset, int limit)
        {
            if (_cachedAlbums != null && _cachedAlbums.Count != 0)
                return Task.FromResult<IReadOnlyList<AlbumMetadata>>(_cachedAlbums.Skip(offset).Take(limit).ToList());

            if (!File.Exists(_pathToMetadatafile))
                throw new FileNotFoundException(_pathToMetadatafile);

            var bytes = File.ReadAllBytes(_pathToMetadatafile);
            var albumMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<AlbumMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            _cachedAlbums = albumMetadataLst;
            return Task.FromResult<IReadOnlyList<AlbumMetadata>>(albumMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="AlbumMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="AlbumMetadata"/> collection.</returns>
        public async Task CreateOrUpdateAlbumMetadata()
        {
            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            // NOTE: Make sure you have already scanned the filemetadata. 
            var metadata = _fileMetadataScanner.GetUniqueAlbumMetadataToCache();

            var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadatafile, bytes);
        }

        /// <summary>
        /// Gets the filtered albums by artists ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{AlbumMetadata}"/>></returns>
        public async Task<IReadOnlyList<AlbumMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit)
        {
            var filtredAlbums = new List<AlbumMetadata>();

            var albums = await GetAlbumMetadata(offset, limit);

            foreach (var item in albums)
            {
                if (item.ArtistIds != null && item.ArtistIds.Contains(artistId))
                {
                    filtredAlbums.Add(item);
                }
            }

            return filtredAlbums;
        }
    }
}
