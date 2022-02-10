using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A common interface for all collections that return genres.
    /// </summary>
    public interface IGenreCollectionBase : ICollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of genres in this collection.
        /// </summary>
        int TotalGenreCount { get; }

        /// <summary>
        /// Removes a genre from the collection.
        /// </summary>
        /// <param name="index">the position remove the genre from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveGenreAsync(int index);

        /// <summary>
        /// Checks if adding a genre to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new item can be added to the collection.</returns>
        Task<bool> IsAddGenreAvailableAsync(int index);

        /// <summary>
        /// Checks if removing a genre to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the  item can be removed from the collection..</returns>
        Task<bool> IsRemoveGenreAvailableAsync(int index);

        /// <summary>
        /// Fires when the merged number of genres in the collection changes.
        /// </summary>
        event EventHandler<int>? GenresCountChanged;
    }
}