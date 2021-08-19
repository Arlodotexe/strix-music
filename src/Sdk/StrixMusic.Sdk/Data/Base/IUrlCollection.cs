using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="Uri"/>s.
    /// </summary>
    public interface IUrlCollection
    {
        /// <summary>
        /// Links to an external resource.
        /// </summary>
        /// <remarks>Data should be populated on object creation.</remarks>
        IReadOnlyList<Uri>? Urls { get; }

        /// <summary>
        /// Adds a new Url to the collection.
        /// </summary>
        /// <param name="url">The <see cref="Uri"/> to add.</param>
        /// <param name="index">The index to to put the Uri in.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddUrlAsync(Uri url, int index);

        /// <summary>
        /// Removes a Url from the collection at the given index.
        /// </summary>
        /// <param name="index">The index of the Uri to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveUrlAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="Uri"/> at a specific position in <see cref="Urls"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="Uri"/> can be added.</returns>
        Task<bool> IsAddUrlAvailableAsync(int index);

        /// <summary>
        /// Checks if the backend supports removing a <see cref="Uri"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="Uri"/> can be removed.</returns>
        Task<bool> IsRemoveUrlAvailableAsync(int index);

        /// <summary>
        /// Fires when <see cref="Urls"/> is changed.
        /// </summary>
        event CollectionChangedEventHandler<Uri>? UrlsChanged;
    }
}