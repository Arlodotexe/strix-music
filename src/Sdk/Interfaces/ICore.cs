using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;

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
        public IReadOnlyList<IDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ILibrary Library { get; }

        /// <summary>
        /// Gets the recently played items for this core.
        /// </summary>
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public IPlayableCollectionGroup Discoverables { get; }

        /// <summary>
        /// Given a query, return suggested completed queries.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Suggested completed queries.</returns>
        public Task<IReadOnlyList<string>?> GetSearchAutoCompleteAsync(string query);

        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        public Task<ISearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Initializes the <see cref="ICore"/> asyncronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAsync();

        /// <summary>
        /// Fires when the <see cref="CoreState"/> has changed.
        /// </summary>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <summary>
        /// Fires when the collection of devices is updated.
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<IDevice>>? DevicesChanged;
    }
}
