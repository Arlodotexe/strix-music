using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Models;
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
    public class TrackMetadataScanner
    {
        /// <summary>
        /// Scans media file for metadata.
        /// </summary>
        /// <param name="fileData">The path to the file.</param>
        /// <returns>Fully scanned <see cref="TrackMetadata"/>.</returns>
        public async Task<TrackMetadata?> ScanTrackMetadata(IFileData fileData)
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

        private TrackMetadata MergeTrackMetadata(TrackMetadata[] metadatas)
        {
            if (metadatas.Length == 1)
                return metadatas[0];

            var mergedMetaData = metadatas[0];
            for (int i = 1; i < metadatas.Length; i++)
            {
                var item = metadatas[i];

                if (mergedMetaData.TrackNumber is null)
                    mergedMetaData.TrackNumber = item.TrackNumber;

                if (mergedMetaData.Genres is null)
                    mergedMetaData.Genres = item.Genres;

                if (mergedMetaData.DiscNumber is null)
                    mergedMetaData.DiscNumber = item.DiscNumber;

                if (mergedMetaData.Duration is null)
                    mergedMetaData.Duration = item.Duration;

                if (mergedMetaData.Lyrics is null)
                    mergedMetaData.Lyrics = item.Lyrics;

                if (mergedMetaData.Language is null)
                    mergedMetaData.Language = item.Language;

                if (mergedMetaData.Description is null)
                    mergedMetaData.Description = item.Description;

                if (mergedMetaData.Title is null)
                    mergedMetaData.Title = item.Title;

                if (mergedMetaData.Url is null)
                    mergedMetaData.Url = item.Url;
            }

            return mergedMetaData;
        }

        private async Task<TrackMetadata?> GetMusicFilesProperties(IFileData fileData)
        {
            var details = await fileData.Properties.GetMusicPropertiesAsync();

            if (details is null)
                return null;

            return new TrackMetadata()
            {
                Id = Guid.NewGuid().ToString(),
                TrackNumber = details.TrackNumber,
                Title = details.Title,
                Genres = details.Genre?.ToList(),
                Duration = details.Duration,
                Url = new Uri(fileData.Path),
            };
        }

        private async Task<TrackMetadata?> GetID3Metadata(IFileData fileData)
        {
            try
            {
                var stream = await fileData.GetStreamAsync();

                using var tagFile = File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);

                // Read the raw tags
                var tags = tagFile.GetTag(TagTypes.Id3v2);

                return new TrackMetadata()
                {
                    Description = tags.Description,
                    Title = tags.Title,
                    DiscNumber = tags.Disc,
                    Duration = tagFile.Properties.Duration,
                    Genres = new List<string>(tags.Genres),
                    TrackNumber = tags.Track,
                };
            }
            catch (UnsupportedFormatException)
            {
                return null;
            }
        }
    }
}
