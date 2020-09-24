using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A playlist collection.
    /// </summary>
    public interface IPlaylistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The total number of available Playlists.
        /// </summary>
        int TotalPlaylistCount { get; }

        /// <summary>
        /// Adds a new playlist to the collection on the backend.
        /// </summary>
        /// <param name="playlist">The playlist to create.</param>
        /// <param name="index">the position to insert the playlist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddPlaylistAsync(IPlayableCollectionGroup playlist, int index);

        /// <summary>
        /// Removes the playlist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the playlist to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemovePlaylistAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlaylist"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlaylist"/> can be added.</returns>
        Task<bool> IsAddPlaylistSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlaylist"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IPlaylist"/> can be removed.</returns>
        Task<bool> IsRemovePlaylistSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="IPlaylist"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset);

        /// <summary>
        /// Fires when a <see cref="IPlaylist"/> in this collection is added or removed in the backend.
        /// </summary>
        /// <remarks>This is used to handle real time changes from the backend, if supported by the core.</remarks>
        event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;
    }
}
