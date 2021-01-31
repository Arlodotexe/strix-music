using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TagLib;
using File = TagLib.File;

namespace StrixMusic.Core.LocalFiles.MetadataScanner
{
    /// <summary>
    /// Handles scanning tracks for all supported metadata.
    /// </summary>
    public class FileMetadataScanner
    {
        private IReadOnlyList<FileMetadata>? __fileMetadata;

        /// <summary>
        /// It is raised whenever a new related metadata is added during scan.
        /// </summary>
        public event EventHandler<FileMetadata>? FileMetadataChanged;

        /// <summary>
        /// Scans a folder and all subfolders for music and music metadata.
        /// </summary>
        /// <param name="folderData">The path to a root folder to scan.</param>
        /// <returns>Fully scanned <see cref="IReadOnlyList{RelatedMetadata}"/>.</returns>
        public async Task ScanFolderForMetadata(IFolderData folderData)
        {
            var files = await folderData.RecursiveDepthFileSearchAsync();
            var relatedMetaDataList = new List<FileMetadata>();

            foreach (var item in files)
            {
                var fileMetadata = await ScanFileMetadata(item);

                if (fileMetadata == null)
                    continue;

                relatedMetaDataList.Add(fileMetadata);
                ApplyRelatedMetadataIds(relatedMetaDataList);

                FileMetadataChanged?.Invoke(this, fileMetadata);
            }
        }

        private async Task<FileMetadata?> ScanFileMetadata(IFileData fileData)
        {
            var id3Metadata = await GetID3Metadata(fileData);

            // disabled for now, scanning non-songs returns valid data
            // var propertyMetadata = await GetMusicFilesProperties(fileData);
            var foundMetadata = new[] { id3Metadata };

            var validMetadatas = foundMetadata.PruneNull().ToArray();

            if (validMetadatas.Length == 0)
                return null;

            var mergedTrackMetada = MergeTrackMetadata(validMetadatas);
            mergedTrackMetada.Id = Guid.NewGuid().ToString();

            return mergedTrackMetada;
        }

        private FileMetadata MergeTrackMetadata(FileMetadata[] metadata)
        {
            if (metadata.Length == 1)
                return metadata[0];

            var mergedMetaData = metadata[0];
            for (var i = 1; i < metadata.Length; i++)
            {
                var item = metadata[i];

                if (mergedMetaData.TrackMetadata != null && item.TrackMetadata != null)
                {
                    mergedMetaData.TrackMetadata.TrackNumber ??= item.TrackMetadata.TrackNumber;
                    mergedMetaData.TrackMetadata.Genres ??= item.TrackMetadata.Genres;
                    mergedMetaData.TrackMetadata.DiscNumber ??= item.TrackMetadata.DiscNumber;
                    mergedMetaData.TrackMetadata.Duration ??= item.TrackMetadata.Duration;
                    mergedMetaData.TrackMetadata.Lyrics ??= item.TrackMetadata.Lyrics;
                    mergedMetaData.TrackMetadata.Language ??= item.TrackMetadata.Language;
                    mergedMetaData.TrackMetadata.Description ??= item.TrackMetadata.Description;
                    mergedMetaData.TrackMetadata.Title ??= item.TrackMetadata.Title;
                    mergedMetaData.TrackMetadata.Url ??= item.TrackMetadata.Url;
                    mergedMetaData.TrackMetadata.Year ??= item.TrackMetadata.Year;
                }

                if (mergedMetaData.AlbumMetadata != null && item.AlbumMetadata != null)
                {
                    mergedMetaData.AlbumMetadata.DatePublished ??= item.AlbumMetadata.DatePublished;
                    mergedMetaData.AlbumMetadata.Genres ??= item.AlbumMetadata.Genres;
                    mergedMetaData.AlbumMetadata.Duration ??= item.AlbumMetadata.Duration;
                    mergedMetaData.AlbumMetadata.Description ??= item.AlbumMetadata.Description;
                    mergedMetaData.AlbumMetadata.Title ??= item.AlbumMetadata.Title;
                }

                if (mergedMetaData.ArtistMetadata != null && item.ArtistMetadata != null)
                {
                    mergedMetaData.ArtistMetadata.Name ??= item.ArtistMetadata.Name;
                    mergedMetaData.ArtistMetadata.Url ??= item.ArtistMetadata.Url;
                }
            }

            return mergedMetaData;
        }

        private async Task<FileMetadata?> GetMusicFilesProperties(IFileData fileData)
        {
            var details = await fileData.Properties.GetMusicPropertiesAsync();

            if (details is null)
                return null;

            var relatedMetadata = new FileMetadata();
            relatedMetadata.AlbumMetadata = new AlbumMetadata()
            {
                Title = details.Album,
                Duration = details.Duration,
                Genres = new List<string>(details.Genre),
            };

            relatedMetadata.TrackMetadata = new TrackMetadata()
            {
                TrackNumber = details.TrackNumber,
                Title = details.Title,
                Genres = details.Genre?.ToList(),
                Duration = details.Duration,
                Source = new Uri(fileData.Path),
                Year = details.Year,
            };

            relatedMetadata.ArtistMetadata = new ArtistMetadata()
            {
                Name = details.Artist,
            };

            return relatedMetadata;
        }

