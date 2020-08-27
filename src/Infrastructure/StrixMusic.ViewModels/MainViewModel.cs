using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Extensions;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.ViewModels.Bindables;

namespace StrixMusic.ViewModels
{
    /// <summary>
    /// The MainViewModel used throughout the app
    /// </summary>
    public class MainViewModel : ObservableRecipient
    {
        private readonly IReadOnlyList<ICore> _cores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public MainViewModel()
        {
            LoadLibraryCommand = new AsyncRelayCommand(LoadLibraryAsync);
            LoadDevicesCommand = new AsyncRelayCommand(LoadDevicesAsync);
            LoadRecentlyPlayedCommand = new AsyncRelayCommand(LoadRecentlyPlayedAsync);
            LoadDiscoverablesCommand = new AsyncRelayCommand(LoadDiscoverablesAsync);

            Devices = new ObservableCollection<BindableDevice>();

            _cores = Ioc.Default.GetServices<ICore>().ToArray();

            _ = InitializeCores(_cores);
        }

        private async Task InitializeCores(IEnumerable<ICore> coresToLoad)
        {
            await coresToLoad.InParallel(core => Task.Run(core.InitAsync));

            foreach (var core in coresToLoad)
            {
                Users.Add(new BindableUserProfile(core.User));
                AttachEvents(core);
            }
        }

        private void AttachEvents(ICore core)
        {
            core.DevicesChanged += Core_DevicesChanged;
        }

        private void DetachEvents(ICore core)
        {
            core.DevicesChanged -= Core_DevicesChanged;
        }

        private void Core_DevicesChanged(object sender, CoreInterfaces.CollectionChangedEventArgs<IDevice> e)
        {
            foreach (var device in e.AddedItems)
            {
                Devices.Add(new BindableDevice(device));
            }

            foreach (var device in e.RemovedItems)
            {
                Devices.Remove(new BindableDevice(device));
            }
        }

        /// <summary>
        /// Contains data about the cores that are loaded.
        /// </summary>
        public ObservableCollection<BindableCore> BindableCoreData { get; } = new ObservableCollection<BindableCore>();

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<BindableUserProfile> Users { get; } = new ObservableCollection<BindableUserProfile>();

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<BindableDevice> Devices { get; }

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public BindableLibrary? Library { get; private set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public BindableRecentlyPlayed? RecentlyPlayed { get; private set; }

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public ObservableCollection<BindableCollectionGroup>? Discoverables { get; } = new ObservableCollection<BindableCollectionGroup>();

        /// <summary>
        /// Loads the <see cref="RecentlyPlayed"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadRecentlyPlayedCommand { get; }

        private async Task<IRecentlyPlayed> LoadRecentlyPlayedAsync()
        {
            var recents = await _cores.InParallel(core => Task.Run(core.GetRecentlyPlayedAsync)).ConfigureAwait(false);

            // TODO: Re-evaluate. We might not need to merge them like this, just replace the existing items per core.
            var mergedRecents = Mergers.MergeRecentlyPlayed(recents);

            RecentlyPlayed = new BindableRecentlyPlayed(mergedRecents);

            return mergedRecents;
        }

        /// <summary>
        /// Loads the <see cref="RecentlyPlayed"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadDiscoverablesCommand { get; }

        private async Task<IAsyncEnumerable<IPlayableCollectionGroup>?> LoadDiscoverablesAsync()
        {
            var discoverables = await _cores.InParallel(core => Task.Run(core.GetDiscoverablesAsync)).ConfigureAwait(false);
            var mergedDiscoverables = Mergers.MergePlayableCollectionGroups(discoverables);

            return mergedDiscoverables;
        }

        /// <summary>
        /// Loads the <see cref="Library"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadLibraryCommand { get; }

        private async Task<ILibrary> LoadLibraryAsync()
        {
            var libs = await _cores.InParallel(core => Task.Run(core.GetLibraryAsync)).ConfigureAwait(false);
            var mergedLibrary = Mergers.MergeLibrary(libs);

            Library = new BindableLibrary(mergedLibrary);

            return mergedLibrary;
        }

        /// <summary>
        /// Loads the <see cref="Devices"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadDevicesCommand { get; }

        private async Task<IAsyncEnumerable<IDevice>> LoadDevicesAsync()
        {
            var devices = await _cores.InParallel(core => Task.Run(core.GetDevicesAsync));
            var mergedDevices = Mergers.MergeDevices(devices);

            await foreach (var device in mergedDevices)
            {
                Devices.Add(new BindableDevice(device));
            }

            return mergedDevices;
        }
    }
}
