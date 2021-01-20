using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merges multiple inputted <see cref="ICore"/>s into a single core.
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

            Discoverables = new MergedDiscoverables(_sources.Select(x => x.Discoverables));
            Library = new MergedLibrary(_sources.Select(x => x.Library));
            SourceCores = _sources.Select(x => x.SourceCore).ToList();
            Pins = new MergedPlayableCollectionGroup(_sources.Select(x => x.Pins).PruneNull());
            RecentlyPlayed = new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed));
            Devices = new SynchronizedObservableCollection<IDevice>(_sources.SelectMany(x => x.Devices, (core, device) => new DeviceProxy(device)));

            AttachEvents();
        }

        private void AttachEvents()
        {
            foreach (var core in _sources)
            {
                core.Devices.CollectionChanged += Devices_CollectionChanged;
            }
        }

        private void DetachEvents()
        {
            foreach (var core in _sources)
            {
                core.Devices.CollectionChanged -= Devices_CollectionChanged;
            }
        }

        private void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                if (item is ICoreDevice device)
                {
                    Devices.Add(new DeviceProxy(device));
                }
            }

            foreach (var item in e.OldItems)
            {
                if (item is ICoreDevice device)
                {
                    var deviceToRemove = Devices.FirstOrDefault(x => x.Id == device.Id && x.SourceCore?.InstanceId == device.SourceCore.InstanceId);

                    // Devices are not actually merged, so we don't need to check count / call .RemoveSource()
                    // If we found any matching devices, remove it entirely.
                    if (!(deviceToRemove is null))
                        Devices.Remove(deviceToRemove);
                }
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public void AddSource(ICore itemToMerge)
        {
            ThrowHelper.ThrowNotSupportedException("Adding or removing cores are not supported. ViewModel must be reset.");
        }

        /// <inheritdoc />
        public void RemoveSource(ICore itemToRemove)
        {
            ThrowHelper.ThrowNotSupportedException("Adding or removing sources are not supported. ViewModel must be reset.");
        }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICore> Sources => _sources;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IDevice> Devices { get; }

        /// <inheritdoc />
        public ILibrary Library { get; }

        /// <inheritdoc />
        public IPlayableCollectionGroup Pins { get; }

        /// <inheritdoc />
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <inheritdoc />
        public IDiscoverables Discoverables { get; }

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