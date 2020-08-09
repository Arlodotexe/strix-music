using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public MainViewModel(ISettingsService settings)
        {
            IEnumerable<ICore> cores = Ioc.Default.GetServices<ICore>();

            _ = InitializeCores(cores);
        }

        private async Task InitializeCores(IEnumerable<ICore> coresToLoad)
        {
            await coresToLoad.InParallel(core => Task.Run(core.Init));

            foreach (ICore core in coresToLoad)
            {
                var library = await core.GetLibraryAsync();
            }
        }

        private async Task LoadLibrary(IEnumerable<ICore> cores)
        {

        }

        /// <summary>
        /// A consolidated list of all users in the app
        /// </summary>
        public ObservableCollection<IUser>? Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<IDevice>? Devices { get; }

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public BindableCollectionGroup? Library { get; set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public BindableCollectionGroup? RecentlyPlayed { get; set; }

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
    }
}
