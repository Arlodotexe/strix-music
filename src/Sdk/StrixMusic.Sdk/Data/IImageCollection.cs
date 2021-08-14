using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IImageCollectionBase"/>
    /// <remarks>This interface should be implemented in the Sdk.</remarks>
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