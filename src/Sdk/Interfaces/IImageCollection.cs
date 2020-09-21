using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A collection of images, which could be changeable from the backend.
    /// </summary>
    public interface IImageCollection
    {
        /// <summary>
        /// Relevant images for the collection.
        /// </summary>
        SynchronizedObservableCollection<IImage> Images { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IImage"/> at a specific position in <see cref="Images"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IImage"/> can be added.</returns>
        Task<bool> IsAddImageSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Images"/>. The bool at each index tells you if removing the <see cref="IImage"/> is supported.
        /// </summary>
        SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap { get; }
    }
}