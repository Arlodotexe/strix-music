using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Represents multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroup : IPlayableCollectionBase, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection
    {
        /// <summary>
        /// The <see cref="IPlayableCollectionBase"/>s in this collection group.
        /// </summary>
        IReadOnlyList<IPlayableCollectionGroup> Children { get; }

        /// <summary>
        /// The total number of available <see cref="Children"/>.
        /// </summary>
        int TotalChildrenCount { get; }

        /// <summary>
        /// Populates a set of <see cref="Children"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0);

        /// <summary>
        /// Fires when <see cref="Children"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> ChildrenChanged;
    }
}
