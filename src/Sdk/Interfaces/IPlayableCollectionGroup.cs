using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Represents multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroup : IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection
    {
        /// <summary>
        /// The <see cref="IPlayableCollectionBase"/>s in this collection group.
        /// </summary>
        ObservableCollection<IPlayableCollectionGroup> Children { get; }

        /// <summary>
        /// The total number of available <see cref="Children"/>.
        /// </summary>
        int TotalChildrenCount { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlayableCollectionGroup"/> at a specific position in <see cref="Children"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, an item can be added.</returns>
        Task<bool> IsAddChildSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Children"/>. The bool at each index tells you if removing the <see cref="IPlayableCollectionGroup"/> is supported.
        /// </summary>
        ObservableCollection<bool> IsRemoveChildSupportedMap { get; }

        /// <summary>
        /// Returns items at a specific index and offset.
        /// </summary>
        /// <remarks>Does not affect <see cref="Children"/>.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset);

        /// <summary>
        /// Populates the <see cref="Children"/> in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateMoreChildrenAsync(int limit);
    }
}
