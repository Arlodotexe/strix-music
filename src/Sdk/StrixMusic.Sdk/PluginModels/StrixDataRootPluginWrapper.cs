// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixDataRootPluginWrapper"/> class.
        /// </summary>
        /// <param name="strixDataRoot">An existing instance to wrap around and provide plugins on top of.</param>
        /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
        public StrixDataRootPluginWrapper(IStrixDataRoot strixDataRoot, params SdkModelPlugin[] plugins)
        {
            foreach (var plugin in plugins)
                ActivePlugins.Import(plugin);

            ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

            Library = new LibraryPluginWrapper(strixDataRoot.Library, plugins);

            if (strixDataRoot.Search is not null)
                Search = new SearchPluginWrapper(strixDataRoot.Search, plugins);

            if (strixDataRoot.Pins is not null)
                Pins = new PlayableCollectionGroupPluginWrapper(strixDataRoot.Pins, plugins);

            if (strixDataRoot.Discoverables is not null)
                Discoverables = new DiscoverablesPluginWrapper(strixDataRoot.Discoverables, plugins);

            if (strixDataRoot.RecentlyPlayed is not null)
                RecentlyPlayed = new RecentlyPlayedPluginWrapper(strixDataRoot.RecentlyPlayed, plugins);

            _strixDataRoot = strixDataRoot;
            AttachEvents(_strixDataRoot);
        }

        private void AttachEvents(IStrixDataRoot strixDataRoot) => strixDataRoot.DevicesChanged += OnDevicesChanged;

        private void DetachEvents(IStrixDataRoot strixDataRoot) => strixDataRoot.DevicesChanged += OnDevicesChanged;

        private void OnDevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<IDevice>> addedItems, IReadOnlyList<CollectionChangedItem<IDevice>> removedItems) => DevicesChanged?.Invoke(this, addedItems, removedItems);

        /// <inheritdoc/>
        public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        /// <inheritdoc/>
        public MergedCollectionConfig MergeConfig => _strixDataRoot.MergeConfig;

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => _strixDataRoot.Devices;

        /// <inheritdoc/>
        public ILibrary Library { get; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public ISearch? Search { get; }

        /// <inheritdoc/>
        public IRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc/>
        public IDiscoverables? Discoverables { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ICore> Sources => _strixDataRoot.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => _strixDataRoot.SourceCores;

        /// <inheritdoc/>
        public bool IsInitialized => _strixDataRoot.IsInitialized;

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
    }
}
