using System;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// Given an implementation of <see cref="IFolderData"/> and <see cref="IFileData"/>, this manages scanning and caching all the music metadata from files in folder, including child folders.
    /// </summary>
    public interface IFileMetadataManager : IAsyncInit, IAsyncDisposable
    {
        /// <summary>
        /// Provides access to all albums in the given folder.
        /// </summary>
        AlbumRepository Albums { get; }

        /// <summary>
        /// Provides access to all artists in the given folder.
        /// </summary>
        ArtistRepository Artists { get; }

        /// <summary>
        /// Provides access to all playlists in the given folder.
        /// </summary>
        PlaylistRepository Playlists { get; }

        /// <summary>
        /// Provides access to all tracks in the given folder.
        /// </summary>
        TrackRepository Tracks { get; }

        /// <summary>
        /// Raised when metadata scanning has started.
        /// </summary>
        event EventHandler? ScanningStarted;

        /// <summary>
        /// Raised when metadata scanning has completed.
        /// </summary>
        event EventHandler? ScanningCompleted;

        /// <summary>
        /// Raised when a new file with metadata is discovered.
        /// </summary>
        event EventHandler<FileMetadata>? FileMetadataAdded;

        /// <summary>
        /// Raised when a previously scanned file has been removed from the file system.
        /// </summary>
        event EventHandler<FileMetadata>? FileMetadataRemoved;
    }
}