using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroup : IPlayableCollectionBase, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection
    {
        /// <summary>
        /// The <see cref="IPlayableCollectionBase"/>s in this Group.
        /// </summary>
        IReadOnlyList<IPlayableCollectionGroup> SubItems { get; }

        /// <summary>
        /// The number of <see cref="SubItems"/> in this collection.
        /// </summary>
        int TotalItemsCount { get; }

        /// <summary>
        /// Populates a set of <see cref="SubItems"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateItems(int limit, int offset = 0);

        /// <summary>
        /// The source collections that this was merged from. Null is the collection wasn't merged.
        /// </summary>
        IReadOnlyList<IPlayableCollectionGroup>? MergedFrom { get; }
    }
}
