using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A collection of <see cref="ICoreArtistCollectionItem"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreArtistCollection : ICorePlayableCollection, IArtistCollectionBase, ICoreImageCollection, ICoreUrlCollection, ICoreArtistCollectionItem, ICoreMember
    {
        /// <summary>
        /// Attempts to play a specific item in the artist collection. Restarts playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem);

        /// <summary>
        /// Gets a requested number of <see cref="IArtist"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new artist to the collection on the backend.
        /// </summary>
        /// <param name="artist">The artist to create.</param>
        /// <param name="index">the position to insert the artist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;
    }
}
