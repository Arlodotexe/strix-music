using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="IAlbumCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IAlbumCollectionBase : IPlayableCollectionBase, IAlbumCollectionItemBase
    {
        /// <summary>
        /// The total number of available Albums.
        /// </summary>
        int TotalAlbumItemsCount { get; }

        /// <summary>
        /// If true, <see cref="PlayAlbumCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPlayAlbumCollectionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PauseAlbumCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPauseAlbumCollectionAsyncAvailable { get; }

        /// <summary>
        /// Attempts to play the album collection, or resumes playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAlbumCollectionAsync();

        /// <summary>
        /// Attempts to pause the album collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseAlbumCollectionAsync();

        /// <summary>
        /// Removes the album from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the album to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAlbumItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IAlbumCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IAlbumCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddAlbumItemAvailable(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IAlbumCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IAlbumCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemoveAlbumItemAvailable(int index);

        /// <summary>
        /// Raised when <see cref="IsPlayAlbumCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsPauseAlbumCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <summary>
        /// Fires when the merged <see cref="TotalAlbumItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? AlbumItemsCountChanged;
    }
}