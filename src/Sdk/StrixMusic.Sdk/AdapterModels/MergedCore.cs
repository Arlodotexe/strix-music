// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Aggregates many <see cref="ICore"/> instances into one.
    /// </summary>
    public sealed class MergedCore : IAppCore
    {
        private readonly List<ICore> _sources;
        private readonly List<IDevice> _devices;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCore"/>.
        /// </summary>
        public MergedCore(IEnumerable<ICore> cores, MergedCollectionConfig config)
        {
            _sources = cores.ToList();
            MergeConfig = config;
            SourceCores = _sources.Select(x => x.SourceCore).ToList();

            Library = new MergedLibrary(_sources.Select(x => x.Library), config);

            if (_sources.All(x => x.Discoverables == null))
                Discoverables = new MergedDiscoverables(_sources.Select(x => x.Discoverables).PruneNull(), config);

            if (_sources.All(x => x.Pins == null))
                Pins = new MergedPlayableCollectionGroup(_sources.Select(x => x.Pins).PruneNull(), config);

            if (_sources.All(x => x.RecentlyPlayed == null))
                RecentlyPlayed = new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed).PruneNull(), config);

            _devices = new List<IDevice>(_sources.SelectMany(x => x.Devices, (_, device) => new DeviceAdapter(device)));

            AttachEvents();
        }

        private void AttachEvents()
        {
            foreach (var core in _sources)
                core.DevicesChanged += Core_DevicesChanged;
        }

        private void DetachEvents()
        {
            foreach (var core in _sources)
                core.DevicesChanged -= Core_DevicesChanged;
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems)
        {
            var itemsToAdd = addedItems.Select(x => new CollectionChangedItem<IDevice>(new DeviceAdapter(x.Data), x.Index)).ToList();
            var itemsToRemove = removedItems.Select(x => new CollectionChangedItem<IDevice>(new DeviceAdapter(x.Data), x.Index)).ToList();

            foreach (var item in itemsToRemove)
                _devices.RemoveAt(item.Index);

            foreach (var item in itemsToAdd)
                _devices.InsertOrAdd(item.Index, item.Data);

            DevicesChanged?.Invoke(this, itemsToAdd, itemsToRemove);
        }

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICore> Sources => _sources;

        /// <inheritdoc />
        public MergedCollectionConfig MergeConfig { get; }

        /// <inheritdoc />
        public IReadOnlyList<IDevice> Devices => _devices;

        /// <inheritdoc />
        public ILibrary Library { get; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? Pins { get; }

        /// <inheritdoc />
        public ISearch? Search { get; }

        /// <inheritdoc />
        public IRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc />
        public IDiscoverables? Discoverables { get; }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        /// <inheritdoc />
        /// <remarks>
        /// Cores can be merged, but are never matched conditionally.
        /// </remarks>
        public bool Equals(ICore other) => false;

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents();

            await Devices.InParallel(x => x.DisposeAsync().AsTask());
            await Library.DisposeAsync();

            if (Pins != null)
                await Pins.DisposeAsync();

            if (Search != null)
                await Search.DisposeAsync();

            if (RecentlyPlayed != null)
                await RecentlyPlayed.DisposeAsync();

            if (Discoverables != null)
                await Discoverables.DisposeAsync();

            await _sources.InParallel(x => x.DisposeAsync().AsTask());
        }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
    }
}
