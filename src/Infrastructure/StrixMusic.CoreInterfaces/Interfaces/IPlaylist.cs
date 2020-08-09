using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylist : IPlayableCollectionBase
    {
        /// <summary>
        /// List of <see cref="ITrack"/>s that this collection contains.
        /// </summary>
        IReadOnlyList<ITrack> Tracks { get; }

        /// <summary>
        /// The total number of <see cref="IPlaylist.Tracks"/> in this collection.
        /// </summary>
        int TotalTrackCount { get; set; }

        /// <summary>
        /// Populates a set of <see cref="IPlayableCollectionGroup.SubItems"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateItems(int limit, int offset = 0);
    }
}
