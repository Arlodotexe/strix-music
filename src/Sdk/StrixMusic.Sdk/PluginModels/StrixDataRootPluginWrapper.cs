// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels
{
    /// <summary>
    /// Wraps an instance of <see cref="IStrixDataRoot"/> with the provided plugins.
    /// </summary>
    public class StrixDataRootPluginWrapper : IStrixDataRoot, IPluginWrapper
    {
        private readonly IStrixDataRoot _strixDataRoot;
        private readonly SdkModelPlugin[] _plugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixDataRootPluginWrapper"/> class.
        /// </summary>
        /// <param name="strixDataRoot">An existing instance to wrap around and provide plugins on top of.</param>
        /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
        public StrixDataRootPluginWrapper(IStrixDataRoot strixDataRoot, params SdkModelPlugin[] plugins)
        {
            foreach (var plugin in plugins)
                ActivePlugins.Import(plugin);

            ActivePlugins = GlobalModelPluginConnector.Create(strixDataRoot, ActivePlugins);

            _strixDataRoot = ActivePlugins.StrixDataRoot.Execute(strixDataRoot);

            Library = new LibraryPluginWrapper(_strixDataRoot.Library, strixDataRoot, plugins);

            if (_strixDataRoot.Search is not null)
                Search = new SearchPluginWrapper(_strixDataRoot.Search, strixDataRoot, plugins);

            if (_strixDataRoot.Pins is not null)
                Pins = new PlayableCollectionGroupPluginWrapper(_strixDataRoot.Pins, strixDataRoot, plugins);

            if (_strixDataRoot.Discoverables is not null)
                Discoverables = new DiscoverablesPluginWrapper(_strixDataRoot.Discoverables,strixDataRoot, plugins);

            if (_strixDataRoot.RecentlyPlayed is not null)
                RecentlyPlayed = new RecentlyPlayedPluginWrapper(_strixDataRoot.RecentlyPlayed,strixDataRoot, plugins);

            Root = this;
            _plugins = plugins;
            AttachEvents(_strixDataRoot);
        }

        private void AttachEvents(IStrixDataRoot strixDataRoot)
        {
            strixDataRoot.SourcesChanged += OnSourcesChanged;
            strixDataRoot.DevicesChanged += OnDevicesChanged;
            strixDataRoot.DevicesChanged += OnDevicesChanged;
            strixDataRoot.PinsChanged += OnPinsChanged;
            strixDataRoot.RecentlyPlayedChanged += OnRecentlyPlayedChanged;
            strixDataRoot.DiscoverablesChanged += OnDiscoverablesChanged;
            strixDataRoot.SearchChanged += OnSearchChanged;
        }

        private void DetachEvents(IStrixDataRoot strixDataRoot)
        {
            strixDataRoot.SourcesChanged -= OnSourcesChanged;
            strixDataRoot.DevicesChanged -= OnDevicesChanged;
            strixDataRoot.DevicesChanged -= OnDevicesChanged;
            strixDataRoot.PinsChanged -= OnPinsChanged;
            strixDataRoot.RecentlyPlayedChanged -= OnRecentlyPlayedChanged;
            strixDataRoot.DiscoverablesChanged -= OnDiscoverablesChanged;
            strixDataRoot.SearchChanged -= OnSearchChanged;
        }

        private void OnDiscoverablesChanged(object sender, IDiscoverables e)
        {
            Discoverables = new DiscoverablesPluginWrapper(e, Root, AppliedPlugins);
            DiscoverablesChanged?.Invoke(this, Discoverables);
        }

        private void OnRecentlyPlayedChanged(object sender, IRecentlyPlayed e)
        {
            RecentlyPlayed = new RecentlyPlayedPluginWrapper(e, Root, AppliedPlugins);
            RecentlyPlayedChanged?.Invoke(this, RecentlyPlayed);
        }

        private void OnPinsChanged(object sender, IPlayableCollectionGroup e)
        {
            Pins = new PlayableCollectionGroupPluginWrapper(e, Root, AppliedPlugins);
            PinsChanged?.Invoke(this, Pins);
        }

        private void OnSearchChanged(object sender, ISearch e)
        {
            Search = new SearchPluginWrapper(e, Root, AppliedPlugins);
            SearchChanged?.Invoke(this, Search);
        }
        
        private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

        private void OnDevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<IDevice>> addedItems, IReadOnlyList<CollectionChangedItem<IDevice>> removedItems) => DevicesChanged?.Invoke(this, addedItems, removedItems);

        /// <inheritdoc/>
        public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);
        
        /// <inheritdoc cref="IMerged.SourcesChanged"/>
        public event EventHandler? SourcesChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? PinsChanged;

        /// <inheritdoc/>
        public event EventHandler<IDiscoverables>? DiscoverablesChanged;

        /// <inheritdoc/>
        public event EventHandler<ISearch>? SearchChanged;

        /// <inheritdoc/>
        public event EventHandler<IRecentlyPlayed>? RecentlyPlayedChanged;

        /// <inheritdoc />
        public string Id => Root.Id;

        /// <inheritdoc/>
        public MergedCollectionConfig MergeConfig => _strixDataRoot.MergeConfig;

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => _strixDataRoot.Devices;

        /// <inheritdoc/>
        public ILibrary Library { get; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? Pins  { get; private set; }

        /// <inheritdoc/>
        public ISearch? Search  { get; private set; }

        /// <inheritdoc/>
        public IRecentlyPlayed? RecentlyPlayed { get; private set; }

        /// <inheritdoc/>
        public IDiscoverables? Discoverables { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<ICore> Sources => _strixDataRoot.Sources;

        /// <inheritdoc/>
        public bool IsInitialized => _strixDataRoot.IsInitialized;

        /// <summary>
        /// The plugins that were provided to the constructor. Can be applied to any other plugin-enabled wrapper.
        /// </summary>
        public SdkModelPlugin[] AppliedPlugins => _plugins;

        /// <inheritdoc/>
        public bool Equals(ICore other) => _strixDataRoot.Equals(other);

        /// <inheritdoc/>
        public Task InitAsync(CancellationToken cancellationToken = default) => _strixDataRoot.InitAsync(cancellationToken);

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            DetachEvents(_strixDataRoot);
            return _strixDataRoot.DisposeAsync();
        }

        /// <inheritdoc />
        public IStrixDataRoot Root { get; }
    }
}
