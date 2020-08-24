using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Describes a core.
    /// </summary>
    public partial interface ICore : IAsyncDisposable
    {
        /// <inheritdoc cref="ICoreConfig" />
        public ICoreConfig CoreConfig { get; set; }

        /// <inheritdoc cref="CoreConfig.CoreState" />
        public CoreState CoreState { get; }

        /// <summary>
        /// The name of the core.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Represents the user that is authenticated with this core.
        /// </summary>
        public IUser User { get; }

        /// <summary>
        /// Gets available devices.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable"/> representing the asynchronous operation. Returns a list of devices.</returns>
        IAsyncEnumerable<IDevice> GetDevicesAsync();

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        /// <returns>A task representing the async operation. Returns <see cref="ILibrary"/> containing the library for this core.</returns>
        Task<ILibrary> GetLibraryAsync();

        /// <summary>
        /// Gets the recently played tracks for this core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IRecentlyPlayed> GetRecentlyPlayedAsync();

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable"/> representing the asynchronous operation. Containing multiple <see cref="IPlayableCollectionGroup"/>s representing discoverable music.</returns>
        IAsyncEnumerable<IPlayableCollectionGroup>? GetDiscoverablesAsync();

        /// <summary>
        /// Given a query, return suggested completed queries.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Suggested completed queries.</returns>
        Task<IAsyncEnumerable<string>> GetSearchAutoCompleteAsync(string query);

        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        Task<ISearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Initializes the <see cref="ICore"/> asyncronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitAsync();

        /// <summary>
        /// Fires when the <see cref="CoreState"/> has changed.
        /// </summary>
        event EventHandler<CoreState>? CoreStateChanged;

        /// <summary>
        /// Fires when the collection of devices is updated.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IDevice>>? DevicesChanged;

        /// <summary>
        /// Fires when the auto-completed suggested for a search is changed.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IAsyncEnumerable<string>>>? SearchAutoCompleteChanged;

        /// <summary>
        /// Fires when the <see cref="SearchResults"/> has changed.
        /// </summary>
        event EventHandler<ISearchResults>? SearchResultsChanged;
    }
}
