using System;
using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A collection of <see cref="Uri"/>s.
    /// </summary>
    public interface IUrlCollection : ICoreMember
    {
        /// <summary>
        /// Links to an external resource.
        /// </summary>
        /// <remarks>Data should be populated on object creation. Handle <see cref="SynchronizedObservableCollection{T}.CollectionChanged"/> to find out when an <see cref="Uri"/> is added or removed.</remarks>
        SynchronizedObservableCollection<Uri>? Urls { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="Uri"/> at a specific position in <see cref="Urls"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="Uri"/> can be added.</returns>
        Task<bool> IsAddUrlSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing a <see cref="Uri"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="Uri"/> can be removed.</returns>
        Task<bool> IsRemoveUrlSupported(int index);
    }
}