using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IArtistCollectionBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IArtistCollection : IArtistCollectionBase, IArtistCollectionItem, IImageCollection, ISdkMember<ICoreArtistCollection>
    {
        /// <summary>
        /// Gets a requested number of <see cref="IArtist"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new artist to the collection on the backend.
        /// </summary>
        /// <param name="artist">The artist to create.</param>
        /// <param name="index">the position to insert the artist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddArtistItemAsync(IArtistCollectionItem artist, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;
    }
}