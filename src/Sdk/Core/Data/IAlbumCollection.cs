using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Defines properties and methods for using and manipulating a collection of albums.
    /// </summary>
    public interface IAlbumCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The total number of available Albums.
        /// </summary>
        int TotalAlbumsCount { get; }

        /// <summary>
        /// Adds a new album to the collection on the backend.
        /// </summary>
        /// <param name="album">The album to create.</param>
        /// <param name="index">the position to insert the album at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddAlbumAsync(IAlbum album, int index);

        /// <summary>
        /// Removes the album from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the album to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAlbumAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IAlbum"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IAlbum"/> can be added.</returns>
        Task<bool> IsAddAlbumSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IAlbum"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IAlbum"/> can be removed.</returns>
        Task<bool> IsRemoveAlbumSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="IAlbum"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset);
    }
}
