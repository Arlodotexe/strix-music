// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.CoreManagement;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.Plugins;
using StrixMusic.Sdk.Plugins.Model;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.ViewModels.Notifications;

namespace StrixMusic.Sdk
{
    /// <summary>
    /// The primary, root view model used to interact with all merged core sources.
    /// </summary>
    public sealed partial class MainViewModel : ObservableRecipient, IAppCore, IAsyncInit
    {
        private readonly ICoreManagementService _coreManagementService;
        private readonly INotificationService _notificationService;

        private readonly Dictionary<string, CancellationTokenSource> _coreInitCancellationTokens = new();
        private readonly List<ICore> _sources = new();

        private MergedLibrary? _mergedLibrary;
        private MergedRecentlyPlayed? _mergedRecentlyPlayed;
        private MergedDiscoverables? _mergedDiscoverables;
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel(StrixDevice strixDevice, INotificationService notificationsService, ICoreManagementService coreManagementService)
        {
            _syncContext = SynchronizationContext.Current;
            _coreManagementService = coreManagementService;
            _notificationService = notificationsService;
            LocalDevice = new DeviceViewModel(this, strixDevice);

            Devices = new ObservableCollection<DeviceViewModel>();
            Notifications = new NotificationsViewModel(notificationsService);

            Cores = new ObservableCollection<CoreViewModel>();
            Users = new ObservableCollection<UserViewModel>();

            AttachEvents();
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

        private void AttachEvents()
        {
            _coreManagementService.CoreInstanceRegistered += CoreManagementService_CoreInstanceRegistered;
            _coreManagementService.CoreInstanceUnregistered += CoreManagementService_CoreInstanceUnregistered;
        }

        private void AttachEvents(IDevice device)
        {
            device.IsActiveChanged += Device_IsActiveChanged;
        }

        private void AttachEvents(ICore core)
        {
            core.DevicesChanged += Core_DevicesChanged;
        }

        private void DetachEvents()
        {
            _coreManagementService.CoreInstanceRegistered -= CoreManagementService_CoreInstanceRegistered;
            _coreManagementService.CoreInstanceUnregistered -= CoreManagementService_CoreInstanceUnregistered;
        }

        private void DetachEvents(IDevice device)
        {
            device.IsActiveChanged += Device_IsActiveChanged;
        }

        private void DetachEvents(ICore core)
        {
            core.DevicesChanged += Core_DevicesChanged;
        }

        private async void CoreManagementService_CoreInstanceRegistered(object sender, CoreInstanceEventArgs e)
        {
            var core = await CreateCore(e.InstanceId, e.CoreMetadata);
            if (core is null)
                return;

            await InitCore(core);

            // If added core after Initialized, we need to manually finish setting up the core.
            // If adding core before Initialized, InitAsync will finish setup.
            if (!IsInitialized)
                return;

            Guard.IsNotNull(_mergedLibrary, nameof(_mergedLibrary));

            _mergedLibrary.Cast<IMergedMutable<ICoreLibrary>>().AddSource(core.Library);

            if (core.Discoverables != null && _mergedDiscoverables != null)
                _mergedDiscoverables.Cast<IMergedMutable<ICoreDiscoverables>>().AddSource(core.Discoverables);

            if (core.RecentlyPlayed != null && _mergedRecentlyPlayed != null)
                _mergedRecentlyPlayed.Cast<IMergedMutable<ICoreRecentlyPlayed>>().AddSource(core.RecentlyPlayed);

            _syncContext.Post(_ =>
            {
                foreach (var device in core.Devices)
                {
                    var deviceVm = new DeviceViewModel(this, new CoreDeviceProxy(device));
                    Devices.Add(deviceVm);
                    AttachEvents(deviceVm);
                }

                if (core.User != null)
                    Users.Add(new UserViewModel(this, new CoreUserProxy(core.User)));
            }, null);

            AttachEvents(core);
        }

        private async void CoreManagementService_CoreInstanceUnregistered(object sender, CoreInstanceEventArgs e)
        {
            var relevantVm = Cores.First(x => x.InstanceId == e.InstanceId);
            Cores.Remove(relevantVm);

            var source = _sources.First(x => x.InstanceId == e.InstanceId);
            _sources.Remove(source);

            if (_mergedDiscoverables != null && source.Discoverables != null && _mergedDiscoverables.Sources.Contains(source.Discoverables))
                _mergedDiscoverables.Cast<IMergedMutable<ICoreDiscoverables>>().RemoveSource(source.Discoverables);

            if (_mergedRecentlyPlayed != null && source.RecentlyPlayed != null && _mergedRecentlyPlayed.Sources.Contains(source.RecentlyPlayed))
                _mergedRecentlyPlayed.Cast<IMergedMutable<ICoreRecentlyPlayed>>().RemoveSource(source.RecentlyPlayed);

            if (_mergedLibrary?.Sources.Contains(source.Library) ?? false)
                _mergedLibrary.Cast<IMergedMutable<ICoreLibrary>>().RemoveSource(source.Library);

            await relevantVm.DisposeAsync();
        }

        /// <summary>
        /// Gets a loaded core from the list of loaded <see cref="Cores"/>.
        /// </summary>
        /// <param name="reference">The core to look for.</param>
        /// <returns>The loaded, observable core.</returns>
        public CoreViewModel GetLoadedCore(ICore reference) => Cores.First(x => x.InstanceId == reference.InstanceId);

        /// <summary>
        /// Initializes and loads the ViewModel, including initializing all registered cores.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            Plugins.ModelPlugins = GlobalModelPluginConnector.Create(Plugins.ModelPlugins);

            var coreInstanceRegistry = await _coreManagementService.GetCoreInstanceRegistryAsync();

            Guard.IsNotNull(coreInstanceRegistry, nameof(coreInstanceRegistry));
            Guard.IsGreaterThan(coreInstanceRegistry.Count, 0, nameof(coreInstanceRegistry));

            foreach (var registeredCoreInstance in coreInstanceRegistry)
            {
                var instanceId = registeredCoreInstance.Key;
                var coreMetadata = registeredCoreInstance.Value;

                // Any registered cores that have already been set up will be ignored.
                await CreateCore(instanceId, coreMetadata);
            }

            var unloadedSources = _sources.Where(x => x.CoreState == CoreState.Unloaded);
            var enumerable = unloadedSources as ICore[] ?? unloadedSources.ToArray();

            foreach (var core in enumerable)
                await InitCore(core);

            _mergedLibrary = new MergedLibrary(_sources.Select(x => x.Library), MergeConfig);
            Library = new LibraryViewModel(this, _mergedLibrary);
            await Library.InitAsync();

            if (_sources.Any(x => x.RecentlyPlayed != null))
            {
                _mergedRecentlyPlayed = new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed).PruneNull(), MergeConfig);
                RecentlyPlayed = new RecentlyPlayedViewModel(this, _mergedRecentlyPlayed);
            }

