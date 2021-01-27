using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Aggregates many <see cref="ICore"/> instances into one.
    /// </summary>
    public class MergedCore : IAppCore
    {
        private readonly List<ICore> _sources;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCore"/>.
        /// </summary>
        /// <param name="cores">The cores to merge together into this object.</param>
        public MergedCore(IEnumerable<ICore> cores)
        {
            _sources = cores?.ToList() ?? ThrowHelper.ThrowArgumentNullException<List<ICore>>(nameof(cores));
            SourceCores = _sources.Select(x => x.SourceCore).ToList();

            Library = new MergedLibrary(_sources.Select(x => x.Library));

            if (_sources.All(x => x.Discoverables == null))
                Discoverables = new MergedDiscoverables(_sources.Select(x => x.Discoverables).PruneNull());

            if (_sources.All(x => x.Pins == null))
                Pins = new MergedPlayableCollectionGroup(_sources.Select(x => x.Pins).PruneNull());

            if (_sources.All(x => x.RecentlyPlayed == null))
                RecentlyPlayed = new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed).PruneNull());

            Devices = new SynchronizedObservableCollection<IDevice>(_sources.SelectMany(x => x.Devices, (core, device) => new CoreDeviceProxy(device)));

            AttachEvents();
        }

        private void AttachEvents()
        {
            foreach (var core in _sources)
            {
                core.DevicesChanged += Core_DevicesChanged;
            }
        }

        private void DetachEvents()
        {
            foreach (var core in _sources)
            {
                core.DevicesChanged -= Core_DevicesChanged;
            }
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICoreDevice>> removedItems)
        {
            var itemsToAdd = addedItems.Select(x => new CollectionChangedEventItem<IDevice>(new CoreDeviceProxy(x.Data), x.Index)).ToList();
            var itemsToRemove= removedItems.Select(x => new CollectionChangedEventItem<IDevice>(new CoreDeviceProxy(x.Data), x.Index)).ToList();

            DevicesChanged?.Invoke(this, itemsToAdd, itemsToRemove);
        }

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICore> Sources => _sources;

        /// <inheritdoc />
        public IReadOnlyList<IDevice> Devices { get; }

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
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        /// <inheritdoc />
        /// <remarks>
        /// Cores are all merged, but never matched
        /// </remarks>
        public bool Equals(ICore other)
        {
            return false;
        }
    }
}