using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// A track collection.
    /// </summary>
    public interface ITrackCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The tracks for this artists. If unknown, returns the most listened to tracks for this user. Can be empty.
        /// </summary>
        IReadOnlyList<ITrack> Tracks { get; }

        /// <summary>
        /// The total number of available <see cref="Tracks"/>.
        /// </summary>
        int TotalTracksCount { get; }

        /// <summary>
        /// Populates a set of <see cref="IArtist.TopTracks"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateTracksAsync(int limit, int offset = 0);

        /// <summary>
        /// Fires when <see cref="Tracks"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;
    }
}
