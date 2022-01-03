using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IImageCollectionBase" />
    /// <remarks>This interface should be implemented in a core.</remarks>
    public interface ICoreImageCollection : IImageCollectionBase, ICoreCollection, ICoreMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="IImageBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset);

        /// <summary>
        /// Adds a new image to the collection on the backend.
        /// </summary>
        /// <param name="image">The image to create.</param>
        /// <param name="index">the position to insert the image at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddImageAsync(ICoreImage image, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
    }
}