using MessagePack;
using OwlCore.AbstractStorage;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.MetadataScanner;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    /// <summary>
    /// Service to retrieve artist records.
    /// </summary>
    public class ArtistService
    {
        private readonly string _ArtistMetadataCacheFileName = "ArtistMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly IFileSystemService _fileSystemService;

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
        /// Gets all <see cref="TrackMetadata"/> over the file system.
        /// </summary>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="limit">Get items starting at this index.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<ArtistMetadata?>> GetArtistMetadata(int offset, int limit)
        {
            //if (!File.Exists(_pathToMetadatafile))
            //    throw new FileNotFoundException(_pathToMetadatafile);

            //var bytes = File.ReadAllBytes(_pathToMetadatafile);
            //var artistMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<ArtistMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            //return Task.FromResult<IReadOnlyList<ArtistMetadata>>(artistMetadataLst.Skip(offset).Take(limit).ToList());

            var allArtists = _fileMetadataScanner.GetUniqueArtistMetadata();

            if (limit == -1)
            {
                return Task.FromResult(allArtists);
            }
            else
            {
                var filteredArtists = allArtists.Skip(offset).Take(limit).ToList();

                return Task.FromResult<IReadOnlyList<ArtistMetadata>>(filteredArtists);
            }
        }

        /// <summary>
        /// Create or Update <see cref="ArtistMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="ArtistMetadata"/> collection.</returns>
        public async Task CreateOrUpdateArtistMetadata()
        {
            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            // NOTE: Make sure you have already scanned the file metadata.
            var metadata = _fileMetadataScanner.GetUniqueArtistMetadata();

            var bytes = MessagePackSerializer.Serialize(metadata, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            File.WriteAllBytes(_pathToMetadatafile, bytes);
        }

        /// <summary>
        /// Gets the filtered artist by album ids.
        /// </summary>
        /// <param name="albumId">The artist Id.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByAlbumId(string albumId, int offset, int limit)
        {
            var filteredArtists = new List<ArtistMetadata>();

            var artists = await GetArtistMetadata(offset, limit);

            foreach (var item in artists)
            {
                if (item.AlbumIds != null && item.AlbumIds.Contains(albumId))
                {
                    filteredArtists.Add(item);
                }
            }

            return filteredArtists;
        }

        /// <summary>
        /// Gets the filtered artist by album ids.
        /// </summary>
        /// <param name="trackId">The artist Id.</param>
        /// <param name="offset">The starting index for retrieving items.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <returns>The filtered <see cref="IReadOnlyList{ArtistMetadata}"/>></returns>
        public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByTrackId(string trackId, int offset, int limit)
        {
            var filteredArtists = new List<ArtistMetadata>();

            var artists = await GetArtistMetadata(0, -1);

            foreach (var item in artists)
            {
                if (item?.AlbumIds != null && item.AlbumIds.Contains(trackId))
                {
                    filteredArtists.Add(item);
                }
            }

            return filteredArtists.Skip(offset).Take(limit).ToList();
        }
    }
}