        private async Task<FileMetadata?> GetID3Metadata(IFileData fileData)
        {
            try
            {
                var stream = await fileData.GetStreamAsync();

                using var tagFile = File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);

                // Read the raw tags
                var tags = tagFile.GetTag(TagTypes.Id3v2);

                // If there's no metadata to read, return null
                if (tags == null)
                    return null;

                return new FileMetadata
                {
                    AlbumMetadata = new AlbumMetadata()
                    {
                        Description = tags.Description,
                        Title = tags.Album,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        DatePublished = tags.DateTagged,
                        TotalTracksCount = Convert.ToInt32(tags.TrackCount),
                        TotalArtistsCount = tags.AlbumArtists.Length,
                    },

                    TrackMetadata = new TrackMetadata()
                    {
                        Source = new Uri(fileData.Path),
                        Description = tags.Description,
                        Title = tags.Title,
                        DiscNumber = tags.Disc,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        TrackNumber = tags.Track,
                        Year = tags.Year,
                    },
                    ArtistMetadata = new ArtistMetadata()
                    {
                        Name = tags.FirstAlbumArtist,
                    },
                };
            }
            catch (CorruptFileException)
            {
                return null;
            }
            catch (UnsupportedFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Create groups and apply ids to the <see cref="List{RelatedMetadata}"/>
        /// </summary>
        /// <param name="relatedMetadata"></param>
        public void ApplyRelatedMetadataIds(List<FileMetadata> relatedMetadata)
        {
            var artistGroup = relatedMetadata.GroupBy(c => c.ArtistMetadata?.Name);
            var albumGroup = relatedMetadata.GroupBy(c => c.AlbumMetadata?.Title);
            var trackGroup = relatedMetadata.GroupBy(c => c.TrackMetadata?.Title);

            foreach (var tracks in albumGroup)
            {
                var albumId = Guid.NewGuid().ToString();
                foreach (var item in tracks)
                {
                    if (item.AlbumMetadata != null && item.TrackMetadata != null && item.ArtistMetadata != null)
                    {
                        item.AlbumMetadata.Id = albumId;
                        item.TrackMetadata.AlbumId = albumId;
                        item.ArtistMetadata.AlbumIds = new List<string> { albumId };
                    }
                }
            }

            foreach (var tracks in artistGroup)
            {
                var artistId = Guid.NewGuid().ToString();
                foreach (var item in tracks)
                {
                    if (item.AlbumMetadata != null && item.TrackMetadata != null && item.ArtistMetadata != null)
                    {
                        item.ArtistMetadata.Id = artistId;
                        item.TrackMetadata.ArtistIds = new List<string> { artistId };
                        item.AlbumMetadata.ArtistIds = new List<string> { artistId };
                    }
                }
            }

            foreach (var tracks in trackGroup)
            {
                var trackId = Guid.NewGuid().ToString();
                foreach (var item in tracks)
                {
                    if (item.AlbumMetadata != null && item.TrackMetadata != null && item.ArtistMetadata != null)
                    {
                        item.TrackMetadata.Id = trackId;
                        item.AlbumMetadata.TrackIds = new List<string> { trackId };
                        item.ArtistMetadata.TrackIds = new List<string> { trackId };
                    }
                }
            }

            __fileMetadata = new List<FileMetadata>(relatedMetadata);
        }

        /// <summary>
        /// Gets all unique albums. Make sure file metadata is already scanned.
        /// </summary>
        /// <returns>A list of unique <see cref="AlbumMetadata"/></returns>
        public IReadOnlyList<AlbumMetadata?> GetUniqueAlbumMetadata()
        {
            var albums = __fileMetadata.Select(c => c.AlbumMetadata).PruneNull();

            return albums.DistinctBy(c => c?.Id).ToList();
        }

        /// <summary>
        /// Gets all unique artist.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/></returns>
        public IReadOnlyList<ArtistMetadata?> GetUniqueArtistMetadata()
        {
            var artists = __fileMetadata.Select(c => c.ArtistMetadata).PruneNull();

            return artists.DistinctBy(c => c?.Id).ToList();
        }

        /// <summary>
        /// Gets all unique tracks. Make sure file meta is already scanned.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/></returns>
        public IReadOnlyList<TrackMetadata?> GetUniqueTrackMetadata()
        {
            var tracks = __fileMetadata.Select(c => c.TrackMetadata).PruneNull();

            return tracks.DistinctBy(c => c?.Id).ToList();
        }
    }
}
