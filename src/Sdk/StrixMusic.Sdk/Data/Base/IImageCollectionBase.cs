using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of images.
    /// </summary>
    public interface IImageCollectionBase : ICollectionBase
    {
        /// <summary>
        /// Checks if the backend supports adding an <see cref="IImageBase"/> at a specific position in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IImageBase"/> can be added.</returns>
        Task<bool> IsAddImageSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IImageBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IImageBase"/> can be removed.</returns>
        Task<bool> IsRemoveImageSupported(int index);

        /// <summary>
        /// The total number of images in the collection.
        /// </summary>
        int TotalImageCount { get; set; }

        /// <summary>
        /// Fires when the merged number of images in the collection changes.
        /// </summary>
        event EventHandler<int> ImagesCountChanged;
    }
}