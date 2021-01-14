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
                case ".smil":
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

                case ".fpl":
                    playlistMetadata = await GetFplMetadata(fileData);
                    break;

                case ".pls":
                    playlistMetadata = await GetPlsMetadata(fileData);
                    break;

                default:
                    // Format not supported
                    return null;
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
        /// <remarks>
        /// This method is safe to use on absolute paths as well.
        /// Does not work for Unix paths.
        /// </remarks>
        /// <returns>An absolute path.</returns>
        public string ResolveFilePath(string path, string currentPath)
        {
            // Check if the path is absolute
            if (IsFullPath(path))
            {
                // Path is absolute
                return path;
            }
            else
            {
                // Path is relative
                string fullPath;
                if (path.StartsWith("~"))
                {
                    // Unix relative file path
                    fullPath = Path.GetFullPath(Path.GetDirectoryName(currentPath) + path.Substring(1));

                }
                else
                {
                    fullPath = Path.GetFullPath(Path.Combine(
                        Path.GetDirectoryName(currentPath), path));
                }

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
        /// Determines whether a given path is full or relative.
        /// </summary>
        public bool IsFullPath(string path)
        {
            // FIXME: http:// paths are not recognized as absolute paths
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || !Path.IsPathRooted(path))
                return false;

            string pathRoot = Path.GetPathRoot(path);
            if (pathRoot.Length <= 2 && pathRoot != "/") // Accepts X:\ and \\UNC\PATH, rejects empty string, \ and X:, but accepts / to support Linux
                return false;

            if (pathRoot[0] != '\\' || pathRoot[1] != '\\')
                return true; // Rooted and not a UNC path

            return pathRoot.Trim('\\').IndexOf('\\') != -1; // A UNC server name without a share name (e.g "\\NAME" or "\\NAME\") is invalid
        }

        /// <summary>
        /// Gets the SMIL metadata from the given file.
        /// </summary>
        /// <remarks>Recognizes Zune's ZPL and WMP's WPL.</remarks>
        private async Task<PlaylistMetadata?> GetSmilMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);

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
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);
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
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);

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
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);

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
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);
                StreamReader content = new StreamReader(stream);

                var metadata = new PlaylistMetadata();
                var tracks = new List<TrackMetadata>();

                // Make sure the file is either a "pointer" to a folder
                // or an MPC playlist
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

        private static readonly byte[] FplMagic = new byte[] { 0xE1, 0xA0, 0x9C, 0x91, 0xF8, 0x3C, 0x77, 0x42, 0x85, 0x2C, 0x3B, 0xCC, 0x14, 0x01, 0xD3, 0xF2 };

        /// <summary>
        /// Gets the FPL metadata from the given file.
        /// </summary>
        /// <remarks>
        /// Supports playlists created by foobar2000 v0.9.1 and newer.
        /// Based on the specification here: https://github.com/rr-/fpl_reader/blob/master/fpl-format.md
        /// </remarks>
        private async Task<PlaylistMetadata?> GetFplMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);
                using var content = new BinaryReader(stream);

                var metadata = new PlaylistMetadata();
                var tracks = new List<TrackMetadata>();

                // Make sure the file is an FPL
                byte[] fileMagic = content.ReadBytes(FplMagic.Length);
                if (!fileMagic.SequenceEqual(FplMagic))
                    return null;

                // foobar2000 playlists don't have titles, so set it
                // to the file name
                metadata.Title = fileData.DisplayName;

                // Get size of meta
                uint metaSize = content.ReadUInt32();

                // Read meta strings (null-terminated)
                byte[] metaBytes = new byte[metaSize];
                long metaPos = stream.Position;
                await stream.ReadAsync(metaBytes, 0, metaBytes.Length);
                var metas = new List<string>();
                string metaTemp = string.Empty;
                for (int i = 0; i < metaBytes.Length; i++)
                {
                    byte b = metaBytes[i];
                    if (b == 0x00)
                    {
                        // End of string
                        metas.Add(metaTemp);
                        metaTemp = string.Empty;
                    }
                    else
                    {
                        // TODO: Is there a better way to do this
                        metaTemp += Encoding.UTF8.GetChars(new[] { b })[0];
                    }
                }

                // Get track count
                uint trackCount = content.ReadUInt32();

                // Read track metadata
                var trackMetadatas = new List<TrackMetadata>();
                for (int i = 0; i < trackCount; i++)
                {
                    var trackMetadata = new TrackMetadata();

                    // Get flags
                    int flags = content.ReadInt32();

                    // Get file name offset
                    uint fileNameOffset = content.ReadUInt32();

                    // Retrieve file name
                    long curPos = stream.Position;
                    stream.Seek(metaPos + fileNameOffset, SeekOrigin.Begin);
                    trackMetadata.Url = new Uri(stream.ReadNullTerminatedString(Encoding.UTF8));
                    stream.Seek(curPos, SeekOrigin.Begin);

                    // Get sub-song index (for files containing multiple tracks, like chapters)
                    uint subSongIndex = content.ReadUInt32();

                    // Check if the track has metadata
                    if ((flags & 1) == 0)
                    {
                        trackMetadatas.Add(trackMetadata);
                        continue;
                    }

                    // Get track file size
                    ulong fileSize = content.ReadUInt64();

                    // Get track file time (last modified)
                    ulong fileTime = content.ReadUInt64();

                    // Get track duration
                    double durationSeconds = content.ReadDouble();
                    trackMetadata.Duration = new TimeSpan(0, 0, (int)durationSeconds);

                    // Get rpg_album, rpg_track, rpk_album, rpk_track
                    // We don't need it but might as well read it
                    float rpgAlbum = content.ReadSingle();
                    float rpgTrack = content.ReadSingle();
                    float rpkAlbum = content.ReadSingle();
                    float rpkTrack = content.ReadSingle();

                    // Get entry count
                    int entryCount = (int)content.ReadUInt32();
                    int primaryKeyCount = (int)content.ReadUInt32();
                    int secondaryKeyCount = (int)content.ReadUInt32();
                    int secondaryKeysOffset = (int)content.ReadUInt32();

                    var primaryPairs = new Dictionary<string, string>(primaryKeyCount);

                    // Get primary keys
                    var primaryKeys = new Dictionary<uint, string>(primaryKeyCount);
                    for (int x = 0; x < primaryKeyCount; x++)
                    {
                        uint id = content.ReadUInt32();
                        uint nameOffset = content.ReadUInt32();
                        curPos = stream.Position;
                        stream.Seek(metaPos + nameOffset, SeekOrigin.Begin);
                        primaryKeys[id] = stream.ReadNullTerminatedString(Encoding.UTF8);
                        stream.Seek(curPos, SeekOrigin.Begin);
                    }

                    // Read 'unk0', no idea what it does
                    uint unk0 = content.ReadUInt32();

                    // Get primary pair values
                    string previousPrimaryKey = primaryKeys.First().Value;
                    for (uint x = 0; x < primaryKeyCount; x++)
                    {
                        uint valueOffset = content.ReadUInt32();
                        curPos = stream.Position;
                        stream.Seek(metaPos + valueOffset, SeekOrigin.Begin);
                        string value = stream.ReadNullTerminatedString(Encoding.UTF8);
                        stream.Seek(curPos, SeekOrigin.Begin);

                        if (primaryKeys.ContainsKey(x))
                            previousPrimaryKey = primaryKeys[x];

                        primaryPairs.Add(previousPrimaryKey, value);
                    }

                    // Get secondary pairs
                    var secondaryPairs = new Dictionary<string, string>(secondaryKeyCount);
                    for (int x = 0; x < secondaryKeyCount; x++)
                    {
                        // Read key
                        uint keyOffset = content.ReadUInt32();
                        curPos = stream.Position;
                        stream.Seek(metaPos + keyOffset, SeekOrigin.Begin);
                        string key = stream.ReadNullTerminatedString(Encoding.UTF8);
                        stream.Seek(curPos, SeekOrigin.Begin);

                        // Read value
                        uint valueOffset = content.ReadUInt32();
                        curPos = stream.Position;
                        stream.Seek(metaPos + valueOffset, SeekOrigin.Begin);
                        string value = stream.ReadNullTerminatedString(Encoding.UTF8);
                        stream.Seek(curPos, SeekOrigin.Begin);

                        secondaryPairs.Add(key, value);
                    }

                    // Check flag for 64 bits of padding
                    if ((flags & 0x04) == 1)
                        stream.Seek(64, SeekOrigin.Current);

                    // Populate TrackMetadata
                    if (primaryPairs.TryGetValue("title", out string title))
                        trackMetadata.Title = title;
                    if (primaryPairs.TryGetValue("discnumber", out string discNumStr))
                        trackMetadata.DiscNumber = uint.Parse(discNumStr);
                    if (primaryPairs.TryGetValue("tracknumber", out string trackNumStr))
                        trackMetadata.TrackNumber = uint.Parse(trackNumStr);
                    if (primaryPairs.TryGetValue("genre", out string genre))
                        trackMetadata.Genres = genre.IntoList();

                    // Add the current track to the list
                    trackMetadatas.Add(trackMetadata);
                }

                return metadata;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the PLS metadata from the given file.
        /// </summary>
        private async Task<PlaylistMetadata?> GetPlsMetadata(IFileData fileData)
        {
            try
            {
                using var stream = await fileData.GetStreamAsync(FileAccessMode.Read);
                StreamReader content = new StreamReader(stream);

                var metadata = new PlaylistMetadata();

                // Make sure the file is either a "pointer" to a folder
                // or an M3U playlist
                string firstLine = await content.ReadLineAsync();
                if (firstLine != "[playlist]")
                    // Not a valid PLS playlist
                    return null;

                var matches = new List<Match>();
                while (!content.EndOfStream)
                {
                    var line = await content.ReadLineAsync();
                    var match = Regex.Match(line, @"^(?<key>[A-Za-z]+)(?<idx>[0-9]*)=(?<val>.+)$", RegexOptions.Compiled);
                    if (match.Success)
                        matches.Add(match);
                }

                Match trackCountMatch = matches.First(m => m.Groups["key"].Value == "NumberOfEntries");
                uint trackCount = uint.Parse(trackCountMatch.Groups["val"].Value);
                matches.Remove(trackCountMatch);
                var tracksTable = new Dictionary<int, TrackMetadata>((int)trackCount);

                foreach (var match in matches)
                {
                    string value = match.Groups["val"].Value;
                    string? indexStr = match.Groups["idx"]?.Value;
                    if (int.TryParse(indexStr, out int index))
                    {
                        if (!tracksTable.ContainsKey(index))
                            tracksTable[index] = new TrackMetadata();
                    }

                    switch (match.Groups["key"].Value)
                    {
                        case "File":
                            tracksTable[index].Source = new Uri(ResolveFilePath(value, fileData));
                            break;

                        case "Length":
                            tracksTable[index].Duration = new TimeSpan(0, 0, int.Parse(value));
                            break;

                        case "Title":
                            tracksTable[index].Title = value;
                            break;
                    }
                }

                // Collapse the tracks table to a plain list
                List<TrackMetadata> tracks = tracksTable.Select(t => t.Value).PruneNull().ToList();

                return metadata;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the AIMPPL metadata from the given file.
        /// </summary>
        /// <remarks>Only tested with AIMPPL4 files.</remarks>
        private async Task<PlaylistMetadata?> GetAimpplMetadata(IFileData fileData)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch
            {
                return null;
            }
        }
    }
}
