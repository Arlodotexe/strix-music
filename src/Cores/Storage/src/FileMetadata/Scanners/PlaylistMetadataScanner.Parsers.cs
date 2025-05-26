using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Cores.Storage.FileMetadata.Models;
using StrixMusic.Cores.Storage.FileMetadata.Models.Playlist.Smil;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

internal partial class PlaylistMetadataScanner
{
    /// <summary>
    /// Gets tracks from the SMIL metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <param name="playlistFile">The path to the file.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    /// <remarks>Recognizes Zune's ZPL and WMP's WPL.</remarks>
    private static async Task<PlaylistMetadata?> GetSmilMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        var ser = new XmlSerializer(typeof(Smil));

        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);
        using var xmlReader = new XmlTextReader(stream);

        var smil = ser.Deserialize(xmlReader) as Smil;
        var playlist = new PlaylistMetadata
        {
            Id = playlistFile.Id,
        };

        var mediaList = smil?.Body?.Seq?.Media?.ToList();
        playlist.Title = smil?.Head?.Title;

        if (mediaList == null)
            return null;

        playlist.TrackIds = new HashSet<string>();

        foreach (var media in mediaList)
        {
            if (media.Src == null)
                return playlist;

            var track = await FindAndDigestRelativeTrackAsync(playlistFile, playlist, media.Src, knownFiles, cancellationToken);
            if (track is not null)
                playlist.TotalTrackCount++;
        }

        return playlist;
    }

    /// <summary>
    /// Gets tracks from the M3U metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <param name="playlistFile">The path to the file.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    /// <remarks>Recognizes both M3U (default encoding) and M3U8 (UTF-8 encoding).</remarks>
    private static async Task<PlaylistMetadata?> GetM3UMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);

        using var content = Path.GetExtension(playlistFile.Name) == ".m3u8" ? new StreamReader(stream, Encoding.UTF8) : new StreamReader(stream);

        var playlist = new PlaylistMetadata
        {
            Id = playlistFile.Id,
        };

        while (!content.EndOfStream)
        {
            var line = await content.ReadLineAsync();

            // Ignore empty lines
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Handle M3U directives
            if (line[0] == '#')
            {
                // --++ Extended M3U ++--
                // Playlist display title
                if (line.StartsWith("#PLAYLIST:", StringComparison.InvariantCulture))
                {
                    playlist.Title = line.Split(':')[1];
                }
            }
            else
            {
                await FindAndDigestRelativeTrackAsync(playlistFile, playlist, line, knownFiles, cancellationToken);
            }
        }

        playlist.Title ??= playlistFile.Name; // If the title is null, filename is assigned because if a playlist has no title its not visible to the user on UI.

        return playlist;
    }

    /// <summary>
    /// Gets tracks from the XSPF metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <remarks>Does not support any application extensions.</remarks>
    /// <param name="playlistFile">The file to scan for metadata.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    private static async Task<PlaylistMetadata?> GetXspfMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);

        var doc = XDocument.Load(stream);
        var xmlRoot = doc.Root;
        var xmlns = xmlRoot.GetDefaultNamespace().NamespaceName;
        var trackList = xmlRoot.Element(XName.Get("trackList", xmlns));
        var trackListElements = trackList.Elements(XName.Get("track", xmlns));

        var listElements = trackListElements as XElement[] ?? trackListElements.ToArray();

        var playlist = new PlaylistMetadata
        {
            Id = playlistFile.Id,
            Title = xmlRoot.Element(XName.Get("title", xmlns))?.Value,
            TotalTrackCount = listElements.Length,
            Description = xmlRoot.Element(XName.Get("annotation", xmlns))?.Value,
        };

        var url = xmlRoot.Element(XName.Get("info", xmlns))?.Value;
        if (url is not null && !string.IsNullOrWhiteSpace(url))
            playlist.Url = new Uri(url);

        foreach (var media in listElements)
        {
            var location = media.Element(XName.Get("location", xmlns))?.Value;
            if (location is null || string.IsNullOrWhiteSpace(location))
                continue;

            await FindAndDigestRelativeTrackAsync(playlistFile, playlist, location, knownFiles, cancellationToken);
        }

        return playlist;
    }

    /// <summary>
    /// Gets tracks from the ASX metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <param name="playlistFile">The file to scan for metadata.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    /// <remarks>Does not support ENTRYREF.</remarks>
    private static async Task<PlaylistMetadata?> GetAsxMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);

        var doc = XDocument.Load(stream);
        var asx = doc.Root;
        Guard.IsNotNull(asx);

        var entries = asx.Elements("entry");
        var baseUrl = asx.Element("base")?.Value ?? string.Empty;

        var playlist = new PlaylistMetadata
        {
            Id = playlistFile.Id,
            Title = asx.Element("title")?.Value,
        };

        foreach (var entry in entries)
        {
            var entryBaseUrl = entry.Element("base")?.Value ?? string.Empty;

            var path = $"{baseUrl} {entryBaseUrl} {entry.Element("ref")?.Attribute("href")?.Value}";

            await FindAndDigestRelativeTrackAsync(playlistFile, playlist, path, knownFiles, cancellationToken);
        }

        return playlist;
    }

    /// <summary>
    /// Gets tracks from the MPC-PL metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <param name="playlistFile">The file to scan for metadata.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    private static async Task<PlaylistMetadata?> GetMpcplMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);
        using var content = new StreamReader(stream);

        var playlist = new PlaylistMetadata();

        // Make sure the file is either a "pointer" to a folder
        // or an MPC playlist
        var firstLine = await content.ReadLineAsync();
        if (firstLine != "MPCPLAYLIST")
            return null;

        while (!content.EndOfStream)
        {
            var line = await content.ReadLineAsync();
            var components = Regex.Match(line, @"^(?<idx>[0-9]+),(?<attr>[A-z]+),(?<val>.+)$").Groups;

            switch (components["attr"].Value)
            {
                case "filename":
                    await FindAndDigestRelativeTrackAsync(playlistFile, playlist, components["val"].Value, knownFiles, cancellationToken);
                    break;

                case "type":
                // No idea what this is supposed to mean.
                // It's not documented anywhere. Probably supposed to be an enum.
                default:
                    // Unsupported attribute
                    break;
            }
        }

        return playlist;
    }

    /// <summary>
    /// Gets tracks from the FPL metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <param name="playlistFile">The file to scan for metadata.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    /// <remarks>
    /// Supports playlists created by foobar2000 v0.9.1 and newer.
    /// Based on the specification here: https://github.com/rr-/fpl_reader/blob/master/fpl-format.md
    /// </remarks>
    private static async Task<PlaylistMetadata?> GetFplMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        try
        {
            // The magic field is a 16-byte magic number.
            // More details: https://github.com/rr-/fpl_reader/blob/master/fpl-format.md#magic
            var fplMagic = new byte[]
            {
                0xE1, 0xA0, 0x9C, 0x91, 0xF8, 0x3C, 0x77, 0x42, 0x85, 0x2C, 0x3B, 0xCC, 0x14, 0x01, 0xD3, 0xF2
            };

            using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);
            using var content = new BinaryReader(stream);

            var playlist = new PlaylistMetadata()
            {
                Id = playlistFile.Id,
            };

            // Make sure the file is an FPL
            var fileMagic = content.ReadBytes(fplMagic.Length);
            if (!fileMagic.SequenceEqual(fplMagic))
                return null;

            // foobar2000 playlists don't have titles, so set it
            // to the file name
            playlist.Title = playlistFile.Name;

            // Get size of meta
            var metaSize = content.ReadUInt32();

            // Read meta strings (null-terminated)
            var metaBytes = new byte[metaSize];
            var metaPos = stream.Position;

            var bytesRead = await stream.ReadAsync(metaBytes, 0, metaBytes.Length, cancellationToken);

            Guard.IsEqualTo(bytesRead, metaBytes.Length);

            var metas = new List<string>();
            var metaTemp = string.Empty;

            foreach (var b in metaBytes)
            {
                if (b == 0x00)
                {
                    // End of string
                    metas.Add(metaTemp);
                    metaTemp = string.Empty;
                }
                else
                {
                    // TODO: Is there a better way to do this?
                    metaTemp += Encoding.UTF8.GetChars(new[] { b })[0];
                }
            }

            // Get track count
            var trackCount = content.ReadUInt32();

            for (var i = 0; i < trackCount; i++)
            {
                // Get flags
                var flags = content.ReadInt32();

                // Get file name offset
                var fileNameOffset = content.ReadUInt32();

                // Retrieve file name
                var curPos = stream.Position;
                stream.Seek(metaPos + fileNameOffset, SeekOrigin.Begin);
                var pathToTrack = stream.ReadNullTerminatedString(Encoding.UTF8);
                stream.Seek(curPos, SeekOrigin.Begin);

                playlist.TrackIds ??= new HashSet<string>();

                // Get track file size
                var fileSize = content.ReadUInt64();

                // Get track file time (last modified)
                var fileTime = content.ReadUInt64();

                // Get track duration
                var durationSeconds = content.ReadDouble();

                // Get rpg_album, rpg_track, rpk_album, rpk_track
                // We don't need it but might as well read it
                var rpgAlbum = content.ReadSingle();
                var rpgTrack = content.ReadSingle();
                var rpkAlbum = content.ReadSingle();
                var rpkTrack = content.ReadSingle();

                // Get entry count
                var entryCount = (int)content.ReadUInt32();
                var primaryKeyCount = (int)content.ReadUInt32();
                var secondaryKeyCount = (int)content.ReadUInt32();
                var secondaryKeysOffset = (int)content.ReadUInt32();

                var primaryPairs = new Dictionary<string, string>(primaryKeyCount);

                // Get primary keys
                var primaryKeys = new Dictionary<uint, string>(primaryKeyCount);
                for (var x = 0; x < primaryKeyCount; x++)
                {
                    var id = content.ReadUInt32();
                    var nameOffset = content.ReadUInt32();

                    curPos = stream.Position;
                    stream.Seek(metaPos + nameOffset, SeekOrigin.Begin);

                    primaryKeys[id] = stream.ReadNullTerminatedString(Encoding.UTF8);
                    stream.Seek(curPos, SeekOrigin.Begin);
                }

                // Read 'unk0', no idea what it does
                var unk0 = content.ReadUInt32();

                // Get primary pair values
                var previousPrimaryKey = primaryKeys.First().Value;
                for (uint x = 0; x < primaryKeyCount; x++)
                {
                    if (!string.IsNullOrEmpty(pathToTrack))
                    {
                        var valueOffset = content.ReadUInt32();

                        curPos = stream.Position;
                        stream.Seek(metaPos + valueOffset, SeekOrigin.Begin);

                        var value = stream.ReadNullTerminatedString(Encoding.UTF8);
                        stream.Seek(curPos, SeekOrigin.Begin);

                        var track = await FindAndDigestRelativeTrackAsync(playlistFile, playlist, pathToTrack, knownFiles, cancellationToken);

                        if (track != null)
                        {
                            Guard.IsNotNull(playlist.TrackIds);
                            playlist.TrackIds.Add(track.Id);
                        }

                        if (primaryKeys.TryGetValue(x, out var val))
                            previousPrimaryKey = val;

                        primaryPairs.Add(previousPrimaryKey, value);
                    }
                }
            }

            return playlist;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets tracks from the PLS metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    private static async Task<PlaylistMetadata?> GetPlsMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);
        using var content = new StreamReader(stream);

        var playlist = new PlaylistMetadata()
        {
            Id = playlistFile.Id,
            Title = playlistFile.Name,
        };

        // Make sure the file is really a PLS file
        var firstLine = await content.ReadLineAsync();

        // Not a valid PLS playlist
        if (firstLine != "[playlist]")
            return null;

        var matches = new List<Match>();
        while (!content.EndOfStream)
        {
            var line = await content.ReadLineAsync();
            var match = Regex.Match(line, @"^(?<key>[A-Za-z]+)(?<idx>[0-9]*)=(?<val>.+)$", RegexOptions.Compiled);
            if (match.Success)
                matches.Add(match);
        }

        var trackCountMatch = matches.First(m => m.Groups["key"].Value == "NumberOfEntries");
        var trackCount = uint.Parse(trackCountMatch.Groups["val"].Value, CultureInfo.InvariantCulture);

        matches.Remove(trackCountMatch);

        foreach (var match in matches)
        {
            var value = match.Groups["val"].Value;
            var indexStr = match.Groups["idx"]?.Value;

            if (match.Groups["key"].Value == "File")
            {
                await FindAndDigestRelativeTrackAsync(playlistFile, playlist, value, knownFiles, cancellationToken);
            }
        }

        return playlist;
    }

    /// <summary>
    /// Gets tracks from the AIMPPL metatdata in <paramref name="playlistFile"/> and links them to the given <paramref name="files"/>.
    /// </summary>
    /// <remarks>Only tested with AIMPPL4 files.</remarks>
    /// <param name="playlistFile">The file to scan for metadata.</param>
    /// <param name="knownFiles">A list of all known files. If a scanned playlist references a file, it must be traversable and must exist this list.</param>
    private static async Task<PlaylistMetadata?> GetAimpplMetadata(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        // Adapted from https://github.com/ApexWeed/aimppl-copy/
        using var stream = await playlistFile.OpenReadAsync(cancellationToken: cancellationToken);
        using var content = new StreamReader(stream);

        var playlist = new PlaylistMetadata()
        {
            Id = playlistFile.Id,
        };

        var mode = AimpplPlaylistMode.Summary;
        while (!content.EndOfStream)
        {
            var line = await content.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            switch (line)
            {
                case "#-----SUMMARY-----#":
                    mode = AimpplPlaylistMode.Summary;
                    continue;
                case "#-----SETTINGS-----#":
                    mode = AimpplPlaylistMode.Settings;
                    continue;
                case "#-----CONTENT-----#":
                    mode = AimpplPlaylistMode.Content;
                    continue;
                default:
                    switch (mode)
                    {
                        case AimpplPlaylistMode.Summary:
                            {
                                var split = line.IndexOf('=');
                                var variable = line.Substring(0, split);
                                var value = line.Substring(split + 1);
                                if (variable == "Name")
                                    playlist.Title = value;

                                break;
                            }

                        case AimpplPlaylistMode.Settings:
                            break;

                        case AimpplPlaylistMode.Content:
                            {
                                if (string.IsNullOrWhiteSpace(line))
                                    continue;

                                if (line.StartsWith("-", StringComparison.InvariantCulture))
                                    break;

                                await line.Split('|')
                                    .Where(x => Uri.IsWellFormedUriString(x, UriKind.Relative))
                                    .InParallel(x => FindAndDigestRelativeTrackAsync(playlistFile, playlist, x, knownFiles, cancellationToken));
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
            }
        }

        return playlist;
    }

    private static async Task<IStorable?> FindAndDigestRelativeTrackAsync(IFile playlistFile, PlaylistMetadata playlist, string relativeTrackPath, IList<IFile> knownFiles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (!Uri.IsWellFormedUriString(relativeTrackPath, UriKind.Relative))
            return null;

        try
        {
            var targetFile = await playlistFile.TraverseRelativePathAsync(relativeTrackPath, cancellationToken: cancellationToken);

            if (knownFiles.All(x => x.Id != targetFile.Id))
                return null;

            playlist.TrackIds ??= new HashSet<string>();
            playlist.TrackIds.Add(targetFile.Id); // The track ID is always the ID of the scanned file.

            return targetFile;
        }
        catch (Exception ex) when (ex is ArgumentException or IOException)
        {
            // Invalid paths or missing files/folders are ignored
            return null;
        }
    }
}

