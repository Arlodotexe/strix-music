using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="IPlaylistCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlaylistCollectionBase : IPlayableCollectionItem, IPlaylistCollectionItemBase
    {
        /// <summary>
        /// The total number of available Playlists.
        /// </summary>
        int TotalPlaylistItemsCount { get; }

        /// <summary>
        /// Removes the playlist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the playlist to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemovePlaylistItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlaylistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlaylistCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddPlaylistItemAvailable(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlaylistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IPlaylistCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemovePlaylistItemAvailable(int index);

        /// <summary>
        /// Fires when the merged <see cref="TotalPlaylistItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? PlaylistItemsCountChanged;
    }
}