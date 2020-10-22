using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A collection of <see cref="IPlaylistCollectionItem"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlaylistCollection : IPlayableCollectionBase, IPlaylistCollectionItem
    {
        /// <summary>
        /// The total number of available Playlists.
        /// </summary>
        int TotalPlaylistItemsCount { get; }

        /// <summary>
        /// Adds a new playlist to the collection on the backend.
        /// </summary>
        /// <param name="playlist">The playlist to create.</param>
        /// <param name="index">the position to insert the playlist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index);

        /// <summary>
        /// Removes the playlist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the playlist to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemovePlaylistItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlaylistCollectionItem"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlaylistCollectionItem"/> can be added.</returns>
        Task<bool> IsAddPlaylistItemSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlaylistCollectionItem"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IPlaylistCollectionItem"/> can be removed.</returns>
        Task<bool> IsRemovePlaylistItemSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="IPlaylistCollectionItem"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset);
    }
}
