// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.BaseModels
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
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if adding a genre to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new item can be added to the collection.</returns>
        Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if removing a genre to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the  item can be removed from the collection..</returns>
        Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the merged number of genres in the collection changes.
        /// </summary>
        event EventHandler<int>? GenresCountChanged;
    }
}
