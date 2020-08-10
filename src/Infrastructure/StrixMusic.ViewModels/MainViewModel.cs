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
using StrixMusic.Services.Settings;
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
        public MainViewModel(ISettingsService settings)
        {
            LoadLibraryCommand = new AsyncRelayCommand(LoadLibraryAsync);
            LoadDevicesCommand = new AsyncRelayCommand(LoadDevicesAsync);

            Devices = new ObservableCollection<IDevice>();

            _cores = Ioc.Default.GetServices<ICore>().ToArray();

            _ = InitializeCores(_cores);
        }

        private async Task InitializeCores(IEnumerable<ICore> coresToLoad)
        {
            await coresToLoad.InParallel(core => Task.Run(core.InitAsync));

            var recentlyPlayed = await coresToLoad.InParallel(core => Task.Run(core.GetRecentlyPlayedAsync));
            var mergedRecentlyPlayed = Mergers.MergeRecentlyPlayed(recentlyPlayed);

            var discoverables = await coresToLoad.InParallel(core => Task.Run(core.GetDiscoverablesAsync));
            var mergedDiscoverables = Mergers.MergePlayableCollectionGroups(discoverables);
        }

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<IUser>? Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<IDevice> Devices { get; }

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public BindableLibrary? Library { get; set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public IRecentlyPlayed? RecentlyPlayed { get; set; }

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public ObservableCollection<BindableCollectionGroup>? Discoverables { get; }

        /// <summary>
        /// Search results.
        /// </summary>
        public ISearchResults? SearchResults { get; }

        /// <summary>
        /// Current search query.
        /// </summary>
        public string SearchQuery { get; set; } = string.Empty;

        /// <summary>
        /// Autocomplete for the current search query.
        /// </summary>
        public ObservableCollection<string>? SearchSuggestions { get; set; }

        /// <summary>
        /// Loads the <see cref="Library"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadLibraryCommand { get; }

        private async Task<ILibrary> LoadLibraryAsync()
        {
            var libs = await _cores.InParallel(core => Task.Run(core.GetLibraryAsync)).ConfigureAwait(false);
            var mergedLibrary = Mergers.MergeLibrary(libs);

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

            return mergedDevices;
        }
    }
}
