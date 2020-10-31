using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A collection of images.
    /// </summary>
    public interface IImageCollectionBase
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
    }
}