using OwlCore.Services;
using StrixMusic.Core.LocalFiles.Models;

namespace StrixMusic.Core.LocalFiles.Services
{
    /// <summary>
    /// Contains caches for all instance created across the core.
    /// </summary>
    public static class InstanceCacheRepo
    {
        //public static InstanceCacheRepository<LocalFilesCoreAlbum> _playlistCache = new InstanceCacheRepository<LocalFilesCorePlaylist>();

        /// <summary>
        /// A cache of all albums across all core instances.
        /// </summary>
        public static InstanceCacheRepository<LocalFilesCoreAlbum> AlbumCache { get; set; } = new InstanceCacheRepository<LocalFilesCoreAlbum>();

        /// <summary>
        /// A cache of all artists across all core instances.
        /// </summary>
        public static InstanceCacheRepository<LocalFilesCoreArtist> ArtistCache { get; set; } = new InstanceCacheRepository<LocalFilesCoreArtist>();

        /// <summary>
        /// A cache of all tracks across all core instances.
        /// </summary>
        public static InstanceCacheRepository<LocalFilesCoreTrack> TrackCache { get; set; } = new InstanceCacheRepository<LocalFilesCoreTrack>();

        /// <summary>
        /// A cache of all images across all core instances.
        /// </summary>
        public static InstanceCacheRepository<LocalFilesCoreImage> ImageCache { get; set; } = new InstanceCacheRepository<LocalFilesCoreImage>();
    }
}