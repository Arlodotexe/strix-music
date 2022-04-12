// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A collection of images.
    /// </summary>
    /// <remarks>This interface should be implemented in a core.</remarks>
    public interface ICoreImageCollection : IImageCollectionBase, ICoreCollection, ICoreMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="IImageBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new image to the collection on the backend.
        /// </summary>
        /// <param name="image">The image to create.</param>
        /// <param name="index">the position to insert the image at.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
    }
}
