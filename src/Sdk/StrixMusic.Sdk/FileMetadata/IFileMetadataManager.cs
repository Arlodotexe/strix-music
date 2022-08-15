// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        /// Stores metadata about artists who contributed towards an album.
        /// </summary>
        IArtistRepository AlbumArtists { get; }

        /// <summary>
        /// Stores artist metadata that contributed towards one or more tracks.
        /// </summary>
        IArtistRepository TrackArtists { get; }

        /// <summary>
        /// Stores track metadata.
        /// </summary>
        ITrackRepository Tracks { get; }

        /// <summary>
        /// Stores playlist metadata.
        /// </summary>
        IPlaylistRepository Playlists { get; }

        /// <summary>
        /// Stores image metadata.
        /// </summary>
        IImageRepository Images { get; }

        /// <summary>
        /// Gets the stream for the provided image Id.
        /// </summary>
        /// <param name="imageId">The unique identifier for this image, created as part of a scan.</param>
        /// <returns>A Task containing the image stream, if found.</returns>
        /// <exception cref="ArgumentException">Couldn't extract scanned image type from image ID.</exception>
        public Task<Stream?> GetImageStreamById(string imageId);

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
        int DegreesOfParallelism { get; }

        /// <summary>
        /// Starts scanning the given folder.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ScanAsync(CancellationToken cancellationToken = default);

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
