using StrixMusic.Sdk.Models;
using System;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// The global plugin connector applies common interface plugins to other plugins that use the interface.
    /// <para/>
    /// For example, if you create a plugin for <see cref="SdkModelPlugins.Downloadable"/>, that plugin is applied last to
    /// <see cref="SdkModelPlugins.AlbumCollection"/>, <see cref="SdkModelPlugins.TrackCollection"/>, and every other plugin which implements <see cref="IDownloadable"/>.
    /// </summary>        
    /// <remarks>
    /// In order to share behavior without blocking other plugins that might be added,
    /// the Global Plugin Connector must be last in the load order.
    /// </remarks>
    public static class GlobalModelPluginConnector
    {
        /// <summary>
        /// Gets a value that identifies a plugin as the global plugin connector.
        /// </summary>
        public static ModelPluginMetadata PluginMetadata { get; } = new ModelPluginMetadata(
            id: "Default",
            displayName: "Global Plugin Connector",
            description: "Required for common plugins to be applied globally. Should always be last in the load order.",
            sdkVer: new Version(0, 0, 0));

        /// <summary>
        /// Enables the global model plugin connector.
        /// </summary>
        /// <remarks>
        /// Since chain evaluation happens only when a model-wrapping plugin is requested,
        /// and since dependent plugin chains are evaluated inside a delegate action,
        /// this can be called lazily for every model before they're ever evaluated.
        /// <para />
        /// <param name="plugins">The plugin container to enable this functionality for.</param>
        public static void Enable(SdkModelPlugins plugins)
        {
            plugins.Playable.Add(x => new PlayablePluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Downloadable.Execute(x),
                InnerImageCollection = plugins.ImageCollection.Execute(x),
                InnerUrlCollection = plugins.UrlCollection.Execute(x),
            });

            plugins.TrackCollection.Add(x => new TrackCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            });

            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            });

            plugins.AlbumCollection.Add(x => new AlbumCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            });

            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBase(PluginMetadata, x)
            {
                InnerDownloadable = plugins.Playable.Execute(x),
                InnerPlayable = plugins.Playable.Execute(x),
                InnerImageCollection = plugins.Playable.Execute(x),
                InnerUrlCollection = plugins.Playable.Execute(x),
            });
        }
    }
}
