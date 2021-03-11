using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            Devices = new ObservableCollection<DeviceViewModel>();

            CoreState = _core.CoreState;

            AttachEvents();
        }

        private void AttachEvents()
        {
            _core.CoreStateChanged += Core_CoreStateChanged;
            _core.DevicesChanged += Core_DevicesChanged;
            _core.InstanceDescriptorChanged += Core_InstanceDescriptorChanged;
        }

        private void DetachEvents()
        {
            _core.CoreStateChanged -= Core_CoreStateChanged;
            _core.DevicesChanged -= Core_DevicesChanged;
            _core.InstanceDescriptorChanged -= Core_InstanceDescriptorChanged;
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Devices.ChangeCollection(addedItems, removedItems, item => new DeviceViewModel(new CoreDeviceProxy(item.Data)));
            });
        }

        private void Core_InstanceDescriptorChanged(object sender, string e)
        {
            OnPropertyChanged(nameof(InstanceDescriptor));
            InstanceDescriptorChanged?.Invoke(sender, e);
        }

        /// <inheritdoc cref="ICore.CoreState" />
        private void Core_CoreStateChanged(object sender, CoreState e)
        {
            CoreState = e;

            _ = Threading.OnPrimaryThread(() =>
            {
                OnPropertyChanged(nameof(CoreState));

                OnPropertyChanged(nameof(IsCoreStateUnloaded));
                OnPropertyChanged(nameof(IsCoreStateConfiguring));
                OnPropertyChanged(nameof(IsCoreStateConfigured));
                OnPropertyChanged(nameof(IsCoreStateLoading));
                OnPropertyChanged(nameof(IsCoreStateLoaded));
                OnPropertyChanged(nameof(IsCoreStateFaulted));
            });
        }

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc cref="ICore.Name" />
        public string Name => _core.Name;

        /// <inheritdoc />
        public string InstanceDescriptor => _core.InstanceDescriptor;

        /// <inheritdoc cref="ICore.User" />
        public ICoreUser? User => _core.User;

        /// <inheritdoc cref="ICore.CoreConfig" />
        public ICoreConfig CoreConfig => _core.CoreConfig;

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState { get; internal set; }

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Data.CoreState.Unloaded"/>.
        /// </summary>
        public bool IsCoreStateUnloaded => CoreState == CoreState.Unloaded;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Data.CoreState.Configuring"/>.
        /// </summary>
        public bool IsCoreStateConfiguring => CoreState == CoreState.Configuring;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Data.CoreState.Configured"/>.
        /// </summary>
        public bool IsCoreStateConfigured => CoreState == CoreState.Configured;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Data.CoreState.Loading"/>.
        /// </summary>
        public bool IsCoreStateLoading => CoreState == CoreState.Loading;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Data.CoreState.Loaded"/>.
        /// </summary>
        public bool IsCoreStateLoaded => CoreState == CoreState.Loaded;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Data.CoreState.Faulted"/>.
        /// </summary>
        public bool IsCoreStateFaulted => CoreState == CoreState.Faulted;

        /// <inheritdoc />
        IReadOnlyList<ICoreDevice> ICore.Devices => _core.Devices;

        /// <inheritdoc cref="ICore.Devices" />
        public ObservableCollection<DeviceViewModel> Devices { get; }

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
        public event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc />
        public ICore SourceCore => _core.SourceCore;
    }
}
