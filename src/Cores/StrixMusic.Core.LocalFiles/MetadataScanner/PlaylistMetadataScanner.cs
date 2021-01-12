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
                case ".vlc":    // TODO: Make sure this actually works with VLC files
                    playlistMetadata = await GetM3UMetadata(fileData);
                    break;

                case ".xspf":
                    playlistMetadata = await GetXspfMetadata(fileData);
                    break;

                case ".asx":
                    playlistMetadata = await GetAsxMetadata(fileData);
                    break;

                case ".mpcpl":
                    playlistMetadata = await GetMpcplMetadata(fileData);
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
        /// Resolves a potentially relative file path to an absolute path.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <param name="currentPath">The path to append to <paramref name="path"/>
        /// if it's relative.</param>
        /// <remarks>This method is safe to use on absolute paths as well.</remarks>
        /// <returns>An absolute path.</returns>
        public string ResolveFilePath(string path, string currentPath)
        {
            // Check if the path is absolute
            if (Regex.IsMatch(path, @"^(?:[a-zA-Z]\:)\\+"))
            {
                // Path is absolute
                return path;
            }
            else
            {
                // Path is relative
                string fullPath = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(currentPath), path));
                return fullPath;
            }
        }

        /// <summary>
        /// Resolves a potentially relative file path to an absolute path.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <param name="fileData">The file to resolve paths relative to.</param>
        /// <remarks>This method is safe to use on absolute paths as well.</remarks>
        /// <returns>An absolute path.</returns>
        public string ResolveFilePath(string path, IFileData fileData)
        {
            return ResolveFilePath(path, Path.GetDirectoryName(fileData.Path));
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
        /// <remarks>Recognizes both M3U (default encoding) and M3U8 (UTF-8 encoding).</remarks>
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
                        string fullPath = ResolveFilePath(line, fileData);
                        trackMetadataTemp.Url = new Uri(fullPath);
                        tracks.Add(trackMetadataTemp);

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

        /// <summary>
        /// Gets the XSPF metadata from the given file.
        /// </summary>
        /// <remarks>Does not support any application extensions.</remarks>
        private async Task<PlaylistMetadata?> GetXspfMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync();

                var doc = XDocument.Load(stream);
                var playlist = doc.Root;
                string xmlns = playlist.GetDefaultNamespace().NamespaceName;
                var tracklist = playlist.Element(XName.Get("trackList", xmlns));
                var trackListElements = tracklist.Elements(XName.Get("track", xmlns));

                var metadata = new PlaylistMetadata()
                {
                    Title = playlist.Element(XName.Get("title", xmlns))?.Value,
                    TotalTracksCount = trackListElements.Count(),
                    Description = playlist.Element(XName.Get("annotation", xmlns))?.Value,
                };
                string? url = playlist.Element(XName.Get("info", xmlns))?.Value;
                if (url != null)
                    metadata.Url = new Uri(url);

                // This is only temporary until we work out how to get track IDs
                var trackMetadata = new List<TrackMetadata>(metadata.TotalTracksCount);
                foreach (var media in trackListElements)
                {
                    int dur = int.Parse(media.Element(XName.Get("duration", xmlns))?.Value);

                    // TODO: Where does the track ID come from?
                    var track = new TrackMetadata
                    {
                        Title = media.Element(XName.Get("title", xmlns))?.Value,
                        Url = new Uri(media.Element(XName.Get("location", xmlns))?.Value),
                        Duration = new TimeSpan(0, 0, 0, 0, dur),
                        Description = media.Element(XName.Get("annotation", xmlns))?.Value,
                    };
                    string? trackNum = media.Element(XName.Get("trackNum", xmlns))?.Value;
                    if (trackNum != null)
                        track.TrackNumber = uint.Parse(trackNum);

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
        /// Gets the ASX metadata from the given file.
        /// </summary>
        /// <remarks>Does not support ENTRYREF.</remarks>
        private async Task<PlaylistMetadata?> GetAsxMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync();

                var doc = XDocument.Load(stream);
                var asx = doc.Root;
                var entries = asx.Elements("entry");
                string baseUrl = asx.Element("base")?.Value ?? string.Empty;

                var metadata = new PlaylistMetadata()
                {
                    Title = asx.Element("title").Value,
                    TotalTracksCount = entries.Count(),
                };

                // This is only temporary until we work out how to get track IDs
                var trackMetadata = new List<TrackMetadata>(metadata.TotalTracksCount);
                foreach (var entry in entries)
                {
                    string entryBaseUrl = entry.Element("base")?.Value ?? string.Empty;

                    // TODO: Where does the track ID come from?
                    var track = new TrackMetadata
                    {
                        Title = entry.Element("title")?.Value,
                        Url = new Uri(baseUrl + entryBaseUrl + entry.Element("ref").Attribute("href").Value),
                    };
                    string? durString = entry.Element("duration")?.Value;
                    if (durString != null)
                        track.Duration = TimeSpan.Parse(durString);

                    trackMetadata.Add(track);
                }

                // TODO: ASX files can reference other ASX files using ENTRYREF.
                // It works kind of like UWP XAML's ItemsPresenter:
                // https://docs.microsoft.com/en-us/windows/win32/wmp/entryref-element

                return metadata;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the MPC-PL metadata from the given file.
        /// </summary>
        private async Task<PlaylistMetadata?> GetMpcplMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync();
                StreamReader content = new StreamReader(stream);

                var metadata = new PlaylistMetadata();
                var tracks = new List<TrackMetadata>();

                // Make sure the file is either a "pointer" to a folder
                // or an M3U playlist
                string firstLine = await content.ReadLineAsync();
                if (firstLine != "MPCPLAYLIST")
                    return null;

                while (!content.EndOfStream)
                {
                    var trackMetadata = new TrackMetadata();

                    var line = await content.ReadLineAsync();
                    var components = Regex.Match(line, @"^(?<idx>[0-9]+),(?<attr>[A-z]+),(?<val>.+)$").Groups;

                    switch (components["attr"].Value)
                    {
                        case "filename":
                            string fullPath = ResolveFilePath(components["val"].Value, fileData.Path);
                            trackMetadata.Url = new Uri(fullPath);

                            int idx = int.Parse(components["idx"].Value);
                            if (idx >= tracks.Count)
                                tracks.Add(trackMetadata);
                            else
                                tracks.Insert(idx, trackMetadata);
                            break;

                        case "type":
                            // TODO: No idea what this is supposed to mean.
                            // It's not documented anywhere. Probably supposed to be an enum.
                        default:
                            // Unsupported attribute
                            break;
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
