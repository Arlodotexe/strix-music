using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A collection of <see cref="IPlayableCollectionGroup"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IPlayableCollectionGroupChildren : IPlayableCollectionGroupChildrenBase, IPlayable, ISdkMember, IMerged<ICorePlayableCollectionGroupChildren>
    {
        /// <summary>
        /// Attempts to play a specific item in the playable collection group. Restarts playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup);

        /// <summary>
        /// Gets a requested number of <see cref="IPlayableCollectionGroupBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset);

        /// <summary>
        /// Adds a new child to the collection on the backend.
        /// </summary>
        /// <param name="child">The child to create.</param>
        /// <param name="index">the position to insert the child at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddChildAsync(IPlayableCollectionGroup child, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged;
    }
}