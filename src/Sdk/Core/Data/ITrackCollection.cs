using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Defines properties and methods for using and manipulating a collection of tracks.
    /// </summary>
    public interface ITrackCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The total number of available Tracks.
        /// </summary>
        int TotalTracksCount { get; }

        /// <summary>
        /// Adds a new track to the collection on the backend.
        /// </summary>
        /// <param name="track">The track to create.</param>
        /// <param name="index">the position to insert the track at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddTrackAsync(ITrack track, int index);

        /// <summary>
        /// Removes the track from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the track to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveTrackAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="ITrack"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="ITrack"/> can be added.</returns>
        Task<bool> IsAddTrackSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="ITrack"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="ITrack"/> can be removed.</returns>
        Task<bool> IsRemoveTrackSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="ITrack"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset);
    }
}