using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Events;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Metadata about genres.
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="string"/> can be added to the collection.</returns>
        Task<bool> IsAddGenreAvailable(int index);

        /// <summary>
        /// Checks if removing a genre to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="string"/> can be removed from the collection..</returns>
        Task<bool> IsRemoveGenreAvailable(int index);

        /// <summary>
        /// Fires when the merged number of genres in the collection changes.
        /// </summary>
        event EventHandler<int>? GenresCountChanged;
    }
}