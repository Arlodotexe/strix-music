using System;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Sdk.FileMetadataManagement.Repositories;

namespace StrixMusic.Sdk.FileMetadataManagement
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
        /// Provides access to all images in the given folder.
        /// </summary>
        ImageRepository Images { get; }

        /// <summary>
        /// If true, the repositories will not be initialized when <see cref="IAsyncInit.InitAsync"/> is called for this <see cref="IFileMetadataManager"/>.
        /// </summary>
        bool SkipRepoInit { get; set; }

        /// <summary>
        /// Gets or sets the type of metadata scan that should be used.
        /// </summary>
        MetadataScanTypes ScanTypes { get; set; }

        /// <summary>
        /// The number of files that are scanned concurrently.
        /// </summary>
        int DegreesOfParallelism { get; set; }

        /// <summary>
        /// Starts scanning the given folder.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task StartScan();

        /// <summary>
        /// Raised when metadata scanning has started.
        /// </summary>
        event EventHandler? ScanningStarted;

        /// <summary>
        /// Raised when metadata scanning has completed.
        /// </summary>
        event EventHandler? ScanningCompleted;
    }
}