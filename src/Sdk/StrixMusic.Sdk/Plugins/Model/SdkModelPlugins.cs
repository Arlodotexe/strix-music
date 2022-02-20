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
    /// <para/> When accessing a member, a plugin may retrieve data from the next plugin (or, if none, the actual implementation), and
    ///         transform or replace it as needed.
    ///         A plugin may ignore the inner implementation entirely and supply new data.
    ///         Or, a plugin might not override that member and simply relay data from the actual implementation.
    /// </summary>
    public class SdkModelPlugins
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdkModelPlugins"/> class.
        /// </summary>
        public SdkModelPlugins()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkModelPlugins"/> class, copying plugins from an existing instance.
        /// </summary>
        public SdkModelPlugins(SdkModelPlugins modelPlugins)
        {
            Playable.AddRange(modelPlugins.Playable);
            Downloadable.AddRange(modelPlugins.Downloadable);

            Album.AddRange(modelPlugins.Album);
            Artist.AddRange(modelPlugins.Artist);
            Playlist.AddRange(modelPlugins.Playlist);
            Track.AddRange(modelPlugins.Track);
            Image.AddRange(modelPlugins.Image);
            Lyrics.AddRange(modelPlugins.Lyrics);
            Url.AddRange(modelPlugins.Url);

            PlayableCollectionGroup.AddRange(modelPlugins.PlayableCollectionGroup);
            Library.AddRange(modelPlugins.Library);
            Discoverables.AddRange(modelPlugins.Discoverables);
            RecentlyPlayed.AddRange(modelPlugins.RecentlyPlayed);
            SearchHistory.AddRange(modelPlugins.SearchHistory);
            
            AlbumCollection.AddRange(modelPlugins.AlbumCollection);
            ArtistCollection.AddRange(modelPlugins.ArtistCollection);
            PlaylistCollection.AddRange(modelPlugins.PlaylistCollection);
            TrackCollection.AddRange(modelPlugins.TrackCollection);
            ImageCollection.AddRange(modelPlugins.ImageCollection);
            UrlCollection.AddRange(modelPlugins.UrlCollection);
            GenreCollection.AddRange(modelPlugins.GenreCollection);
        }

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IDownloadable"/>.
        /// </summary>
        public ChainedProxyBuilder<DownloadablePluginBase, IDownloadable> Downloadable { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IPlayable"/>.
        /// </summary>
        public ChainedProxyBuilder<PlayablePluginBase, IPlayable> Playable { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IAlbum"/>.
        /// </summary>
        public ChainedProxyBuilder<AlbumPluginBase, IAlbum> Album { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IArtist"/>.
        /// </summary>
        public ChainedProxyBuilder<ArtistPluginBase, IArtist> Artist { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IPlaylist"/>.
        /// </summary>
        public ChainedProxyBuilder<PlaylistPluginBase, IPlaylist> Playlist { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="ITrack"/>.
        /// </summary>
        public ChainedProxyBuilder<TrackPluginBase, ITrack> Track { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IImage"/>.
        /// </summary>
        public ChainedProxyBuilder<ImagePluginBase, IImage> Image { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="ILyrics"/>.
        /// </summary>
        public ChainedProxyBuilder<LyricsPluginBase, ILyrics> Lyrics { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IUrl"/>.
        /// </summary>
        public ChainedProxyBuilder<UrlPluginBase, IUrl> Url { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IPlayableCollectionGroup"/>.
        /// </summary>
        public ChainedProxyBuilder<PlayableCollectionGroupPluginBase, IPlayableCollectionGroup> PlayableCollectionGroup { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="ILibrary"/>.
        /// </summary>
        public ChainedProxyBuilder<LibraryPluginBase, ILibrary> Library { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IDiscoverables"/>.
        /// </summary>
        public ChainedProxyBuilder<DiscoverablesPluginBase, IDiscoverables> Discoverables { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IRecentlyPlayed"/>.
        /// </summary>
        public ChainedProxyBuilder<RecentlyPlayedPluginBase, IRecentlyPlayed> RecentlyPlayed { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="ISearchHistory"/>.
        /// </summary>
        public ChainedProxyBuilder<SearchHistoryPluginBase, ISearchHistory> SearchHistory { get; } = new();

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
