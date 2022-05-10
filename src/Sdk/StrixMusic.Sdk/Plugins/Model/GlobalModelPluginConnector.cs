using System;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// The global plugin connector applies common interface plugins to other plugins that derive them.
    /// <para/>
    /// For example, if you create a plugin for <see cref="SdkModelPlugin.Downloadable"/>, that plugin is applied to
    /// <see cref="SdkModelPlugin.AlbumCollection"/>, <see cref="SdkModelPlugin.TrackCollection"/>, and every other
    /// plugin which implements <see cref="IDownloadable"/>.
    /// </summary>
    public static class GlobalModelPluginConnector
    {
        /// <summary>
        /// Gets a value that identifies a plugin as the global plugin connector.
        /// </summary>
        public static ModelPluginMetadata PluginMetadata { get; } = new(
            id: "GlobalModelPluginConnector",
            displayName: "Global Model Plugin Connector",
            description: "Required for common plugins to be applied globally. Should always be last in the load order.",
            new Version(0, 0, 0));

        /// <summary>
        /// Creates a new instance of <see cref="SdkModelPlugin"/> from the provided <paramref name="plugins"/> and
        /// inserts a new "Global Model Plugin Connector" plugin at the end of each plugin chain, which applies common
        /// interface plugins to other plugins that derive them.
        /// <para/>
        /// <example>
        /// For example, if you create a plugin for <see cref="SdkModelPlugin.Downloadable"/>, that plugin is applied to
        /// <see cref="SdkModelPlugin.AlbumCollection"/>, <see cref="SdkModelPlugin.TrackCollection"/>, and every other
        /// plugin which implements <see cref="IDownloadable"/>.
        /// </example>
        /// </summary>
        /// <remarks>
        /// <para/> A global connector is only ever built if all user-added plugins have executed and none of them
        ///         have blocked/ignored the next plugin in their chain.
        /// <para/> Once this happens, the connector executes and collects all plugins which derive other
        ///         plugin-enabled interfaces, chaining them so each has a chance to execute.
        /// <para/> The resulting chain is constructed as a single plugin and neatly placed at the end of a new plugin
        ///         chain with all provided user-added plugins preceding it.
        /// </remarks>
        /// <param name="plugins">A plugin container which contains existing plugins to weave with the global plugin connector.</param>
        /// <returns>A new instance of <see cref="SdkModelPlugin"/> with the provided plugins applied to all common types.</returns>
        public static SdkModelPlugin Create(SdkModelPlugin plugins)
        {
            // Create plugin connectors inside of a buildable plugin.
            // Note that only plugins which derived other plugin-enabled interfaces need setup here.
            // The derived interface plugins are applied inside each method.
            var playableBuilder = GenerateGlobalPlayablePluginBuilder(plugins);
            var playableCollectionGroupBuilder = GenerateGlobalPlayableCollectionGroupPluginBuilder(plugins);
            var libraryBuilder = GenerateGlobalLibraryPluginBuilder(plugins);
            var discoverablesBuilder = GenerateGlobalDiscoverablesPluginBuilder(plugins);
            var recentlyPlayedBuilder = GenerateGlobalRecentlyPlayedPluginBuilder(plugins);
            var searchHistoryBuilder = GenerateGlobalSearchHistoryPluginBuilder(plugins);
            var searchResultsBuilder = GenerateGlobalSearchResultsPluginBuilder(plugins);
            
            var albumCollectionBuilder = GenerateGlobalAlbumCollectionPluginBuilder(plugins);
            var artistCollectionBuilder = GenerateGlobalArtistCollectionPluginBuilder(plugins);
            var trackCollectionBuilder = GenerateGlobalTrackCollectionPluginBuilder(plugins);
            
            var albumBuilder = GenerateGlobalAlbumPluginBuilder(plugins);
            var artistBuilder = GenerateGlobalArtistPluginBuilder(plugins);
            var playlistBuilder = GenerateGlobalPlaylistPluginBuilder(plugins);
            var trackBuilder = GenerateGlobalTrackPluginBuilder(plugins);

            // Clone plugin container & add global connectors.
            // Global connectors must be added to a new instance, otherwise some global connectors could
            // treat other global connectors as a user-added plugin and attempt to ingest them, causing
            // undesired behavior.
            var pluginsWithGlobalConnectors = new SdkModelPlugin(PluginMetadata);
            pluginsWithGlobalConnectors.Import(plugins);
            
            pluginsWithGlobalConnectors.Playable.Add(x => new PlayablePluginBase(PluginMetadata, playableBuilder.Execute(x)));
            
            pluginsWithGlobalConnectors.PlayableCollectionGroup.Add(x => new PlayableCollectionGroupPluginBase(PluginMetadata, playableCollectionGroupBuilder.Execute(x)));
            pluginsWithGlobalConnectors.Library.Add(x => new LibraryPluginBase(PluginMetadata, libraryBuilder.Execute(x)));
            pluginsWithGlobalConnectors.Discoverables.Add(x => new DiscoverablesPluginBase(PluginMetadata, discoverablesBuilder.Execute(x)));
            pluginsWithGlobalConnectors.RecentlyPlayed.Add(x => new RecentlyPlayedPluginBase(PluginMetadata, recentlyPlayedBuilder.Execute(x)));
            pluginsWithGlobalConnectors.SearchHistory.Add(x => new SearchHistoryPluginBase(PluginMetadata, searchHistoryBuilder.Execute(x)));
            pluginsWithGlobalConnectors.SearchResults.Add(x => new SearchResultsPluginBase(PluginMetadata, searchResultsBuilder.Execute(x)));
            
            pluginsWithGlobalConnectors.AlbumCollection.Add(x => new AlbumCollectionPluginBase(PluginMetadata, albumCollectionBuilder.Execute(x)));
            pluginsWithGlobalConnectors.ArtistCollection.Add(x => new ArtistCollectionPluginBase(PluginMetadata, artistCollectionBuilder.Execute(x)));
            pluginsWithGlobalConnectors.TrackCollection.Add(x => new TrackCollectionPluginBase(PluginMetadata, trackCollectionBuilder.Execute(x)));
            
            pluginsWithGlobalConnectors.Album.Add(x => new AlbumPluginBase(PluginMetadata, albumBuilder.Execute(x)));
            pluginsWithGlobalConnectors.Artist.Add(x => new ArtistPluginBase(PluginMetadata, artistBuilder.Execute(x)));
            pluginsWithGlobalConnectors.Playlist.Add(x => new PlaylistPluginBase(PluginMetadata, playlistBuilder.Execute(x)));
            pluginsWithGlobalConnectors.Track.Add(x => new TrackPluginBase(PluginMetadata, trackBuilder.Execute(x)));

            return pluginsWithGlobalConnectors;
        }

        private static ChainedProxyBuilder<PlayablePluginBase, IPlayable> GenerateGlobalPlayablePluginBuilder(SdkModelPlugin plugins) => new()
        {
            x => new PlayablePluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<PlayableCollectionGroupPluginBase, IPlayableCollectionGroup> GenerateGlobalPlayableCollectionGroupPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<LibraryPluginBase, ILibrary> GenerateGlobalLibraryPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new LibraryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new LibraryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new LibraryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new LibraryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new LibraryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new LibraryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<DiscoverablesPluginBase, IDiscoverables> GenerateGlobalDiscoverablesPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new DiscoverablesPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<RecentlyPlayedPluginBase, IRecentlyPlayed> GenerateGlobalRecentlyPlayedPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<SearchHistoryPluginBase, ISearchHistory> GenerateGlobalSearchHistoryPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new SearchHistoryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<SearchResultsPluginBase, ISearchResults> GenerateGlobalSearchResultsPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new SearchResultsPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<ArtistCollectionPluginBase, IArtistCollection> GenerateGlobalArtistCollectionPluginBuilder(SdkModelPlugin plugins) => new()
        {
            x => new ArtistCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new ArtistCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },
        };

        private static ChainedProxyBuilder<AlbumCollectionPluginBase, IAlbumCollection> GenerateGlobalAlbumCollectionPluginBuilder(SdkModelPlugin plugins) => new()
        {
            x => new AlbumCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new AlbumCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },
        };

        private static ChainedProxyBuilder<TrackCollectionPluginBase, ITrackCollection> GenerateGlobalTrackCollectionPluginBuilder(SdkModelPlugin plugins) => new()
        {
            x => new TrackCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new TrackCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },
        };

        private static ChainedProxyBuilder<AlbumPluginBase, IAlbum> GenerateGlobalAlbumPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new AlbumPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerGenreCollection = plugins.GenreCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new AlbumPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // ArtistCollection members
            x => new AlbumPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // TrackCollection members
            x => new AlbumPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            }
        };

        private static ChainedProxyBuilder<ArtistPluginBase, IArtist> GenerateGlobalArtistPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new ArtistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerGenreCollection = plugins.GenreCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new ArtistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new ArtistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // TrackCollection members
            x => new ArtistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            }
        };

        private static ChainedProxyBuilder<PlaylistPluginBase, IPlaylist> GenerateGlobalPlaylistPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new PlaylistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new PlaylistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // TrackCollection members
            x => new PlaylistPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            }
        };

        private static ChainedProxyBuilder<TrackPluginBase, ITrack> GenerateGlobalTrackPluginBuilder(SdkModelPlugin plugins) => new()
        {
            // Downloadable members
            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new TrackPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerGenreCollection = plugins.GenreCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new TrackPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // ArtistCollection members
            x => new TrackPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },
        };
    }
}
