using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A collection of tracks and the properties and methods for using and manipulating them.
    /// </summary>
    public interface ITrackCollectionBase : IPlayableCollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of available Tracks.
        /// </summary>
        int TotalTrackCount { get; }

        /// <summary>
        /// If true, <see cref="PlayTrackCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPlayTrackCollectionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PauseTrackCollectionAsync()"/> can be used.
        /// </summary>
        bool IsPauseTrackCollectionAsyncAvailable { get; }

        /// <summary>
        /// Attempts to play the Track collection, or resumes playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayTrackCollectionAsync();

        /// <summary>
        /// Attempts to pause the Track collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseTrackCollectionAsync();

        /// <summary>
        /// Removes the track from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the track to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveTrackAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="ITrackBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="ITrackBase"/> can be added.</returns>
        Task<bool> IsAddTrackAvailableAsync(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="ITrackBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="ITrackBase"/> can be removed.</returns>
        Task<bool> IsRemoveTrackAvailableAsync(int index);

        /// <summary>
        /// Raised when <see cref="IsPlayTrackCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsPauseTrackCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <summary>
        /// Fires when the merged <see cref="TotalTrackCount"/> changes.
        /// </summary>
        event EventHandler<int>? TracksCountChanged;
    }
}