using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;
using System;

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
    /// <para/> For each interface with a corresponding model plugin, the plugin is applied to every single instance of that interface that
    ///         passes through the plugin system.
    /// <para/> e.g., if you create a plugin for <see cref="SdkModelPlugins.Playable"/>, that plugin is applied to
    ///         every single <see cref="IPlayable"/> instantiated by the SDK or returned from / added to a collection.
    /// 
    /// </summary>
    public class SdkModelPlugins
    {
        /// <summary>
        /// Creates a new instance of <see cref="SdkModelPlugins"/>.
        /// </summary>
        public SdkModelPlugins()
        {
            var pluginMetadata = new ModelPluginMetadata(
                id: "Default",
                displayName: "Global Plugin Connector",
                description: "Required for common plugins to be applied globally. Should always be last in the load order.",
                sdkVer: new Version(0, 0, 0));

            // In order to share behavior without blocking custom plugins that might be added,
            // The Global Plugin Connector must be last in the load order.

            // Since chain evaluation happens only when a model-wrapping plugin is requested,
            // and since dependent plugin chains are evaluated inside a delegate action,
            // we can do this lazily for every model right here in the ctor.

            // If any of these contain plugins, the provided behavior is applied to
            // all other plugins which implement the returned interface
            // before using the default implementation.
            Playable.Add(x => new PlayablePluginBase(pluginMetadata, x)
            {
                InnerDownloadable = Downloadable.Execute(x),
                InnerImageCollection = ImageCollection.Execute(x),
                InnerUrlCollection = UrlCollection.Execute(x),
            });

            TrackCollection.Add(x => new TrackCollectionPluginBase(pluginMetadata, x)
            {
                InnerDownloadable = Playable.Execute(x),
                InnerPlayable = Playable.Execute(x),
                InnerImageCollection = Playable.Execute(x),
                InnerUrlCollection = Playable.Execute(x),
            });

            ArtistCollection.Add(x => new ArtistCollectionPluginBase(pluginMetadata, x)
            {
                InnerDownloadable = Playable.Execute(x),
                InnerPlayable = Playable.Execute(x),
                InnerImageCollection = Playable.Execute(x),
                InnerUrlCollection = Playable.Execute(x),
            });

            AlbumCollection.Add(x => new AlbumCollectionPluginBase(pluginMetadata, x)
            {
                InnerDownloadable = Playable.Execute(x),
                InnerPlayable = Playable.Execute(x),
                InnerImageCollection = Playable.Execute(x),
                InnerUrlCollection = Playable.Execute(x),
            });

            PlaylistCollection.Add(x => new PlaylistCollectionPluginBase(pluginMetadata, x)
            {
                InnerDownloadable = Playable.Execute(x),
                InnerPlayable = Playable.Execute(x),
                InnerImageCollection = Playable.Execute(x),
                InnerUrlCollection = Playable.Execute(x),
            });
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
        /// All plugins that provide overridden behavior for <see cref="IImage"/>.
        /// </summary>
        public ChainedProxyBuilder<ImagePluginBase, IImage> Image { get; } = new();

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
