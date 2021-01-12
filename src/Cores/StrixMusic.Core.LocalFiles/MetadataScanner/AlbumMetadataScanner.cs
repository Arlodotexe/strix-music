using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace StrixMusic.Core.LocalFiles.MetadataScanner
{
    /// <summary>
    /// Handles scanning albums for all supported metadata.
    /// </summary>
    public class AlbumMetadataScanner
    {
        /// <summary>
        /// Scans media file for metadata.
        /// </summary>
        /// <param name="fileData">The path to the file.</param>
        /// <returns>Fully scanned <see cref="AlbumMetadata"/>.</returns>
        public async Task<AlbumMetadata?> ScanAlbumMetadata(IFileData fileData)
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

        private AlbumMetadata MergeTrackMetadata(AlbumMetadata[] metadatas)
        {
            if (metadatas.Length == 1)
                return metadatas[0];

            var mergedMetaData = metadatas[0];
            for (int i = 1; i < metadatas.Length; i++)
            {
                var item = metadatas[i];

                if (mergedMetaData.DatePublished is null)
                    mergedMetaData.DatePublished = item.DatePublished;

                if (mergedMetaData.Genres is null)
                    mergedMetaData.Genres = item.Genres;

                if (mergedMetaData.TotalArtistsCount is null)
                    mergedMetaData.TotalArtistsCount = item.TotalArtistsCount;

                if (mergedMetaData.Duration is null)
                    mergedMetaData.Duration = item.Duration;

                if (mergedMetaData.TotalArtistsCount is null)
                    mergedMetaData.TotalArtistsCount = item.TotalArtistsCount;

                if (mergedMetaData.Description is null)
                    mergedMetaData.Description = item.Description;

                if (mergedMetaData.Title is null)
                    mergedMetaData.Title = item.Title;
            }

            return mergedMetaData;
        }

        private async Task<AlbumMetadata?> GetMusicFilesProperties(IFileData fileData)
        {
            var details = await fileData.Properties.GetMusicPropertiesAsync();

            if (details is null)
                return null;

            return new AlbumMetadata()
            {
                Title = details.Album,
                Duration = details.Duration,
                Genres = new List<string>(details.Genre),
            };
        }

        private async Task<AlbumMetadata?> GetID3Metadata(IFileData fileData)
        {
            try
            {
                var stream = await fileData.GetStreamAsync();

                using var tagFile = File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);

                // Read the raw tags
                var tags = tagFile.GetTag(TagTypes.Id3v2);

                return new AlbumMetadata()
                {
                    Description = tags.Description,
                    Title = tags.Album,
                    Duration = tagFile.Properties.Duration,
                    Genres = new List<string>(tags.Genres),
                    DatePublished = tags.DateTagged,
                    TotalTracksCount = Convert.ToInt32(tags.TrackCount),
                    TotalArtistsCount = tags.AlbumArtists.Length,
                };
            }
            catch (UnsupportedFormatException)
            {
                return null;
            }
        }
    }
}
