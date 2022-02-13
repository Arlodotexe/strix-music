using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.Plugins.Model
{

    /// <summary>
    /// A model plugin is one or more implementation of <see cref="IModelPlugin"/>
    /// that modifies the behavior of an interface implementation
    /// by wrapping around an existing instance and selectively overriding members.
    /// 
    /// <para/> This class contains a chainable plugin builder for every relevant model interface used in the Strix Music SDK.
    ///         When the chain is built, the first added Plugin is returned, with the next Plugin provided during construction for 
    ///         proxying, which also had the next item passed into it during construction, and so on.
    /// <para/> When accessing a member, a plugin may retreive data from the next plugin (or, if none, the actual implementation), and
    ///         transform or replace it as needed.
    ///         A plugin may ignore the inner implementation entirely and supply new data.
    ///         Or, a plugin might not override that member and simply relay data from the actual implementation.
    /// </summary>
    public class SdkModelPlugins
    {
        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IDownloadable"/>.
        /// </summary>
        public ChainedProxyBuilder<DownloadablePluginBase, IDownloadable> Downloadable { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IPlayable"/>.
        /// </summary>
        public ChainedProxyBuilder<PlayablePluginBase, IPlayable> Playable { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IImage"/>.
        /// </summary>
        public ChainedProxyBuilder<ImagePluginBase, IImage> Image { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="ILyrics"/>.
        /// </summary>
        public ChainedProxyBuilder<LyricsPluginBase, ILyrics> Lyrics { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="ITrackCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<TrackCollectionPluginBase, ITrackCollection> TrackCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IArtistCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<ArtistCollectionPluginBase, IArtistCollection> ArtistCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IAlbumCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<AlbumCollectionPluginBase, IAlbumCollection> AlbumCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IPlaylistCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<PlaylistCollectionPluginBase, IPlaylistCollection> PlaylistCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IImageCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<ImageCollectionPluginBase, IImageCollection> ImageCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IUrlCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<UrlCollectionPluginBase, IUrlCollection> UrlCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IGenreCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<GenreCollectionPluginBase, IGenreCollection> GenreCollection { get; } = new();
    }
}
