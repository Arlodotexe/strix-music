using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of tracks and the properties and methods for using and manipulating them.
    /// </summary>
    public interface ITrackCollectionBase : IPlayableCollectionBase
    {
        /// <summary>
        /// The total number of available Tracks.
        /// </summary>
        int TotalTracksCount { get; }

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
        Task<bool> IsAddTrackSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="ITrackBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="ITrackBase"/> can be removed.</returns>
        Task<bool> IsRemoveTrackSupported(int index);

        /// <summary>
        /// Fires when the merged <see cref="ITrackCollectionBase.TotalTracksCount"/> changes.
        /// </summary>
        event EventHandler<int>? TrackItemsCountChanged;
    }
}