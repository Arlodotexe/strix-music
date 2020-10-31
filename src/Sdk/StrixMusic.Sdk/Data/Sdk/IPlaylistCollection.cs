using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IPlaylistCollectionBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlaylistCollection : IPlaylistCollectionBase, IPlaylistCollectionItem, ISdkMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="IPlaylistCollectionItemBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new playlist to the collection on the backend.
        /// </summary>
        /// <param name="playlist">The playlist to create.</param>
        /// <param name="index">the position to insert the playlist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index);
    }
}