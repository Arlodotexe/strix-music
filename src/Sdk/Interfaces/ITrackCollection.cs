using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A read only track collection.
    /// </summary>
    public interface ITrackCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The tracks for this artists. If unknown, returns the most listened to tracks for this user. Can be empty.
        /// </summary>
        SynchronizedObservableCollection<ITrack> Tracks { get; }

        /// <summary>
        /// The total number of available <see cref="Tracks"/>.
        /// </summary>
        int TotalTracksCount { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="ITrack"/> at a specific position in <see cref="ITrackCollection.Tracks"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="ITrack"/> can be added.</returns>
        Task<bool> IsAddTrackSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="ITrackCollection.Tracks"/>. The bool at each index tells you if removing the <see cref="ITrack"/> is supported.
        /// </summary>
        SynchronizedObservableCollection<bool> IsRemoveTrackSupportedMap { get; }

        /// <summary>
        /// Returns items at a specific index and offset.
        /// </summary>
        /// <remarks>Does not affect <see cref="Tracks"/>.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset);

        /// <summary>
        /// Populates more items into <see cref="Tracks"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateMoreTracksAsync(int limit);
    }
}