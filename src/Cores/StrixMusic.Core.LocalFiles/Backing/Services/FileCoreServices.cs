using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Core.LocalFiles.MetadataScanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    /// <summary>
    /// Holds the objects of all services.
    /// </summary>
    public class FileCoreServices
    {
        private ArtistService? _artistService;
        private AlbumService? _albumService;
        private TrackService? _trackService;
        private PlaylistService? _playlistService;

        /// <summary>
        /// Creates new instance of <see cref="FileCoreServices"/>.
        /// </summary>
        /// <param name="fileSystemService">The filesystem service used by metadata services.</param>
        /// <param name="fileMetadataScanner">The <see cref="FileMetadataScanner"> used by <see cref="PlaylistService"/>.</param>
        public FileCoreServices(IFileSystemService fileSystemService, FileMetadataScanner fileMetadataScanner)
        {
            _trackService = new TrackService(fileSystemService, fileMetadataScanner);
            _albumService = new AlbumService(fileSystemService, fileMetadataScanner);
            _artistService = new ArtistService(fileSystemService, fileMetadataScanner);
            _playlistService = new PlaylistService(fileSystemService);

            Guard.IsNotNull(_trackService, nameof(_trackService));
            Guard.IsNotNull(_albumService, nameof(_albumService));
            Guard.IsNotNull(_artistService, nameof(_artistService));
            Guard.IsNotNull(_playlistService, nameof(_playlistService));
        }

        /// <summary>
        /// A <see cref="ArtistMetadata"/> service.
        /// </summary>
        public ArtistService? ArtistService => _artistService;

        /// <summary>
        /// A <see cref="AlbumService"/> service.
        /// </summary>
        public AlbumService? AlbumService => _albumService;

        /// <summary>
        /// A <see cref="PlaylistService"/> service.
        /// </summary>
        public PlaylistService? PlaylistService => _playlistService;

        /// <summary>
        /// A <see cref="TrackService"/> service.
        /// </summary>
        public TrackService? TrackService => _trackService;
    }
}
