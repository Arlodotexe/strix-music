using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="IAlbumCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IAlbumCollectionBase : IPlayableCollectionBase, IAlbumCollectionItemBase
    {
        /// <summary>
        /// The total number of available Albums.
        /// </summary>
        int TotalAlbumItemsCount { get; }

        /// <summary>
        /// Removes the album from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the album to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAlbumItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IAlbumCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IAlbumCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddAlbumItemSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IAlbumCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IAlbumCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemoveAlbumItemSupported(int index);

        /// <summary>
        /// Fires when the merged <see cref="TotalAlbumItemsCount"/> changes.
        /// </summary>
        event EventHandler<int> AlbumItemsCountChanged;
    }
}