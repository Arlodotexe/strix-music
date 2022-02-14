// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A collection of images.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IImageCollection : IImageCollectionBase, ISdkMember, IMerged<ICoreImageCollection>
    {
        /// <summary>
        /// Gets a requested number of <see cref="IImageBase"/>s starting at the given offset.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset);

        /// <summary>
        /// Adds a new image to the collection.
        /// </summary>
        /// <param name="image">The image to add.</param>
        /// <param name="index">the position to insert the image at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddImageAsync(IImage image, int index);

        /// <summary>
        /// Fires when the items are changed.
        /// </summary>
        event CollectionChangedEventHandler<IImage>? ImagesChanged;
    }
}