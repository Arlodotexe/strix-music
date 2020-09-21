using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A read only album collection.
    /// </summary>
    public interface IAlbumCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// A collection of albums.
        /// </summary>
        SynchronizedObservableCollection<IAlbum> Albums { get; }

        /// <summary>
        /// The total number of available <see cref="Albums"/>.
        /// </summary>
        int TotalAlbumsCount { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IAlbum"/> at a specific position in <see cref="Albums"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IAlbum"/> can be added.</returns>
        Task<bool> IsAddAlbumSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Albums"/>. The bool at each index tells you if removing the <see cref="IAlbum"/> is supported.
        /// </summary>
        SynchronizedObservableCollection<bool> IsRemoveAlbumSupportedMap { get; }

        /// <summary>
        /// Returns items at a specific index and offset.
        /// </summary>
        /// <remarks>Does not affect <see cref="Albums"/>.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset);

        /// <summary>
        /// Populates more items into <see cref="Albums"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateMoreAlbumsAsync(int limit);
    }
}
