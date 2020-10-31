using OwlCore.Collections;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IImageCollectionBase"/>
    /// <remarks>This interface should be implemented in the Sdk.</remarks>
    public interface IImageCollection : IImageCollectionBase, ISdkMember
    {
        /// <summary>
        /// Relevant images for the collection.
        /// </summary>
        /// <remarks>Data should be populated on object creation. Handle <see cref="SynchronizedObservableCollection{T}.CollectionChanged"/> to find out when an image is added or removed.</remarks>
        SynchronizedObservableCollection<IImage> Images { get; }
    }
}