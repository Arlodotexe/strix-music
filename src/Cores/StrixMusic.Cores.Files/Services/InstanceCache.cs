using System;
using OwlCore.Services;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Sdk.FileMetadataManagement.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Files.Services
{
    /// <summary>
    /// Contains caches for all instance created across all core instances.
    /// </summary>
    /// <remarks>
    /// A "file core" has the possibility of using a folder which is a subfolder of the root selected in another core instance.
    /// This means the same exact data will appear in two different core instances.
    /// <para/>
    /// Data from multiple different core instances are merged together by the Strix SDK, but this is computationally expensive.
    /// <para/>
    /// By using a shared repository between all core instances, we reduce remove extra overhead when a single file is updated.
    /// </remarks>
    public static class InstanceCache
    {
        /// <summary>
        /// A cache of all albums across all core instances.
        /// </summary>
        public static AlbumCacheRepo Albums { get; set; } = new AlbumCacheRepo();

        /// <summary>
        /// A cache of all artists across all core instances.
        /// </summary>
        public static ArtistCacheRepo Artists { get; set; } = new ArtistCacheRepo();

        /// <summary>
        /// A cache of all tracks across all core instances.
        /// </summary>
        public static TrackCacheRepo Tracks { get; set; } = new TrackCacheRepo();

        /// <summary>
        /// A cache of all images across all core instances.
        /// </summary>
        public static ImageCacheRepo Images { get; set; } = new ImageCacheRepo();

        /// <summary>
        /// A cache of all playlists across all core instances.
        /// </summary>
        public static PlaylistCacheRepo Playlists { get; set; } = new PlaylistCacheRepo();
    }

    /// <summary>
    /// A cache of all albums across all core instances.
    /// </summary>
    public class AlbumCacheRepo : InstanceCacheRepository<FilesCoreAlbum>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public FilesCoreAlbum GetOrCreate(string id, ICore sourceCore, AlbumMetadata albumMetadata)
        {
            return GetOrCreate(id, () => new FilesCoreAlbum(sourceCore, albumMetadata));
        }
    }

    /// <summary>
    /// A cache of all artists across all core instances.
    /// </summary>
    public class ArtistCacheRepo : InstanceCacheRepository<FilesCoreArtist>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public FilesCoreArtist GetOrCreate(string id, ICore sourceCore, ArtistMetadata artistMetadata)
        {
            return GetOrCreate(id, () => new FilesCoreArtist(sourceCore, artistMetadata));
        }
    }

    /// <summary>
    /// A cache of all tracks across all core instances.
    /// </summary>
    public class TrackCacheRepo : InstanceCacheRepository<FilesCoreTrack>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public FilesCoreTrack GetOrCreate(string id, ICore sourceCore, TrackMetadata trackMetadata)
        {
            return GetOrCreate(id, () => new FilesCoreTrack(sourceCore, trackMetadata));
        }
    }

    /// <summary>
    /// A cache of all images across all core instances.
    /// </summary>
    public class ImageCacheRepo : InstanceCacheRepository<FilesCoreImage>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, System.Func{T})"/>
        public FilesCoreImage GetOrCreate(string id, ICore sourceCore, ImageMetadata imageMetadata)
        {
            return GetOrCreate(id, () => new FilesCoreImage(sourceCore, imageMetadata));
        }
    }

    /// <summary>
    /// A cache of all playlists across all core instances.
    /// </summary>
    public class PlaylistCacheRepo : InstanceCacheRepository<LocalFilesCorePlaylist>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public LocalFilesCorePlaylist GetOrCreate(string id, ICore sourceCore, PlaylistMetadata playlistMetadata)
        {
            return GetOrCreate(id, () => new LocalFilesCorePlaylist(sourceCore, playlistMetadata));
        }
    }
}