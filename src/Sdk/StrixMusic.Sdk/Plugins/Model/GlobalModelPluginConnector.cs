// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

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
        /// <param name="root">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
        /// <param name="plugins">A plugin container which contains existing plugins to weave with the global plugin connector.</param>
        /// <returns>A new instance of <see cref="SdkModelPlugin"/> with the provided plugins applied to all common types.</returns>
        public static SdkModelPlugin Create(IStrixDataRoot root, SdkModelPlugin plugins)
        {
            // Create plugin connectors inside of a buildable plugin.
            // Note that only plugins which derived other plugin-enabled interfaces need setup here.
            // The derived interface plugins are applied inside each method.
            var playableBuilder = GenerateGlobalPlayablePluginBuilder(plugins, root);
            var playableCollectionGroupBuilder = GenerateGlobalPlayableCollectionGroupPluginBuilder(plugins, root);
            var libraryBuilder = GenerateGlobalLibraryPluginBuilder(plugins, root);
            var discoverablesBuilder = GenerateGlobalDiscoverablesPluginBuilder(plugins, root);
            var recentlyPlayedBuilder = GenerateGlobalRecentlyPlayedPluginBuilder(plugins, root);
            var searchHistoryBuilder = GenerateGlobalSearchHistoryPluginBuilder(plugins, root);
            var searchResultsBuilder = GenerateGlobalSearchResultsPluginBuilder(plugins, root);
            
            var albumCollectionBuilder = GenerateGlobalAlbumCollectionPluginBuilder(plugins, root);
            var artistCollectionBuilder = GenerateGlobalArtistCollectionPluginBuilder(plugins, root);
            var trackCollectionBuilder = GenerateGlobalTrackCollectionPluginBuilder(plugins, root);
            
            var albumBuilder = GenerateGlobalAlbumPluginBuilder(plugins, root);
            var artistBuilder = GenerateGlobalArtistPluginBuilder(plugins, root);
            var playlistBuilder = GenerateGlobalPlaylistPluginBuilder(plugins, root);
            var trackBuilder = GenerateGlobalTrackPluginBuilder(plugins, root);

            // Clone plugin container & add global connectors.
            // Global connectors must be added to a new instance, otherwise some global connectors could
            // treat other global connectors as a user-added plugin and attempt to ingest them, causing
            // undesired behavior.
            var pluginsWithGlobalConnectors = new SdkModelPlugin(PluginMetadata);
            pluginsWithGlobalConnectors.Import(plugins);
            
            pluginsWithGlobalConnectors.Playable.Add(x => new PlayablePluginBase(PluginMetadata, playableBuilder.Execute(x), root));
            
            pluginsWithGlobalConnectors.PlayableCollectionGroup.Add(x => new PlayableCollectionGroupPluginBase(PluginMetadata, playableCollectionGroupBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.Library.Add(x => new LibraryPluginBase(PluginMetadata, libraryBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.Discoverables.Add(x => new DiscoverablesPluginBase(PluginMetadata, discoverablesBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.RecentlyPlayed.Add(x => new RecentlyPlayedPluginBase(PluginMetadata, recentlyPlayedBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.SearchHistory.Add(x => new SearchHistoryPluginBase(PluginMetadata, searchHistoryBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.SearchResults.Add(x => new SearchResultsPluginBase(PluginMetadata, searchResultsBuilder.Execute(x), root));
            
            pluginsWithGlobalConnectors.AlbumCollection.Add(x => new AlbumCollectionPluginBase(PluginMetadata, albumCollectionBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.ArtistCollection.Add(x => new ArtistCollectionPluginBase(PluginMetadata, artistCollectionBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.TrackCollection.Add(x => new TrackCollectionPluginBase(PluginMetadata, trackCollectionBuilder.Execute(x), root));
            
            pluginsWithGlobalConnectors.Album.Add(x => new AlbumPluginBase(PluginMetadata, albumBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.Artist.Add(x => new ArtistPluginBase(PluginMetadata, artistBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.Playlist.Add(x => new PlaylistPluginBase(PluginMetadata, playlistBuilder.Execute(x), root));
            pluginsWithGlobalConnectors.Track.Add(x => new TrackPluginBase(PluginMetadata, trackBuilder.Execute(x), root));

            return pluginsWithGlobalConnectors;
        }

        private static ChainedProxyBuilder<PlayablePluginBase, IPlayable> GenerateGlobalPlayablePluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            x => new PlayablePluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<PlayableCollectionGroupPluginBase, IPlayableCollectionGroup> GenerateGlobalPlayableCollectionGroupPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new PlayableCollectionGroupPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<LibraryPluginBase, ILibrary> GenerateGlobalLibraryPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new LibraryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new LibraryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new LibraryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new LibraryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new LibraryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new LibraryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<DiscoverablesPluginBase, IDiscoverables> GenerateGlobalDiscoverablesPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new DiscoverablesPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new DiscoverablesPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<RecentlyPlayedPluginBase, IRecentlyPlayed> GenerateGlobalRecentlyPlayedPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new RecentlyPlayedPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<SearchHistoryPluginBase, ISearchHistory> GenerateGlobalSearchHistoryPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new SearchHistoryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new SearchHistoryPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<SearchResultsPluginBase, ISearchResults> GenerateGlobalSearchResultsPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new SearchResultsPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // ArtistCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // PlaylistCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.PlaylistCollection.Execute(x),
                InnerPlayable = plugins.PlaylistCollection.Execute(x),
                InnerImageCollection = plugins.PlaylistCollection.Execute(x),
                InnerUrlCollection = plugins.PlaylistCollection.Execute(x),
                InnerPlaylistCollection = plugins.PlaylistCollection.Execute(x),
            },

            // TrackCollection members
            x => new SearchResultsPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<ArtistCollectionPluginBase, IArtistCollection> GenerateGlobalArtistCollectionPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            x => new ArtistCollectionPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new ArtistCollectionPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },
        };

        private static ChainedProxyBuilder<AlbumCollectionPluginBase, IAlbumCollection> GenerateGlobalAlbumCollectionPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            x => new AlbumCollectionPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new AlbumCollectionPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },
        };

        private static ChainedProxyBuilder<TrackCollectionPluginBase, ITrackCollection> GenerateGlobalTrackCollectionPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            x => new TrackCollectionPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new TrackCollectionPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },
        };

        private static ChainedProxyBuilder<AlbumPluginBase, IAlbum> GenerateGlobalAlbumPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new AlbumPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerGenreCollection = plugins.GenreCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new AlbumPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // ArtistCollection members
            x => new AlbumPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.ArtistCollection.Execute(x),
                InnerPlayable = plugins.ArtistCollection.Execute(x),
                InnerImageCollection = plugins.ArtistCollection.Execute(x),
                InnerUrlCollection = plugins.ArtistCollection.Execute(x),
                InnerArtistCollection = plugins.ArtistCollection.Execute(x),
            },

            // TrackCollection members
            x => new AlbumPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            }
        };

        private static ChainedProxyBuilder<ArtistPluginBase, IArtist> GenerateGlobalArtistPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new ArtistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerGenreCollection = plugins.GenreCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new ArtistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // AlbumCollection members
            x => new ArtistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.AlbumCollection.Execute(x),
                InnerPlayable = plugins.AlbumCollection.Execute(x),
                InnerImageCollection = plugins.AlbumCollection.Execute(x),
                InnerUrlCollection = plugins.AlbumCollection.Execute(x),
                InnerAlbumCollection = plugins.AlbumCollection.Execute(x),
            },

            // TrackCollection members
            x => new ArtistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            }
        };

        private static ChainedProxyBuilder<PlaylistPluginBase, IPlaylist> GenerateGlobalPlaylistPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // ImageCollection members
            x => new PlaylistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new PlaylistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // TrackCollection members
            x => new PlaylistPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.TrackCollection.Execute(x),
                InnerPlayable = plugins.TrackCollection.Execute(x),
                InnerImageCollection = plugins.TrackCollection.Execute(x),
                InnerUrlCollection = plugins.TrackCollection.Execute(x),
                InnerTrackCollection = plugins.TrackCollection.Execute(x),
            }
        };

        private static ChainedProxyBuilder<TrackPluginBase, ITrack> GenerateGlobalTrackPluginBuilder(SdkModelPlugin plugins, IStrixDataRoot root) => new()
        {
            // Downloadable members
            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new TrackPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerGenreCollection = plugins.GenreCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },

            // Playable members
            x => new TrackPluginBase(PluginMetadata, x, root)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            },

            // ArtistCollection members
            x => new TrackPluginBase(PluginMetadata, x, root)
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
