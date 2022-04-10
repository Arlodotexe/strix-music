// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A common interface for all collections that return urls.
    /// </summary>
    public interface IUrlCollectionBase : ICollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of urls in this collection.
        /// </summary>
        int TotalUrlCount { get; }

        /// <summary>
        /// Removes a url from the collection.
        /// </summary>
        /// <param name="index">the position remove the url from.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if adding a url to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new item can be added to the collection.</returns>
        Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if removing a url to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the item can be removed from the collection..</returns>
        Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the merged number of urls in the collection changes.
        /// </summary>
        event EventHandler<int>? UrlsCountChanged;
    }
}
