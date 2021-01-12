using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StrixMusic.Core.LocalFiles.MetadataScanner
{
    /// <summary>
    /// Handles scanning playlists for all supported metadata.
    /// </summary>
    public class PlaylistMetadataScanner
    {
        /// <summary>
        /// Scans playlist file for metadata.
        /// </summary>
        /// <param name="fileData">The path to the file.</param>
        /// <returns>Fully scanned <see cref="PlaylistMetadata"/>.</returns>
        public async Task<PlaylistMetadata?> ScanPlaylistMetadata(IFileData fileData)
        {
            var smilMetadata = await GetSmilMetadata(fileData);

            // disabled for now, scanning non-songs returns valid data
            // var propertyMetadata = await GetMusicFilesProperties(fileData);
            var foundMetadata = new[] { smilMetadata };

            var validMetadatas = foundMetadata.PruneNull().ToArray();

            if (validMetadatas.Length == 0)
                return null;

            var mergedTrackMetada = MergePlaylistMetadata(validMetadatas);
            mergedTrackMetada.Id = Guid.NewGuid().ToString();

            return mergedTrackMetada;
        }

        private PlaylistMetadata MergePlaylistMetadata(PlaylistMetadata[] metadatas)
        {
            if (metadatas.Length == 1)
                return metadatas[0];

            var mergedMetaData = metadatas[0];
            for (int i = 1; i < metadatas.Length; i++)
            {
                var item = metadatas[i];

                if (mergedMetaData.Url is null)
                    mergedMetaData.Url = item.Url;

                if (mergedMetaData.TrackIds is null)
                    mergedMetaData.TrackIds = item.TrackIds;

                if (mergedMetaData.Duration is null)
                    mergedMetaData.Duration = item.Duration;

                if (mergedMetaData.Description is null)
                    mergedMetaData.Description = item.Description;

                if (mergedMetaData.Title is null)
                    mergedMetaData.Title = item.Title;
            }

            return mergedMetaData;
        }

        /// <summary>
        /// Gets the SMIL metadata from the given file.
        /// </summary>
        /// <remarks>Recognizes Zune's ZPL and WMP's WPL.</remarks>
        private async Task<PlaylistMetadata?> GetSmilMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync();

                var doc = XDocument.Load(stream);
                var smil = doc.Root;
                var seq = smil.Element("seq");
                var head = smil.Element("head");

                return new PlaylistMetadata()
                {
                    Title = head.Element("title").Value,
                    TotalTracksCount = int.Parse(head.Elements()
                        .First(e => e.Name == "meta" && e.Attribute("name").Value == "itemCount").Attribute("content").Value),
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
