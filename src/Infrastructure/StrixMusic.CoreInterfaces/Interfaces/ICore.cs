using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Describes a core.
    /// </summary>
    public interface ICore
    {
        /// <inheritdoc cref="ICoreConfig" />
        public ICoreConfig CoreConfig { get; set; }

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
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        Task<ILibrary> GetLibraryAsync();

        /// <summary>
        /// Gets the recently played tracks for this core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. </returns>
        Task<IRecentlyPlayed> GetRecentlyPlayedAsync();

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable"/> representing the asynchronous operation. Containing multiple <see cref="IPlayableCollectionGroup"/>s representing discoverable music.</returns>
        IAsyncEnumerable<IPlayableCollectionGroup>? GetDiscoverablesAsync();

        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        Task<ISearchResults> GetSearchResults(string query);

        /// <summary>
        /// Given a query, return suggested completed queries.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Suggested completed queries.</returns>
        IAsyncEnumerable<string> GetSearchAutoComplete(string query);

        /// <summary>
        /// Initializes the <see cref="ICore"/> asyncronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Init();

        /// <inheritdoc />
        public CoreState CoreState { get; }
    }
}
