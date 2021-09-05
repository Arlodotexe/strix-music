using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IAlbumCollectionBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IAlbumCollection : IAlbumCollectionBase, IAlbumCollectionItem, IImageCollection, IUrlCollection, ISdkMember, IMerged<ICoreAlbumCollection>
    {
        /// <summary>
        /// Attempts to play a specific item in the album collection. Restarts playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem);

        /// <summary>
        /// Gets a requested number of <see cref="IAlbumCollectionItemBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new album to the collection on the backend.
        /// </summary>
        /// <param name="album">The album to create.</param>
        /// <param name="index">the position to insert the album at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddAlbumItemAsync(IAlbumCollectionItem album, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;
    }
}
