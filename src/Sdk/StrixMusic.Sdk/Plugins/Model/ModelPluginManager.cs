using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// Manages plugins that modify behavior of interfaces or models.
    /// </summary>
    public class ModelPluginManager
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
        /// All plugins that provide overridden behavior for <see cref="IImageCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<ImageCollectionPluginBase, IImageCollection> ImageCollection { get; } = new();

        /// <summary>
        /// All plugins that provide overridden behavior for <see cref="IUrlCollection"/>.
        /// </summary>
        public ChainedProxyBuilder<UrlCollectionPluginBase, IUrlCollection> UrlCollection { get; } = new();
    }
}
