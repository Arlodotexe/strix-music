using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk
{
    /// <summary>
    /// The MainViewModel used throughout the app
    /// </summary>
    public partial class MainViewModel : ObservableRecipient, IAppCore, IAsyncDisposable
    {
        private readonly List<ICore> _sources = new List<ICore>();
        private readonly SynchronizedObservableCollection<IDevice> _devices = new SynchronizedObservableCollection<IDevice>();

        private readonly List<(ICore core, CancellationTokenSource cancellationToken)> _coreInitData = new List<(ICore, CancellationTokenSource)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            Singleton = this;

            Devices = new SynchronizedObservableCollection<DeviceViewModel>();

            Cores = new SynchronizedObservableCollection<CoreViewModel>();
            Users = new SynchronizedObservableCollection<UserProfileViewModel>();
            PlaybackQueue = new SynchronizedObservableCollection<TrackViewModel>();
        }

        /// <summary>
        /// Raised when top-level navigation is needed.
        /// </summary>
        public event EventHandler<AppNavigationTarget>? AppNavigationRequested;

        private void AttachEvents()
        {
            foreach (var source in _sources)
            {
                source.Devices.CollectionChanged += Devices_CollectionChanged;
            }
        }

        private void DetachEvents()
        {
            foreach (var source in _sources)
            {
                source.Devices.CollectionChanged -= Devices_CollectionChanged;
            }
        }

        /// <summary>
        /// Initializes and loads the cores given.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeCoresAsync((ICore core, IServiceCollection services)[] initData)
        {
            var cores = initData.Select(x => x.core);

            _sources.AddRange(cores);

            await Cores.InParallel(x => x.DisposeAsync().AsTask());

            // These collections should be empty, no matter how many times the method is called.
            Devices.Clear();
            Cores.Clear();
            Users.Clear();
            PlaybackQueue.Clear();

            foreach (var coreInitData in initData)
            {
                await InitCore(coreInitData);
            }

            Library = new LibraryViewModel(new MergedLibrary(_sources.Select(x => x.Library)));

            if (_sources.All(x => x.RecentlyPlayed == null))
                RecentlyPlayed = new RecentlyPlayedViewModel(new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed).PruneNull()));

            if (_sources.All(x => x.Discoverables == null))
                Discoverables = new DiscoverablesViewModel(new MergedDiscoverables(_sources.Select(x => x.Discoverables).PruneNull()));

            Devices = new SynchronizedObservableCollection<DeviceViewModel>(_sources.SelectMany(x => x.Devices, (core, device) => new DeviceViewModel(new CoreDeviceProxy(device))));

            AttachEvents();
        }

        private async Task InitCore((ICore core, IServiceCollection services) data)
        {
            var (core, services) = data;

            var cancellationToken = new CancellationTokenSource();
            _coreInitData.Add(new ValueTuple<ICore, CancellationTokenSource>(core, cancellationToken));

            // If the core requests config, cancel the init task.
            // Then wait for the core state to change to Configured.
            core.CoreStateChanged += OnCoreStateChanged_HandleConfigRequest;

            try
            {
                await Task.Run(() => core.InitAsync(services), cancellationToken.Token);
            }
            catch (TaskCanceledException)
            {
            }

            // TODO: if one core a already requested config, have a queue in case another tries.
            if (cancellationToken.IsCancellationRequested)
            {
                AppNavigationRequested?.Invoke(core, AppNavigationTarget.Settings);

                var timeAllowedForUISetup = TimeSpan.FromMinutes(10);

                var updatedState = await Threading.EventAsTask<CoreState>(cb => core.CoreStateChanged += cb, cb => core.CoreStateChanged -= cb, timeAllowedForUISetup);

                if (updatedState is null)
                {
                    // Timed out. Roll back changes? Unregister core? TODO.
                }
                else if (updatedState.Value.Result == CoreState.Configured)
                {
                    await InitCore(data);
                }
                else if (updatedState.Value.Result == CoreState.Unloaded)
                {
                    // User cancelled core loading.
                    cancellationToken.Dispose();
                    return;
                }

                cancellationToken.Dispose();
                return;
            }

            // Registers itself into Cores
#pragma warning disable CA2000 // Dispose objects before losing scope
            _ = new CoreViewModel(core);
#pragma warning restore CA2000 // Dispose objects before losing scope

            Users.Add(new UserViewModel(new CoreUserProxy(core.User)));

            foreach (var device in core.Devices)
                _devices.Add(new CoreDeviceProxy(device));

            core.CoreStateChanged -= OnCoreStateChanged_HandleConfigRequest;
            cancellationToken.Dispose();
        }

        private void OnCoreStateChanged_HandleConfigRequest(object sender, CoreState e)
        {
            if (!(sender is ICore core))
                return;

            if (e == CoreState.Configuring)
            {
                var cancellationToken = _coreInitData.First(x => x.core == core).cancellationToken;

                cancellationToken.Cancel();

                core.CoreStateChanged -= OnCoreStateChanged_HandleConfigRequest;
            }
        }

        private void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is IDevice device)
                    {
                        _devices.Add(device);
                        Devices.Add(new DeviceViewModel(device));
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is IDevice device)
                    {
                        _devices.Remove(device);
                        var vmToRemove = Devices.FirstOrDefault(x => x.Id == device.Id && x.Name == device.Name);
                        Devices.Remove(vmToRemove);
                    }
                }
            }
        }

        /// <summary>
        /// The single instance of the <see cref="MainViewModel"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="MainViewModel"/> contains only logic for interacting with instance of cores, and relaying them to the UI.
        /// With how the settings are set up, creating more than one <see cref="MainViewModel"/> off the same instance IDs would result in a
        /// second instance of MainViewModel with identical states and functionality. Therefore, a singleton is desired over multi-instance.
        /// </remarks>
        public static MainViewModel? Singleton { get; private set; }

        /// <summary>
        /// Contains data about the cores that are loaded.
        /// </summary>
        public SynchronizedObservableCollection<CoreViewModel> Cores { get; }

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public SynchronizedObservableCollection<UserProfileViewModel> Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        public SynchronizedObservableCollection<DeviceViewModel> Devices { get; private set; }

        /// <inheritdoc />
        SynchronizedObservableCollection<IDevice> IAppCore.Devices => _devices;

        /// <inheritdoc />
        ILibrary IAppCore.Library { get; } = null!;

        /// <inheritdoc />
        IPlayableCollectionGroup? IAppCore.Pins { get; }

        /// <inheritdoc />
        IRecentlyPlayed? IAppCore.RecentlyPlayed { get; }

        /// <inheritdoc />
        IDiscoverables? IAppCore.Discoverables { get; }

        /// <inheritdoc />
        ISearch? IAppCore.Search { get; }

        /// <summary>
        /// Gets the active device in <see cref="Devices"/>.
        /// </summary>
        public DeviceViewModel? ActiveDevice => Devices.FirstOrDefault(x => x.IsActive);

        /// <summary>
        /// Gets the active device in <see cref="Devices"/>.
        /// </summary>
        public DeviceViewModel? LocalDevice => Devices.FirstOrDefault(x => x.Type == DeviceType.Local && x.Model is StrixDevice);

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public LibraryViewModel? Library { get; private set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public RecentlyPlayedViewModel? RecentlyPlayed { get; private set; }

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public DiscoverablesViewModel? Discoverables { get; private set; }

        /// <inheritdoc cref="SearchViewModel" />
        public SearchViewModel? Search { get; }

        /// <summary>
        /// The current playback queue. First item plays next.
        /// </summary>
        public ObservableCollection<TrackViewModel> PlaybackQueue { get; }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await Task.CompletedTask;
            DetachEvents();
        }

        /// <inheritdoc />
        public bool Equals(ICore other) => false;

        /// <inheritdoc />
        IReadOnlyList<ICore> IMerged<ICore>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICore> IMerged<ICore>.SourceCores => _sources;
    }
}
