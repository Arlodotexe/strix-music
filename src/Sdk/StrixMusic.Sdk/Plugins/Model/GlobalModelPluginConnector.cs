using System;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// The global plugin connector applies common interface plugins to other plugins that use the interface.
    /// <para/>
    /// For example, if you create a plugin for <see cref="SdkModelPlugins.Downloadable"/>, that plugin is applied to
    /// <see cref="SdkModelPlugins.AlbumCollection"/>, <see cref="SdkModelPlugins.TrackCollection"/>, and every other
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
            sdkVer: new Version(0, 0, 0));

        /// <summary>
        /// Creates a new <see cref="SdkModelPlugins"/> from those provided in <paramref name="plugins"/> and
        /// inserts a new "Global Connector" plugin at the end of the chain, which aggregates
        /// all other plugins which implement an interface that the relevant plugin also implements.
        /// <para/>
        /// For example, if you create a plugin for <see cref="SdkModelPlugins.Downloadable"/>, that plugin is applied to
        /// <see cref="SdkModelPlugins.AlbumCollection"/>, <see cref="SdkModelPlugins.TrackCollection"/>, and every other
        /// plugin which implements <see cref="IDownloadable"/>.
        /// </summary>
        /// <param name="plugins">A plugin container which contains existing plugins to weave with the global plugin connector.</param>
        public static SdkModelPlugins Create(SdkModelPlugins plugins)
        {
            Guard.IsNotNull(plugins, nameof(plugins));

            // A global connector is only ever built when all user-added plugins have executed and none of them
            // have ignored the next plugin in the chain.
            // Once this happens, the connector collects all other plugins which implement an interface that the
            // relevant plugin also implements.
            // The resulting chain is constructed as a single plugin and neatly placed at the end of a new plugin
            // chain with all provided user-added plugins preceding it.
            var globalPlayablePluginBuilder = GenerateGlobalPlayablePluginBuilder(plugins);
            var globalAlbumCollectionPluginBuilder = GenerateGlobalAlbumCollectionPluginBuilder(plugins);
            var globalArtistCollectionPluginBuilder = GenerateGlobalArtistCollectionPluginBuilder(plugins);
            var globalTrackCollectionPluginBuilder = GenerateGlobalTrackCollectionPluginBuilder(plugins);
            var globalAlbumPluginBuilder = GenerateGlobalAlbumPluginBuilder(plugins);

            var pluginsWithGlobalConnectors = new SdkModelPlugins();

            // Add all existing plugins.
            pluginsWithGlobalConnectors.Playable.AddRange(plugins.Playable);
            pluginsWithGlobalConnectors.Downloadable.AddRange(plugins.Downloadable);
            pluginsWithGlobalConnectors.AlbumCollection.AddRange(plugins.AlbumCollection);
            pluginsWithGlobalConnectors.ArtistCollection.AddRange(plugins.ArtistCollection);
            pluginsWithGlobalConnectors.TrackCollection.AddRange(plugins.TrackCollection);
            pluginsWithGlobalConnectors.UrlCollection.AddRange(plugins.UrlCollection);
            pluginsWithGlobalConnectors.ImageCollection.AddRange(plugins.ImageCollection);
            pluginsWithGlobalConnectors.Album.AddRange(plugins.Album);

            // Add globally connected plugins.
            pluginsWithGlobalConnectors.Playable.Add(x => new PlayablePluginBase(PluginMetadata, globalPlayablePluginBuilder.Execute(x)));
            pluginsWithGlobalConnectors.AlbumCollection.Add(x => new AlbumCollectionPluginBase(PluginMetadata, globalAlbumCollectionPluginBuilder.Execute(x)));
            pluginsWithGlobalConnectors.ArtistCollection.Add(x => new ArtistCollectionPluginBase(PluginMetadata, globalArtistCollectionPluginBuilder.Execute(x)));
            pluginsWithGlobalConnectors.TrackCollection.Add(x => new TrackCollectionPluginBase(PluginMetadata, globalTrackCollectionPluginBuilder.Execute(x)));
            pluginsWithGlobalConnectors.Album.Add(x => new AlbumPluginBase(PluginMetadata, globalAlbumPluginBuilder.Execute(x)));

            return pluginsWithGlobalConnectors;
        }

        private static ChainedProxyBuilder<PlayablePluginBase, IPlayable> GenerateGlobalPlayablePluginBuilder(SdkModelPlugins plugins) => new()
        {
            x => new PlayablePluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            },
        };

        private static ChainedProxyBuilder<ArtistCollectionPluginBase, IArtistCollection> GenerateGlobalArtistCollectionPluginBuilder(SdkModelPlugins plugins) => new()
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

        private static ChainedProxyBuilder<AlbumCollectionPluginBase, IAlbumCollection> GenerateGlobalAlbumCollectionPluginBuilder(SdkModelPlugins plugins) => new()
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

        private static ChainedProxyBuilder<TrackCollectionPluginBase, ITrackCollection> GenerateGlobalTrackCollectionPluginBuilder(SdkModelPlugins plugins) => new()
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

        private static ChainedProxyBuilder<AlbumPluginBase, IAlbum> GenerateGlobalAlbumPluginBuilder(SdkModelPlugins plugins) => new()
        {
            // Downloadable members
            x => new AlbumPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
            },

            // UrlCollection members
            // GenreCollection members
            // ImageCollection members
            x => new AlbumPluginBase(PluginMetadata, x)
            {
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
    }
}
