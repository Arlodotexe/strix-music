using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A collection of <see cref="IArtistCollectionItem"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IArtistCollection : IPlayableCollectionBase, IArtistCollectionItem
    {
        /// <summary>
        /// The total number of available Artists.
        /// </summary>
        int TotalArtistItemsCount { get; }

        /// <summary>
        /// Adds a new artist to the collection on the backend.
        /// </summary>
        /// <param name="artist">The artist to create.</param>
        /// <param name="index">the position to insert the artist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddArtistItemAsync(IArtistCollectionItem artist, int index);

        /// <summary>
        /// Removes the artist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the artist to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveArtistAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IArtistCollectionItem"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IArtistCollectionItem"/> can be added.</returns>
        Task<bool> IsAddArtistSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IArtist"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IArtistCollectionItem"/> can be removed.</returns>
        Task<bool> IsRemoveArtistSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="IArtist"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IArtistCollectionItem> GetArtistsAsync(int limit, int offset);
    }
}
