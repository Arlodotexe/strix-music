using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains information about an <see cref="ICore"/>
    /// </summary>
    public class CoreViewModel : ObservableObject, ICore
    {
        private readonly ICore _core;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreViewModel"/> class.
        /// </summary>
        /// <param name="core">The base <see cref="ICore"/></param>
        /// <remarks>
        /// Creating a new <see cref="CoreViewModel"/> will register itself into <see cref="MainViewModel.Cores"/>.
        /// </remarks>
        public CoreViewModel(ICore core)
        {
            _core = core;

            MainViewModel.Singleton?.Cores.Add(this);

            Library = new LibraryViewModel(new MergedLibrary(_core.Library.IntoList()));

            if (_core.RecentlyPlayed != null)
                RecentlyPlayed = new RecentlyPlayedViewModel(new MergedRecentlyPlayed(_core.RecentlyPlayed.IntoList()));

            if (_core.Discoverables != null)
                Discoverables = new DiscoverablesViewModel(new MergedDiscoverables(_core.Discoverables.IntoList()));

            if (_core.Pins != null)
                Pins = new PlayableCollectionGroupViewModel(new MergedPlayableCollectionGroup(_core.Pins.IntoList()));

            if (_core.Search != null)
                Search = new SearchViewModel(new MergedSearch(_core.Search.IntoList()));

            Devices = new ObservableCollection<ICoreDevice>();

            CoreState = _core.CoreState;

            AttachEvents();
        }

        private void AttachEvents()
        {
            _core.CoreStateChanged += Core_CoreStateChanged;
            _core.DevicesChanged += Core_DevicesChanged;
        }

        private void DetachEvents()
        {
            _core.CoreStateChanged -= Core_CoreStateChanged;
            _core.DevicesChanged -= Core_DevicesChanged;
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICoreDevice>> removedItems)
        {
            foreach (var item in removedItems)
            {
                _ = Threading.OnPrimaryThread(() => Devices.RemoveAt(item.Index));
            }

            var sortedIndices = removedItems.Select(x => x.Index).ToList();
            sortedIndices.Sort();

            // If elements are removed before they are added, the added items may be inserted at the wrong index.
            // To compensate, we need to check how many items were removed before the current index and shift the insert position back by that amount.
            for (var i = 0; i > addedItems.Count; i++)
            {
                var item = addedItems[i];
                var insertOffset = item.Index;

                // Finds the last removed index where the value is less than current pos.
                // Quicker to do this by getting the first removed index where value is greater than current pos, minus 1 index.
                var closestPrecedingRemovedIndex = sortedIndices.FindIndex(x => x > i) - 1;

                // If found
                if (closestPrecedingRemovedIndex != -1)
                {
                    // Shift the insert position backwards by the number of items that were removed
                    insertOffset = closestPrecedingRemovedIndex * -1;
                }

                // Insert the item
                _ = Threading.OnPrimaryThread(() => Devices.InsertOrAdd(insertOffset, item.Data));
            }
        }

        /// <inheritdoc cref="ICore.CoreState" />
        private void Core_CoreStateChanged(object sender, CoreState e)
        {
            CoreState = e;

            _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(CoreState)));
        }

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc cref="ICore.Name" />
        public string Name => _core.Name;

        /// <inheritdoc cref="ICore.User" />
        public ICoreUser? User => _core.User;

        /// <inheritdoc cref="ICore.CoreConfig" />
        public ICoreConfig CoreConfig => _core.CoreConfig;

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState { get; internal set; }

        /// <inheritdoc />
        IReadOnlyList<ICoreDevice> ICore.Devices => Devices;

        /// <inheritdoc cref="ICore.Devices" />
        public ObservableCollection<ICoreDevice> Devices { get; }

        /// <inheritdoc cref="ICore.Library" />
        ICoreLibrary ICore.Library => _core.Library;

        /// <inheritdoc cref="LibraryViewModel"/>
        public LibraryViewModel Library { get; }

        /// <inheritdoc />
        ICoreSearch? ICore.Search { get; }

        /// <inheritdoc cref="SearchViewModel"/>
        public SearchViewModel? Search { get; }

        /// <inheritdoc cref="ICore.RecentlyPlayed" />
        ICoreRecentlyPlayed? ICore.RecentlyPlayed => _core.RecentlyPlayed;

        /// <inheritdoc cref="RecentlyPlayed"/>
        public RecentlyPlayedViewModel? RecentlyPlayed { get; }

        /// <inheritdoc cref="ICore.Discoverables" />
        ICoreDiscoverables? ICore.Discoverables => _core.Discoverables;

        /// <inheritdoc cref="DiscoverablesViewModel" />
        public DiscoverablesViewModel? Discoverables { get; }

        /// <inheritdoc cref="ICore.Pins" />
        ICorePlayableCollectionGroup? ICore.Pins => _core.Pins;

        /// <inheritdoc cref="ICore.Pins" />
        public PlayableCollectionGroupViewModel? Pins { get; }

        /// <inheritdoc cref="IAsyncInit.InitAsync" />
        public Task InitAsync(IServiceCollection services) => _core.InitAsync(services);

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync()
        {
            await _core.DisposeAsync().ConfigureAwait(false);
            DetachEvents();
        }

        /// <inheritdoc/>
        public Task<ICoreMember?> GetContextById(string id) => _core.GetContextById(id);

        /// <inheritdoc />
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track) => _core.GetMediaSource(track);

        /// <inheritdoc cref="ICore.CoreStateChanged" />
        public event EventHandler<CoreState>? CoreStateChanged
        {
            add => _core.CoreStateChanged += value;

            remove => _core.CoreStateChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public ICore SourceCore => _core.SourceCore;
    }
}
