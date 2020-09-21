using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using StrixMusic.Sdk.Enums;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Describes a core.
    /// </summary>
    public interface ICore : IAsyncDisposable
    {
        /// <inheritdoc cref="ICoreConfig" />
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc cref="Enums.CoreState" />
        public CoreState CoreState { get; }

        /// <summary>
        /// Identifies this instance of the core.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The name of the core.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Represents the user that is authenticated with this core.
        /// </summary>
        public IUser User { get; }

        /// <summary>
        /// The available devices.
        /// </summary>
        public ObservableCollection<IDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ILibrary Library { get; }

        /// <summary>
        /// A list of pinned playable items.
        /// </summary>
        public ObservableCollection<IPlayable> Pins { get; }

        /// <summary>
        /// Gets the recently played items for this core.
        /// </summary>
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public IDiscoverables Discoverables { get; }

        /// <summary>
        /// Given a query, return suggested completed queries.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Suggested completed queries.</returns>
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query);

        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        public Task<ISearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Initializes the <see cref="ICore"/> asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAsync();

        /// <summary>
        /// Fires when the <see cref="CoreState"/> has changed.
        /// </summary>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlayable"/> at a specific position in <see cref="Pins"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlayable"/> can be added.</returns>
        Task<bool> IsAddPinSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Pins"/>. The bool at each index tells you if removing the <see cref="IPlayable"/> is supported.
        /// </summary>
        ObservableCollection<bool> IsRemovePinSupportedMap { get; }

        ///<summary>
        /// Gets the object against a context.
        /// </summary>
        /// <returns>Returns an IPlayable object</returns>
        Task<object?> GetContextById(string? id);
    }
}