            if (_sources.Any(x => x.Discoverables != null))
            {
                _mergedDiscoverables = new MergedDiscoverables(_sources.Select(x => x.Discoverables).PruneNull(), MergeConfig);
                Discoverables = new DiscoverablesViewModel(this, _mergedDiscoverables);
            }

            Devices = new ObservableCollection<DeviceViewModel>(enumerable.SelectMany(x => x.Devices, (_, device) => new DeviceViewModel(this, new CoreDeviceProxy(device))))
            {
                LocalDevice,
            };

            OnPropertyChanged(nameof(Devices));

            DevicesChanged?.Invoke(this, Devices.Select((x, i) => new CollectionChangedItem<IDevice>(x, i)).ToList(), new List<CollectionChangedItem<IDevice>>());

            foreach (var device in Devices)
                AttachEvents(device);

            Users = new ObservableCollection<UserViewModel>(enumerable.Select(x => x.User).PruneNull().Select(x => new UserViewModel(this, new CoreUserProxy(x))));

            _sources.ForEach(AttachEvents);

            IsInitialized = true;
        }

        private async Task<ICore?> CreateCore(string instanceId, CoreMetadata coreMetadata)
        {
            // Skip if registered but already set up.
            if (_sources.Any(x => x.InstanceId == instanceId))
                return null;

            var currentSdkVersion = typeof(ICore).Assembly.GetName().Version;
            if (coreMetadata.SdkVer != currentSdkVersion)
            {
                _notificationService.RaiseNotification(
                    $"{coreMetadata.DisplayName} not compatible", 
                    $"Uses SDK version {coreMetadata.SdkVer}, which is not compatible with the current version {currentSdkVersion}.");

                return null;
            }

            var core = await CoreRegistry.CreateCoreAsync(coreMetadata.Id, instanceId);
            _sources.Add(core);

            // Adds itself into Cores.
            // Library etc. need the CoreViewModel, but are created before CoreViewModel ctor is finished,
            // before the below can be added to the list of MainViewModel.Cores.
            _syncContext.Post(_ =>
            {
                _ = new CoreViewModel(this, core, coreMetadata);
            }, null);
            return core;
        }

        /// <summary>
        /// Initializes and sets up the given core, triggering UI as needed.
        /// </summary>
        /// <param name="core">The core to initialize</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task InitCore(ICore core)
        {
            var setupCancellationTokenSource = new CancellationTokenSource();
            _coreInitCancellationTokens.GetOrAdd(core.InstanceId, setupCancellationTokenSource);

            // If the core requests setup, cancel the init task.
            // Then wait for the core state to change to Configured.
            core.CoreStateChanged += OnCoreStateChanged_HandleConfigRequest;

            var cancelled = true;

            try
            {
#warning Improper cancellation. Refactor to pass the token directly.
                await core.InitAsync(setupCancellationTokenSource.Token);
            }
            #warning Handle special exceptions like HttpException + catch all others
            catch (OperationCanceledException)
            {
                cancelled = true;
            }

            _coreInitCancellationTokens.Remove(core.InstanceId);

            if (core.CoreState == CoreState.Configured || core.CoreState == CoreState.Loaded)
            {
                setupCancellationTokenSource.Dispose();
            }
            else if (core.CoreState == CoreState.Unloaded || cancelled)
            {
                setupCancellationTokenSource.Dispose();
                await _coreManagementService.UnregisterCoreInstanceAsync(core.InstanceId);
            }
            else if (setupCancellationTokenSource.IsCancellationRequested)
            {
                // TODO: Re-evaluate. Is the loop/EventAsTask needed? Can we just check state and do a recursive local function call?
                while (true)
                {
                    // Wait for the configuration to complete.
                    var timeAllowedForUISetup = TimeSpan.FromMinutes(10);
                    var updatedState = await Flow.EventAsTask<CoreState>(cb => core.CoreStateChanged += cb, cb => core.CoreStateChanged -= cb, timeAllowedForUISetup);

                    // Timed out or cancelled.
                    if (updatedState is null || updatedState.Value.Result == CoreState.Unloaded)
                    {
                        setupCancellationTokenSource.Dispose();
                        await _coreManagementService.UnregisterCoreInstanceAsync(core.InstanceId);
                        break;
                    }

                    if (updatedState.Value.Result == CoreState.NeedsSetup)
                    {
                        // If the user needs to set up the core, wait for another state change.
                        // Displaying the config UI is handled in the relevant ViewModel.
                        continue;
                    }

                    if (updatedState.Value.Result == CoreState.Configured)
                    {
                        await InitCore(core);
                    }

                    break;
                }
            }

            core.CoreStateChanged -= OnCoreStateChanged_HandleConfigRequest;

            setupCancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Method fires if core configuration is requested during InitAsync.
        /// </summary>
        private void OnCoreStateChanged_HandleConfigRequest(object sender, CoreState e)
        {
            if (sender is not ICore core)
            {
                ThrowHelper.ThrowInvalidOperationException();
                return;
            }

            if (e == CoreState.NeedsSetup)
            {
                var cancellationToken = _coreInitCancellationTokens.First(x => x.Key == core.InstanceId).Value;

                cancellationToken.Cancel();

                core.CoreStateChanged -= OnCoreStateChanged_HandleConfigRequest;
            }
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems)
        {
            var wrappedAddedVms = addedItems.Select(x => new CollectionChangedItem<DeviceViewModel>(new DeviceViewModel(this, new CoreDeviceProxy(x.Data)), x.Index)).ToList();
            var wrappedRemovedVms = removedItems.Select(x => new CollectionChangedItem<DeviceViewModel>(Devices.ElementAt(x.Index), x.Index)).ToList();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var wrappedAdded = wrappedAddedVms.Cast<CollectionChangedItem<IDevice>>().ToOrAsList();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var wrappedRemoved = wrappedRemovedVms.Cast<CollectionChangedItem<IDevice>>().ToOrAsList();

            Devices.ChangeCollection(wrappedAddedVms, wrappedRemovedVms);

            DevicesChanged?.Invoke(this, wrappedAdded, wrappedRemoved);

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

        private void Device_IsActiveChanged(object sender, bool e)
        {
            ActiveDeviceChanged?.Invoke(this, ActiveDevice);
            _syncContext.Post(_ => OnPropertyChanged(nameof(ActiveDevice)), null);
        }

        /// <inheritdoc cref="IAsyncInit.IsInitialized"/>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Contains data about the cores that are loaded.
        /// </summary>
        public ObservableCollection<CoreViewModel> Cores { get; }

        /// <summary>
        /// The notifications displayed in the app.
        /// </summary>
        public NotificationsViewModel Notifications { get; }

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<UserViewModel> Users { get; private set; }

        /// <inheritdoc />
        public PluginManager Plugins { get; } = new();

        /// <inheritdoc />
        public MergedCollectionConfig MergeConfig { get; } = new();

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
        /// Gets the first active device in <see cref="Devices"/>.
        /// </summary>
        public DeviceViewModel? ActiveDevice => Devices.FirstOrDefault(x => x.IsActive);

        /// <summary>
        /// Gets the device which is used for local playback.
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
        /// Raised when <see cref="ActiveDevice"/> is changed.
        /// </summary>
        public event EventHandler<IDevice?>? ActiveDeviceChanged;

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync()
        {
            await Task.CompletedTask;

            foreach (var device in Devices)
                DetachEvents(device);

            _sources.ForEach(DetachEvents);

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
