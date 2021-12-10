using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IPlayableCollectionGroupChildrenBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlayableCollectionGroupChildren : IPlayableCollectionGroupChildrenBase, ICoreMember
    {
        /// <summary>
        /// Attempts to play a specific item in the playable collection group. Restarts playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup);

        /// <summary>
        /// Gets a requested number of <see cref="ICorePlayableCollectionGroup"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset);

        /// <summary>
        /// Adds a new child to the collection on the backend.
        /// </summary>
        /// <param name="child">The child to create.</param>
        /// <param name="index">the position to insert the child at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddChildAsync(ICorePlayableCollectionGroup child, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;
    }
}