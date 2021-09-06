using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="IPlaylistCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlaylistCollectionBase : IPlayableCollectionItem, IPlaylistCollectionItemBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of available Playlists.
        /// </summary>
        int TotalPlaylistItemsCount { get; }

        /// <summary>
        /// If true, <see cref="PlayPlaylistCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPlayPlaylistCollectionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PausePlaylistCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPausePlaylistCollectionAsyncAvailable { get; }

        /// <summary>
        /// Attempts to play the Playlist collection, or resumes playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlaylistCollectionAsync();

        /// <summary>
        /// Attempts to pause the Playlist collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PausePlaylistCollectionAsync();

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
        Task<bool> IsAddPlaylistItemAvailableAsync(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlaylistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IPlaylistCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemovePlaylistItemAvailableAsync(int index);

        /// <summary>
        /// Raised when <see cref="IsPlayPlaylistCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsPausePlaylistCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

        /// <summary>
        /// Fires when the merged <see cref="TotalPlaylistItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? PlaylistItemsCountChanged;
    }
}