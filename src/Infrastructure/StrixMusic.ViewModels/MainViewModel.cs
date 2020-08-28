using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Extensions;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.ViewModels.Bindables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
        public MainViewModel()
        {
            Devices = new ObservableCollection<BindableDevice>();
            SearchAutoComplete = new ObservableCollection<string>();

            LoadedCores = new ObservableCollection<BindableCore>();
            Users = new ObservableCollection<BindableUserProfile>();

            _cores = Ioc.Default.GetServices<ICore>().ToArray();

            GetSearchResultsAsyncCommand = new AsyncRelayCommand<string>(MultiSearchResultsAsync);
            GetSearchAutoSuggestAsyncCommand = new RelayCommand<string>(MultiSearchSuggestions);

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

        private void MultiSearchSuggestions(string query)
        {
            SearchAutoComplete.Clear();

            Parallel.ForEach(_cores, async core =>
            {
                await foreach (var item in await core.GetSearchAutoCompleteAsync(query))
                {
                    SearchAutoComplete.Add(item);
                }
            });
        }

        private async Task<ISearchResults> MultiSearchResultsAsync(string query)
        {
            var searchResults = await _cores.InParallel(core => Task.Run(() => core.GetSearchResultsAsync(query)));

            // TODO: Merge search results
            var merged = searchResults.First();

            SearchResult = new BindableSearchResults(merged);

            return merged;
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
        public ObservableCollection<BindableCore> LoadedCores { get; }

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<BindableUserProfile> Users { get; }

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
        public BindableCollectionGroup? Discoverables { get; private set; }

        /// <summary>
        /// Gets search results for a query.
        /// </summary>
        public IAsyncRelayCommand<string> GetSearchResultsAsyncCommand { get; }

        /// <summary>
        /// Gets autocompleted suggestions for a search query.
        /// </summary>
        public IRelayCommand<string> GetSearchAutoSuggestAsyncCommand { get; }

        /// <summary>
        /// Contains search results.
        /// </summary>
        public BindableSearchResults? SearchResult { get; private set; }

        /// <summary>
        /// The autocomplete strings for the search results.
        /// </summary>
        public ObservableCollection<string> SearchAutoComplete { get; private set; }
    }
}
