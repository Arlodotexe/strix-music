using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            PlaylistMetadata? playlistMetadata = null;
            switch (fileData.FileExtension)
            {
                case ".zpl":
                case ".wpl":
                    playlistMetadata = await GetSmilMetadata(fileData);
                    break;

                case ".m3u":
                case ".m3u8":
                case ".vlc":
                    playlistMetadata = await GetM3UMetadata(fileData);
                    break;
            }

            // disabled for now, scanning non-songs returns valid data
            // var propertyMetadata = await GetMusicFilesProperties(fileData);
            var foundMetadata = new[] { playlistMetadata };

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
                var seq = smil.Element("body").Element("seq");
                var head = smil.Element("head");

                var metadata = new PlaylistMetadata()
                {
                    Title = head.Element("title").Value,
                    TotalTracksCount = int.Parse(head.Elements()
                        .First(e => e.Name == "meta" && e.Attribute("name").Value == "itemCount").Attribute("content").Value),
                };

                // This is only temporary until we work out how to get track IDs
                var trackMetadata = new List<TrackMetadata>(seq.Elements().Count());
                foreach (var media in seq.Elements("media"))
                {
                    // TODO: Where does the track ID come from?
                    var track = new TrackMetadata
                    {
                        Title = media.Attribute("trackTitle")?.Value,
                        Url = new Uri(media.Attribute("src")?.Value),
                        Duration = new TimeSpan(0, 0, 0, 0, int.Parse(media.Attribute("duration")?.Value)),
                    };

                    trackMetadata.Add(track);
                }

                return metadata;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the M3U metadata from the given file.
        /// </summary>
        private async Task<PlaylistMetadata?> GetM3UMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync();
                StreamReader content;
                if (fileData.FileExtension == ".m3u8")
                    content = new StreamReader(stream, Encoding.UTF8);
                else
                    content = new StreamReader(stream);

                var metadata = new PlaylistMetadata();
                var trackMetadataTemp = new TrackMetadata();
                var tracks = new List<TrackMetadata>();

                // Make sure the file is either a "pointer" to a folder
                // or an M3U playlist
                string firstLine = await content.ReadLineAsync();
                if (firstLine != "#EXTM3U")
                {
                    if (Directory.Exists(firstLine))
                    {
                        // TODO: Path exists, create a playlist with the tracks in that folder
                        metadata.Title = Path.GetDirectoryName(firstLine);
                        metadata.Url = new Uri(firstLine);
                        return metadata;
                    }
                    else
                    {
                        // Not a valid M3U playlist
                        return null;
                    }
                }

                while (!content.EndOfStream)
                {
                    var line = await content.ReadLineAsync();

                    // Handle M3U directives
                    if (line[0] == '#')
                    {
                        // --++ Extended M3U ++--
                        // Playlist display title
                        if (line.StartsWith("#PLAYLIST:"))
                        {
                            metadata.Title = line.Split(':')[1];
                        }
                        else if (line.StartsWith("#EXTINF:"))
                        {
                            var parameters = line.Split(':')[1].Split(',');
                            var metaArtistAndTitle = parameters[1].Split('-');

                            trackMetadataTemp.Duration = new TimeSpan(0, 0, int.Parse(parameters[0]));
                            trackMetadataTemp.Title = metaArtistAndTitle[1].Trim();
                        }
                        else
                        {
                            // Directive not recognized, might be a comment
                        }
                    }
                    else
                    {
                        // Assume the line is a path to a music file

                        // Check if the path is absolute
                        if (Regex.IsMatch(line, @"^(?:[a-zA-Z]\:)\\+"))
                        {
                            // Path is absolute
                            trackMetadataTemp.Url = new Uri(line);
                            tracks.Add(trackMetadataTemp);
                        }
                        else
                        {
                            // Path is relative
                            string fullPath = Path.GetFullPath(Path.Combine(
                                Path.GetDirectoryName(fileData.Path), line
                            ));
                            trackMetadataTemp.Url = new Uri(fullPath);
                            tracks.Add(trackMetadataTemp);
                        }

                        trackMetadataTemp = new TrackMetadata();
                    }
                }

                return metadata;
            }
            catch
            {
                return null;
            }
        }
    }
}
