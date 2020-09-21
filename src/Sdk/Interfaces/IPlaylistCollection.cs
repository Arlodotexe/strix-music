using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A playlist collection.
    /// </summary>
    public interface IPlaylistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The playlists in the library.
        /// </summary>
        SynchronizedObservableCollection<IPlaylist> Playlists { get; }

        /// <summary>
        /// The total number of available <see cref="Playlists"/>.
        /// </summary>
        int TotalPlaylistCount { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlaylist"/> at a specific position in <see cref="Playlists"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlaylist"/> can be added.</returns>
        Task<bool> IsAddPlaylistSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Playlists"/>. The bool at each index tells you if removing the <see cref="IPlaylist"/> is supported.
        /// </summary>
        SynchronizedObservableCollection<bool> IsRemovePlaylistSupportedMap { get; }

        /// <summary>
        /// Returns items at a specific index and offset.
        /// </summary>
        /// <remarks>Does not affect <see cref="Playlists"/>.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset);

        /// <summary>
        /// Populates more items into <see cref="Playlists"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateMorePlaylistsAsync(int limit);
    }
}
