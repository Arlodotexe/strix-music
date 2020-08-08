using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// A playlist collection.
    /// </summary>
    public interface IPlaylistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The playlists in the library.
        /// </summary>
        IReadOnlyList<IPlaylist> Playlists { get; }

        /// <summary>
        /// The total number of playlists in this collection.
        /// </summary>
        int TotalPlaylistCount { get; }

        /// <summary>
        /// Populates the <see cref="IPlaylist"/> in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulatePlaylists(int limit, int offset = 0);
    }
}
