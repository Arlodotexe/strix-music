using System;
using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A collection of <see cref="Uri"/>s.
    /// </summary>
    public interface IUrlCollection
    {
        /// <summary>
        /// Links to an external resource.
        /// </summary>
        SynchronizedObservableCollection<Uri>? Urls { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="Uri"/> at a specific position in <see cref="Urls"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="Uri"/> can be added.</returns>
        Task<bool> IsAddUrlSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Urls"/>. The bool at each index tells you if removing the <see cref="Uri"/> is supported.
        /// </summary>
        SynchronizedObservableCollection<bool> IsRemoveUrlSupportedMap { get; }
    }
}