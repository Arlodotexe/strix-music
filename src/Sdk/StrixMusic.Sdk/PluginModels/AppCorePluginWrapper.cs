// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
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
    /// Wraps an instance of <see cref="IAppCore"/> with the provided plugins.
    /// </summary>
    public class AppCorePluginWrapper : IAppCore, IPluginWrapper
    {
        private readonly IAppCore _appCore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCorePluginWrapper"/> class.
        /// </summary>
        /// <param name="appCore">An existing instance to wrap around and provide plugins on top of.</param>
        /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
        public AppCorePluginWrapper(IAppCore appCore, params SdkModelPlugin[] plugins)
        {
            foreach (var plugin in plugins)
                ActivePlugins.Import(plugin);

            ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

            Library = new LibraryPluginWrapper(appCore.Library);

            if (appCore.Search is not null)
                Search = new SearchPluginWrapper(appCore.Search, plugins);

            if (appCore.Pins is not null)
                Pins = new PlayableCollectionGroupPluginWrapper(appCore.Pins, plugins);

            if (appCore.Discoverables is not null)
                Discoverables = new DiscoverablesPluginWrapper(appCore.Discoverables, plugins);

            if (appCore.RecentlyPlayed is not null)
                RecentlyPlayed = new RecentlyPlayedPluginWrapper(appCore.RecentlyPlayed, plugins);

            _appCore = appCore;
            AttachEvents(_appCore);
        }

        private void AttachEvents(IAppCore appCore) => appCore.DevicesChanged += OnDevicesChanged;

        private void DetachEvents(IAppCore appCore) => appCore.DevicesChanged += OnDevicesChanged;

        private void OnDevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<IDevice>> addedItems, IReadOnlyList<CollectionChangedItem<IDevice>> removedItems) => DevicesChanged?.Invoke(this, addedItems, removedItems);

        /// <inheritdoc/>
        public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        /// <inheritdoc/>
        public MergedCollectionConfig MergeConfig => _appCore.MergeConfig;

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => _appCore.Devices;

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
        public IReadOnlyList<ICore> Sources => _appCore.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => _appCore.SourceCores;

        /// <inheritdoc/>
        public bool IsInitialized => _appCore.IsInitialized;

        /// <inheritdoc/>
        public bool Equals(ICore other) => _appCore.Equals(other);

        /// <inheritdoc/>
        public Task InitAsync(CancellationToken cancellationToken = default) => _appCore.InitAsync(cancellationToken);

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            DetachEvents(_appCore);
            return _appCore.DisposeAsync();
        }
    }
}
