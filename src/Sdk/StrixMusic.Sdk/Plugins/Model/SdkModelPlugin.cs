using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// A model plugin is one or more implementations of <see cref="IModelPlugin"/>
    /// that modifies the behavior of an interface implementation
    /// by wrapping around an existing instance and selectively overriding members.
    /// </summary>
    /// <remarks>
    ///         Contains a chainable plugin builder for every relevant model interface used
    ///         in the Strix Music SDK. 
    /// <para/> When the chain is built, the first added Plugin is returned, with the next Plugin provided during construction for 
    ///         proxying, which also had the next item passed into it during construction, and so on.
    /// <para/> When accessing a member, a plugin may retrieve data from the next plugin (or, if none, the actual implementation), and
    ///         transform or replace it as needed.
    ///         A plugin may ignore the inner implementation entirely and supply new data.
    ///         Or, a plugin might not override that member and simply relay data from the actual implementation.
    /// </remarks>
    public class SdkModelPlugin : IModelPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdkModelPlugin"/> class.
        /// </summary>
        /// <param name="metadata">Metadata that identifies this plugin.</param>
        public SdkModelPlugin(ModelPluginMetadata metadata)
        {
            Metadata = metadata;
        }

        /// <inheritdoc/>
        public ModelPluginMetadata Metadata { get; }

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IStrixDataRoot"/>.
        /// </summary>
        public ChainedProxyBuilder<StrixDataRootPluginBase, IStrixDataRoot> StrixDataRoot { get; } = new();

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
        /// All plugins that provide overridden behavior for <see cref="ISearchResults"/>.
        /// </summary>
        public ChainedProxyBuilder<SearchResultsPluginBase, ISearchResults> SearchResults { get; } = new();

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

        /// <summary>
        /// Copies all plugins from the provided <paramref name="modelPlugin"/> into this instance.
        /// </summary>
        /// <param name="modelPlugin">The plugin collection to import into this instance.</param>
        public void Import(SdkModelPlugin modelPlugin)
        {
            var currentSdkVersion = typeof(SdkModelPlugin).Assembly.GetName().Version;

            if (modelPlugin.Metadata.SdkVersion.Major != currentSdkVersion.Major)
            {
                ThrowHelper.ThrowArgumentException($"The imported plugin [{modelPlugin.Metadata.Id}] \"{modelPlugin.Metadata.DisplayName}\" was built against SDK " +
                                                   $"version {modelPlugin.Metadata.SdkVersion} and is incompatible with the running version {currentSdkVersion}. " +
                                                   $"Please ask the author to update the plugin to the latest release of the Strix Music SDK.");
            }

            StrixDataRoot.AddRange(modelPlugin.StrixDataRoot);

            Playable.AddRange(modelPlugin.Playable);
            Downloadable.AddRange(modelPlugin.Downloadable);

            Album.AddRange(modelPlugin.Album);
            Artist.AddRange(modelPlugin.Artist);
            Playlist.AddRange(modelPlugin.Playlist);
            Track.AddRange(modelPlugin.Track);
            Image.AddRange(modelPlugin.Image);
            Lyrics.AddRange(modelPlugin.Lyrics);
            Url.AddRange(modelPlugin.Url);

            PlayableCollectionGroup.AddRange(modelPlugin.PlayableCollectionGroup);
            Library.AddRange(modelPlugin.Library);
            Discoverables.AddRange(modelPlugin.Discoverables);
            RecentlyPlayed.AddRange(modelPlugin.RecentlyPlayed);
            SearchHistory.AddRange(modelPlugin.SearchHistory);
            SearchResults.AddRange(modelPlugin.SearchResults);

            AlbumCollection.AddRange(modelPlugin.AlbumCollection);
            ArtistCollection.AddRange(modelPlugin.ArtistCollection);
            PlaylistCollection.AddRange(modelPlugin.PlaylistCollection);
            TrackCollection.AddRange(modelPlugin.TrackCollection);
            ImageCollection.AddRange(modelPlugin.ImageCollection);
            UrlCollection.AddRange(modelPlugin.UrlCollection);
            GenreCollection.AddRange(modelPlugin.GenreCollection);
        }
    }
}
