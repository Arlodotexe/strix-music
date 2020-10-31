using OwlCore.Collections;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IImageCollectionBase" />
    /// <remarks>This interface should be implemented in a core.</remarks>
    public interface ICoreImageCollection : IImageCollectionBase, ICoreMember
    {
        /// <summary>
        /// Relevant images for the collection.
        /// </summary>
        /// <remarks>Data should be populated on object creation. Handle <see cref="SynchronizedObservableCollection{T}.CollectionChanged"/> to find out when an image is added or removed.</remarks>
        SynchronizedObservableCollection<ICoreImage> Images { get; }
    }
}