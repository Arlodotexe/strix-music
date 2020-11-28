using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        /// Initializes and loads the cores given.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeCores((ICore core, IServiceCollection services)[] initData)
        {
            var cores = initData.Select(x => x.core);

            _sources.AddRange(cores);

            await Cores.InParallel(x => x.DisposeAsync().AsTask());

            // These collections should be empty, no matter how many times the method is called.
            Devices.Clear();
            Cores.Clear();
            Users.Clear();
            PlaybackQueue.Clear();

            await initData.InParallel(async data =>
            {
                var (core, services) = data;

                // Registers itself into Cores
                _ = new CoreViewModel(core);

                await core.InitAsync(services);

                Users.Add(new UserViewModel(new MergedUser(core.User)));

                foreach (var device in core.Devices)
                    _devices.Add(new MergedDevice(device));
            });

            Library = new LibraryViewModel(new MergedLibrary(_sources.Select(x => x.Library)));

            RecentlyPlayed = new RecentlyPlayedViewModel(new MergedRecentlyPlayed(_sources.Select(x => x.RecentlyPlayed)));

            Discoverables = new DiscoverablesViewModel(new MergedDiscoverables(_sources.Select(x => x.Discoverables)));

            Devices = new SynchronizedObservableCollection<DeviceViewModel>(_sources.SelectMany(x => x.Devices, (core, device) => new DeviceViewModel(new MergedDevice(device))));

            AttachEvents();
        }

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
        public IPlayableCollectionGroup Pins { get; } = null!;

        /// <inheritdoc />
        IRecentlyPlayed IAppCore.RecentlyPlayed { get; } = null!;

        /// <inheritdoc />
        IDiscoverables IAppCore.Discoverables { get; } = null!;

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

        /// <summary>
        /// The current playback queue. First item plays next.
        /// </summary>
        public ObservableCollection<TrackViewModel> PlaybackQueue { get; }

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICore> ISdkMember<ICore>.Sources => _sources;

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            // TODO
            await Task.CompletedTask;
        }
    }
}
