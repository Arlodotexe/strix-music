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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Returns a list of devices.</returns>
        Task<IList<IDevice>> GetDevices();

        /// <summary>
        /// Fires when a new device appears.
        /// </summary>
        event EventHandler<IDevice> DeviceAdded;

        /// <summary>
        /// Fires when a device dissapears.
        /// </summary>
        event EventHandler<IDevice> DeviceRemoved;

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        Task<IPlayableCollectionGroup> GetLibrary();

        /// <summary>
        /// Emits when the library changes.
        /// </summary>
        event EventHandler<IPlayableCollectionGroup>? LibraryChanged;

        /// <summary>
        /// Gets the recently played tracks for this core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. </returns>
        Task<IPlayableCollectionGroup> GetRecentlyPlayed();

        /// <summary>
        /// Emits when the recently played tracks change.
        /// </summary>
        event EventHandler<IPlayableCollectionGroup>? RecentlyPlayedChanged;

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Containing multiple <see cref="IPlayableCollectionGroup"/>s representing discoverable music.</returns>
        Task<IList<IPlayableCollectionGroup>?> GetDiscoverables();

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
        Task<IEnumerable<string>> GetSearchAutoComplete(string query);
    }
}
