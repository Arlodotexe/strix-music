using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.Events;
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
        private readonly List<(ICore core, CancellationTokenSource cancellationToken)> _coreInitData = new List<(ICore, CancellationTokenSource)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel(StrixDevice strixDevice)
        {
            Singleton = this;
            LocalDevice = new DeviceViewModel(strixDevice);

            Devices = new ObservableCollection<DeviceViewModel>();

            Cores = new ObservableCollection<CoreViewModel>();
            Users = new ObservableCollection<UserProfileViewModel>();
            PlaybackQueue = new ObservableCollection<TrackViewModel>();
        }

        /// <summary>
        /// Raised when top-level navigation is needed.
        /// </summary>
        public event EventHandler<AppNavigationTarget>? AppNavigationRequested;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        private void AttachEvents()
        {
            foreach (var source in _sources)
            {
                source.DevicesChanged += Core_DevicesChanged;
            }

            foreach (var device in Devices)
            {
                device.IsActiveChanged += Device_IsActiveChanged;
            }
        }

        private void DetachEvents()
        {
            foreach (var source in _sources)
            {
                source.DevicesChanged -= Core_DevicesChanged;
            }

            foreach (var device in Devices)
            {
                device.IsActiveChanged -= Device_IsActiveChanged;
            }
        }

        /// <summary>
        /// Initializes and loads the cores given.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeCoresAsync(List<ICore> cores, Func<ICore, Task<IServiceCollection>> getInjectedCoreServices)
        {
            Guard.IsNotNull(cores, nameof(cores));

            _sources.AddRange(cores);

            await Cores.InParallel(x => x.DisposeAsync().AsTask());

            // These collections should be empty, no matter how many times the method is called.
            Devices.Clear();
            Cores.Clear();
            Users.Clear();
            PlaybackQueue.Clear();

            foreach (var core in cores)
            {
                await Task.Run(async () =>
                {
                    var services = await getInjectedCoreServices(core);
                    await InitCore(core, services);
                });
            }

            Library = new LibraryViewModel(new MergedLibrary(_sources.Select(x => x.Library)));
            await Library.InitAsync();

            if (_sources.All(x => x.RecentlyPlayed == null))
                RecentlyPlayed = new RecentlyPlayedViewModel(new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed).PruneNull()));

            if (_sources.All(x => x.Discoverables == null))
                Discoverables = new DiscoverablesViewModel(new MergedDiscoverables(_sources.Select(x => x.Discoverables).PruneNull()));

            Devices = new ObservableCollection<DeviceViewModel>(_sources.SelectMany(x => x.Devices, (core, device) => new DeviceViewModel(new CoreDeviceProxy(device))))
            {
                LocalDevice,
            };

            OnPropertyChanged(nameof(Devices));

            AttachEvents();
        }

        private async Task InitCore(ICore core, IServiceCollection services)
        {
            var cancellationToken = new CancellationTokenSource();
            _coreInitData.Add(new ValueTuple<ICore, CancellationTokenSource>(core, cancellationToken));

            // If the core requests config, cancel the init task.
            // Then wait for the core state to change to Configured.
            core.CoreStateChanged += OnCoreStateChanged_HandleConfigRequest;

            // Handle additional actions needed for core state changes (unloaded, etc)
            core.CoreStateChanged += Core_CoreStateChanged;

            try
            {
                await Task.Run(() => core.InitAsync(services), cancellationToken.Token);
            }
            catch (TaskCanceledException)
            {
                // TODO: if one core a already requested config, have a queue in case another tries.
                AppNavigationRequested?.Invoke(core, AppNavigationTarget.Settings);

                var timeAllowedForUISetup = TimeSpan.FromMinutes(10);

                var updatedState = await Threading.EventAsTask<CoreState>(cb => core.CoreStateChanged += cb, cb => core.CoreStateChanged -= cb, timeAllowedForUISetup);

                if (updatedState is null)
                {
                    // Timed out. Roll back changes? Unregister core? TODO.
                }
                else if (updatedState.Value.Result == CoreState.Configured)
                {
                    await InitCore(core, services);
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

            await Threading.OnPrimaryThread(() =>
            {
                // Registers itself into Cores
#pragma warning disable CA2000 // Dispose objects before losing scope
                _ = new CoreViewModel(core);
#pragma warning restore CA2000 // Dispose objects before losing scope
                if (core.User != null)
                    Users.Add(new UserViewModel(new CoreUserProxy(core.User)));

                foreach (var device in core.Devices)
                    Devices.Add(new DeviceViewModel(new CoreDeviceProxy(device)));
            });

            core.CoreStateChanged -= OnCoreStateChanged_HandleConfigRequest;

            cancellationToken.Dispose();
        }

        private void Core_CoreStateChanged(object sender, CoreState e)
        {
            OnCoreStateChanged_HandleCoreUnloaded(sender, e);
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

        private async void OnCoreStateChanged_HandleCoreUnloaded(object sender, CoreState e)
        {
            if (!(sender is ICore core))
                return;

            if (e == CoreState.Unloaded)
            {
                var relevantVm = Cores.FirstOrDefault(x => x.InstanceId == core.InstanceId);
                Guard.IsNotNull(relevantVm, nameof(relevantVm));

                await Threading.OnPrimaryThread(() => Cores.Remove(relevantVm));

                core.CoreStateChanged -= Core_CoreStateChanged;
                await relevantVm.DisposeAsync();
            }
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems)
        {
            Devices.ChangeCollection(addedItems, removedItems, x => new DeviceViewModel(new CoreDeviceProxy(x.Data)));

            foreach (var device in addedItems)
            {
                device.Data.IsActiveChanged += Device_IsActiveChanged;
            }

            foreach (var device in removedItems)
            {
                // TODO: Handle devices that haven't subscribed to the IsActiveChanged event.
                device.Data.IsActiveChanged -= Device_IsActiveChanged;
            }
        }

        private void Device_IsActiveChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(ActiveDevice)));

        /// <summary>
        /// The singleton instance of the <see cref="MainViewModel"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="MainViewModel"/> contains only logic for interacting with instance of cores, and relaying them to the UI.
        /// Creating more than one <see cref="MainViewModel"/> off the same instance IDs would result in a second instance of
        /// MainViewModel with identical states and functionality. Therefore, a singleton is desired over multi-instance.
        /// </remarks>
        public static MainViewModel? Singleton { get; private set; }

        /// <summary>
        /// Contains data about the cores that are loaded.
        /// </summary>
        public ObservableCollection<CoreViewModel> Cores { get; }

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<UserProfileViewModel> Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

        /// <inheritdoc />
        IReadOnlyList<IDevice> IAppCore.Devices => Devices;

        /// <inheritdoc />
        ILibrary IAppCore.Library => Library ?? throw new InvalidOperationException($"{nameof(MainViewModel)} not yet initialized.");

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
        public DeviceViewModel LocalDevice { get; }

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
