using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Extensions;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;
using StrixMusic.Sdk.Observables;

namespace StrixMusic.Sdk
{
    /// <summary>
    /// The MainViewModel used throughout the app
    /// </summary>
    public class MainViewModel : ObservableRecipient
    {
        private readonly IEnumerable<ICore> _cores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel(IEnumerable<ICore> cores)
        {
            _cores = cores;

            Devices = new ObservableCollection<ObservableDevice>();
            SearchAutoComplete = new ObservableCollection<string>();

            LoadedCores = new ObservableCollection<ObservableCore>();
            Users = new ObservableCollection<ObservableUserProfile>();
            PlaybackQueue = new ObservableCollection<ObservableTrack>();

            GetSearchResultsAsyncCommand = new AsyncRelayCommand<string>(GlobalSearchResultsAsync);
            GetSearchAutoSuggestAsyncCommand = new RelayCommand<string>(GlobalSearchSuggestions);

            _ = InitializeCores(_cores);
        }

        private async Task InitializeCores(IEnumerable<ICore> coresToLoad)
        {
            // Handle possible multiple enumeration
            var toLoad = coresToLoad as ICore[] ?? coresToLoad.ToArray();

            await toLoad.InParallel(core => Task.Run(core.InitAsync));

            foreach (var core in toLoad)
            {
                Users.Add(new ObservableUserProfile(core.User));

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

        /// <summary>
        /// Gets search suggestions from all cores and asynchronously populate it into <see cref="SearchAutoComplete"/>.
        /// </summary>
        /// <param name="query">The query to search for.</param>
        public void GlobalSearchSuggestions(string query)
        {
            SearchAutoComplete.Clear();

            Parallel.ForEach(_cores, async core =>
            {
                var searchResults = await core.GetSearchAutoCompleteAsync(query);
                if (searchResults == null)
                    return;

                foreach (var item in searchResults)
                {
                    if (!SearchAutoComplete.Contains(item))
                        SearchAutoComplete.Add(item);
                }
            });
        }

        /// <summary>
        /// Performs a search on all loaded cores, and loads it into <see cref="SearchResults"/>.
        /// </summary>
        /// <param name="query">The query to search for.</param>
        /// <returns>The merged search results.</returns>
        public async Task<ISearchResults> GlobalSearchResultsAsync(string query)
        {
            var searchResults = await _cores.InParallel(core => Task.Run(() => core.GetSearchResultsAsync(query)));

            // TODO: Merge search results
            var merged = searchResults.First();

            SearchResults = new ObservableSearchResults(merged);

            return merged;
        }

        private void Core_DevicesChanged(object sender, CollectionChangedEventArgs<IDevice> e)
        {
            foreach (var item in e.AddedItems)
            {
                Devices.RemoveAt(item.Index);
            }

            foreach (var item in e.RemovedItems)
            {
                Devices.RemoveAt(item.Index);
            }
        }

        /// <summary>
        /// Contains data about the cores that are loaded.
        /// </summary>
        public ObservableCollection<ObservableCore> LoadedCores { get; }

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<ObservableUserProfile> Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<ObservableDevice> Devices { get; }

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public ObservableLibrary? Library { get; private set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public ObservableRecentlyPlayed? RecentlyPlayed { get; private set; }

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public ObservableCollectionGroup? Discoverables { get; private set; }

        /// <summary>
        /// Gets search results for a query.
        /// </summary>
        public IAsyncRelayCommand<string> GetSearchResultsAsyncCommand { get; }

        /// <summary>
        /// Gets autocomplete suggestions for a search query.
        /// </summary>
        public IRelayCommand<string> GetSearchAutoSuggestAsyncCommand { get; }

        /// <summary>
        /// Contains search results.
        /// </summary>
        public ObservableSearchResults? SearchResults { get; private set; }

        /// <summary>
        /// The autocomplete strings for the search results.
        /// </summary>
        public ObservableCollection<string> SearchAutoComplete { get; }

        /// <summary>
        /// The current playback queue. First item plays next.
        /// </summary>
        public ObservableCollection<ObservableTrack> PlaybackQueue { get; }
    }
}
