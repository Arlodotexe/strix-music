// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Aggregates many <see cref="ICore"/> instances into one instance.
    /// All data returned and emitted by this instance will contained merged data from the provided sources.
    /// </summary>
    public sealed class MergedCore : IStrixDataRoot, IMergedMutable<ICore>
    {
        private readonly List<ICore> _sources;
        private readonly List<IDevice> _devices;

        private readonly MergedDiscoverables _discoverables;
        private readonly MergedPlayableCollectionGroup _pins;
        private readonly MergedRecentlyPlayed _recentlyPlayed;
        private readonly MergedLibrary _library;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCore"/>.
        /// </summary>
        public MergedCore(IEnumerable<ICore> cores, MergedCollectionConfig config)
        {
            _sources = cores.ToList();
            SourceCores = _sources.Select(x => x.SourceCore).ToList();
            MergeConfig = config;

            _library = new MergedLibrary(_sources.Select(x => x.Library), config);

            // These items have no notification support for changing after construction,
            // so they need to be initialized every time in case a new source is added with a non-null value.
            _discoverables = new MergedDiscoverables(_sources.Select(x => x.Discoverables).PruneNull(), config);
            _pins = new MergedPlayableCollectionGroup(_sources.Select(x => x.Pins).PruneNull(), config);
            _recentlyPlayed = new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed).PruneNull(), config);

            _devices = new List<IDevice>(_sources.SelectMany(x => x.Devices, (_, device) => new DeviceAdapter(device)));

            foreach (var source in _sources)
                AttachEvents(source);
        }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default) => throw new System.NotImplementedException();

        private void AttachEvents(ICore core)
        {
            core.DevicesChanged += Core_DevicesChanged;
        }

        private void DetachEvents(ICore core)
        {
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
        public ILibrary Library => _library;

        /// <inheritdoc />
        public IPlayableCollectionGroup? Pins => _pins;

        /// <inheritdoc />
        public ISearch? Search { get; }

        /// <inheritdoc />
        public IRecentlyPlayed? RecentlyPlayed => _recentlyPlayed;

        /// <inheritdoc />
        public IDiscoverables? Discoverables => _discoverables;

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
        public void AddSource(ICore itemToMerge)
        {
            if (_sources.Contains(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Cannot add the same source twice.");

            _sources.Add(itemToMerge);
            _devices.AddRange(itemToMerge.Devices.Select(x => new DeviceAdapter(x)));
            _library.AddSource(itemToMerge.Library);

            if (itemToMerge.Discoverables is not null)
                _discoverables.AddSource(itemToMerge.Discoverables);

            if (itemToMerge.RecentlyPlayed is not null)
                _recentlyPlayed.AddSource(itemToMerge.RecentlyPlayed);

            if (itemToMerge.Pins is not null)
                _pins.AddSource(itemToMerge.Pins);

            AttachEvents(itemToMerge);
        }

        /// <inheritdoc />
        public void RemoveSource(ICore itemToRemove)
        {
            if (!_sources.Contains(itemToRemove))
                ThrowHelper.ThrowArgumentException(nameof(itemToRemove), "Cannot remove an item that doesn't exist in the collection.");

            _sources.Remove(itemToRemove);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            foreach (var source in _sources)
                DetachEvents(source);

            await _sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
