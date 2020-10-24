using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A collection of <see cref="IAlbumCollectionItem"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IAlbumCollection : IPlayableCollectionBase, IAlbumCollectionItem
    {
        /// <summary>
        /// The total number of available Albums.
        /// </summary>
        int TotalAlbumItemsCount { get; }

        /// <summary>
        /// Adds a new album to the collection on the backend.
        /// </summary>
        /// <param name="album">The album to create.</param>
        /// <param name="index">the position to insert the album at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddAlbumItemAsync(IAlbumCollectionItem album, int index);

        /// <summary>
        /// Removes the album from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the album to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAlbumItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IAlbumCollectionItem"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IAlbumCollectionItem"/> can be added.</returns>
        Task<bool> IsAddAlbumItemSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IAlbumCollectionItem"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IAlbumCollectionItem"/> can be removed.</returns>
        Task<bool> IsRemoveAlbumItemSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="IAlbumCollectionItem"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset);
    }
}
