using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
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
    /// Service to retreive artist records.
    /// </summary>
    public class ArtistService
    {
        private readonly string _ArtistMetadataCacheFileName = "TrackMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly IFileSystemService _fileSystemService;
        private IFolderData? _folderData;
        private IReadOnlyList<ArtistMetadata>? _cachedArtists;

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
        /// Gets all <see cref="ArtistMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ArtistMetadata>> GetArtistMetadata(int offset, int limit)
        {
            if (_cachedArtists != null && _cachedArtists.Count != 0)
                return Task.FromResult<IReadOnlyList<ArtistMetadata>>(_cachedArtists.Skip(offset).Take(limit).ToList());

            if (!File.Exists(_pathToMetadatafile))
                throw new FileNotFoundException(_pathToMetadatafile);

            var bytes = File.ReadAllBytes(_pathToMetadatafile);
            var artistMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<ArtistMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<ArtistMetadata>>(artistMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="ArtistMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="ArtistMetadata"/> collection.</returns>
        public async Task CreateOrUpdateArtistMetadata()
        {
            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            // NOTE: Make sure you have already scanned the filemetadata.
            var metadata = _fileMetadataScanner.GetUniqueArtistMetadataToCache();

            var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadatafile, bytes);
        }

        /// <summary>
        /// Gets the filtered artist by album ids.
        /// </summary>
        /// <param name="artistId">The artist Id.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<ArtistMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit)
        {
            var filtredAlbums = new List<ArtistMetadata>();

            var artists = await GetArtistMetadata(offset, limit);

            foreach (var item in artists)
            {
                if (item.AlbumIds != null && item.AlbumIds.Contains(artistId))
                {
                    filtredAlbums.Add(item);
                }
            }

            return filtredAlbums;
        }
    }
}
