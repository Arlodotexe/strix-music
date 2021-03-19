using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of images.
    /// </summary>
    public interface IImageCollectionBase : ICollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// Checks if the backend supports adding an <see cref="IImageBase"/> at a specific position in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IImageBase"/> can be added.</returns>
        Task<bool> IsAddImageAvailable(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IImageBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IImageBase"/> can be removed.</returns>
        Task<bool> IsRemoveImageAvailable(int index);

        /// <summary>
        /// Removes the image from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the image to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveImageAsync(int index);

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