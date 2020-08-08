using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// An album collection.
    /// </summary>
    public interface IAlbumCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The albums in the library.
        /// </summary>
        IReadOnlyList<IAlbum> Albums { get; }

        /// <summary>
        /// The total number of albums in this collection.
        /// </summary>
        int TotalAlbumsCount { get; }

        /// <summary>
        /// Populates the <see cref="IArtist.Albums"/> in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateAlbums(int limit, int offset = 0);
    }
}
