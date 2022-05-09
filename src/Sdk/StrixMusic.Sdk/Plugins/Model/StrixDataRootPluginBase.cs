using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IStrixDataRoot"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class StrixDataRootPluginBase : IModelPlugin, IStrixDataRoot, IDelegatable<IStrixDataRoot>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ImagePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        internal protected StrixDataRootPluginBase(ModelPluginMetadata registration, IStrixDataRoot inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public virtual IStrixDataRoot Inner { get; }

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<IPlayableCollectionGroup>? PinsChanged
        {
            add => Inner.PinsChanged += value;
            remove => Inner.PinsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<ISearch>? SearchChanged
        {
            add => Inner.SearchChanged += value;
            remove => Inner.SearchChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<IRecentlyPlayed>? RecentlyPlayedChanged
        {
            add => Inner.RecentlyPlayedChanged += value;
            remove => Inner.RecentlyPlayedChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<IDiscoverables>? DiscoverablesChanged
        {
            add => Inner.DiscoverablesChanged += value;
            remove => Inner.DiscoverablesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IDevice>? DevicesChanged
        {
            add => Inner.DevicesChanged += value;
            remove => Inner.DevicesChanged -= value;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ICore> Sources => Inner.Sources;

        /// <inheritdoc/>
        public virtual MergedCollectionConfig MergeConfig => Inner.MergeConfig;

        /// <inheritdoc/>
        public virtual IReadOnlyList<IDevice> Devices => Inner.Devices;

        /// <inheritdoc/>
        public virtual ILibrary Library => Inner.Library;

        /// <inheritdoc/>
        public virtual IPlayableCollectionGroup? Pins => Inner.Pins;

        /// <inheritdoc/>
        public virtual ISearch? Search => Inner.Search;

        /// <inheritdoc/>
        public virtual IRecentlyPlayed? RecentlyPlayed => Inner.RecentlyPlayed;

        /// <inheritdoc/>
        public virtual IDiscoverables? Discoverables => Inner.Discoverables;

        /// <inheritdoc/>
        public virtual bool IsInitialized => Inner.IsInitialized;

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImage other) => Inner.Equals(other);

        /// <inheritdoc/>
        public bool Equals(ICore other) => Inner.Equals(other);

        /// <inheritdoc/>
        public Task InitAsync(CancellationToken cancellationToken = default) => Inner.InitAsync(cancellationToken);
    }
}
