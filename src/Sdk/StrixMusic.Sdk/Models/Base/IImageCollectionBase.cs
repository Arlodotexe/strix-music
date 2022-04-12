// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A common class for a collection of images.
    /// </summary>
    /// <seealso cref="IImageCollection"/>
    /// <seealso cref="ICoreImageCollection"/>
    public interface IImageCollectionBase : ICollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// Checks if adding a <see cref="IImageBase"/> to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IImageBase"/> can be added.</returns>
        Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if removing a <see cref="IImageBase"/> to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IImageBase"/> can be removed.</returns>
        Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the image from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the image to remove.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveImageAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// The total number of images in the collection.
        /// </summary>
        int TotalImageCount { get; }

        /// <summary>
        /// Fires when the merged number of images in the collection changes.
        /// </summary>
        event EventHandler<int>? ImagesCountChanged;
    }
}
