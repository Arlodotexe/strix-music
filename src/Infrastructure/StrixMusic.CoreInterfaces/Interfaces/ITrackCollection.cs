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
        /// The total number of top tracks in this collection.
        /// </summary>
        int TotalTracksCount { get; }

        /// <summary>
        /// Populates a set of <see cref="IArtist.TopTracks"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateTracks(int limit, int offset = 0);
    }
}
