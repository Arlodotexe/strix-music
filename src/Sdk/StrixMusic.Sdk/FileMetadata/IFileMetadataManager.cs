// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Sdk.FileMetadata.Repositories;

namespace StrixMusic.Sdk.FileMetadata
{
    /// <summary>
    /// Given an implementation of OwlCore.AbstractStorage, this manages scanning and caching all the music metadata from files in folder, including child folders.
    /// </summary>
    public interface IFileMetadataManager : IAsyncInit, IAsyncDisposable
    {
        /// <summary>
        /// Stores album metadata.
        /// </summary>
        IAlbumRepository Albums { get; }

        /// <summary>
        /// Stores artist metadata.
        /// </summary>
        IArtistRepository Artists { get; }

        /// <summary>
        /// Stores playlist metadata.
        /// </summary>
        IPlaylistRepository Playlists { get; }

        /// <summary>
        /// Stores track metadata.
        /// </summary>
        ITrackRepository Tracks { get; }

        /// <summary>
        /// Stores image metadata.
        /// </summary>
        IImageRepository Images { get; }

        /// <summary>
        /// If true, the repositories will not be initialized when InitAsync is called for this <see cref="IFileMetadataManager"/>.
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