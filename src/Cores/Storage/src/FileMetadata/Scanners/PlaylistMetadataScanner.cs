using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

/// <summary>
/// Handles extracting playlist metadata from files.
/// </summary>
internal static partial class PlaylistMetadataScanner
{
    /// <summary>
    /// Scans playlist file for metadata.
    /// </summary>
    /// <param name="playlistFile">The path to the file.</param>
    /// <param name="fileMetadatas">The relevant files to link data to.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Fully scanned <see cref="PlaylistMetadata"/>.</returns>
    public static async Task<PlaylistMetadata?> ScanPlaylistFileAsync(IFile playlistFile, IList<IFile> knownFiles, CancellationToken cancellationToken = default) => Path.GetExtension(playlistFile.Name) switch
    {
        ".zpl" => await GetSmilMetadata(playlistFile, knownFiles, cancellationToken),
        ".wpl" => await GetSmilMetadata(playlistFile, knownFiles, cancellationToken),
        ".smil" => await GetSmilMetadata(playlistFile, knownFiles, cancellationToken),
        ".m3u" => await GetM3UMetadata(playlistFile, knownFiles, cancellationToken),
        ".m3u8" => await GetM3UMetadata(playlistFile, knownFiles, cancellationToken),
        ".vlc" => await GetM3UMetadata(playlistFile, knownFiles, cancellationToken),
        ".xspf" => await GetXspfMetadata(playlistFile, knownFiles, cancellationToken),
        ".asx" => await GetAsxMetadata(playlistFile, knownFiles, cancellationToken),
        ".mpcpl" => await GetMpcplMetadata(playlistFile, knownFiles, cancellationToken),
        ".fpl" => await GetFplMetadata(playlistFile, knownFiles, cancellationToken),
        ".pls" => await GetPlsMetadata(playlistFile, knownFiles, cancellationToken),
        ".aimppl4" => await GetAimpplMetadata(playlistFile, knownFiles, cancellationToken),
        _ => null
    };

    private enum AimpplPlaylistMode
    {
        Summary,
        Settings,
        Content,
    }
}
