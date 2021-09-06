using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="IArtistCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IArtistCollectionBase : IPlayableCollectionBase, IArtistCollectionItemBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of available Artists.
        /// </summary>
        int TotalArtistItemsCount { get; }

        /// <summary>
        /// If true, <see cref="PlayArtistCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPlayArtistCollectionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PauseArtistCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPauseArtistCollectionAsyncAvailable { get; }

        /// <summary>
        /// Attempts to play the Artist collection, or resumes playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayArtistCollectionAsync();

        /// <summary>
        /// Attempts to pause the Artist collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseArtistCollectionAsync();

        /// <summary>
        /// Removes the artist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the artist to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveArtistItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IArtistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IArtistCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddArtistItemAvailableAsync(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IArtist"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IArtistCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemoveArtistItemAvailableAsync(int index);

        /// <summary>
        /// Raised when <see cref="IsPlayArtistCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsPauseArtistCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <summary>
        /// Fires when the merged <see cref="TotalArtistItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? ArtistItemsCountChanged;
    }
}