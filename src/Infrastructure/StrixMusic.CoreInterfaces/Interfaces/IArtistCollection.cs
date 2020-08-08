using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// A collection of artists.
    /// </summary>
    public interface IArtistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The artists in the library.
        /// </summary>
        IReadOnlyList<IArtist> Artists { get; }

        /// <summary>
        /// Populates a set of <see cref="IArtist.Albums"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateArtists(int limit, int offset = 0);

        /// <summary>
        /// The total number of artists in this collection.
        /// </summary>
        int TotalArtistsCount { get; }
    }
}
