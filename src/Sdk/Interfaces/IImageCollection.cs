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
        /// <remarks>Data should be populated on object creation. Handle <see cref="SynchronizedObservableCollection{T}.CollectionChanged"/> to find out when an image is added or removed.</remarks>
        SynchronizedObservableCollection<IImage> Images { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IImage"/> at a specific position in <see cref="Images"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IImage"/> can be added.</returns>
        Task<bool> IsAddImageSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IImage"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IImage"/> can be removed.</returns>
        Task<bool> IsRemoveImageSupported(int index);
    }
}