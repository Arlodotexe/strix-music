using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A collection of <see cref="IPlayableCollectionGroup"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlayableCollectionGroupChildren : IPlayableCollectionBase
    {
        /// <summary>
        /// The total number of available Children.
        /// </summary>
        int TotalChildrenCount { get; }

        /// <summary>
        /// Adds a new child to the collection on the backend.
        /// </summary>
        /// <param name="child">The child to create.</param>
        /// <param name="index">the position to insert the child at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddChildAsync(IPlayableCollectionGroup child, int index);

        /// <summary>
        /// Removes the child from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the child to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveChildAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlayableCollectionGroup"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, an item can be added.</returns>
        Task<bool> IsAddChildSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlayableCollectionGroup"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the item can be removed.</returns>
        Task<bool> IsRemoveChildSupported(int index);

        /// <summary>
        /// Gets a requested number of <see cref="IPlayableCollectionGroup"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset);
    }
}