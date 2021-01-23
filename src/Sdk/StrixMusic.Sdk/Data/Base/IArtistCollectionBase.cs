using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A collection of <see cref="IArtistCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IArtistCollectionBase : IPlayableCollectionBase, IArtistCollectionItemBase
    {
        /// <summary>
        /// The total number of available Artists.
        /// </summary>
        int TotalArtistItemsCount { get; }

        /// <summary>
        /// Removes the artist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the artist to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveArtistItemAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IArtistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IArtistCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddArtistItemAvailable(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IArtist"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IArtistCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemoveArtistItemAvailable(int index);

        /// <summary>
        /// Fires when the merged <see cref="TotalArtistItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? ArtistItemsCountChanged;
    }
}