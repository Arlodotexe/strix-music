using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// An album collection.
    /// </summary>
    public interface IAlbumCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// A collection of albums.
        /// </summary>
        IReadOnlyList<IAlbum> Albums { get; }

        /// <summary>
        /// The total number of available <see cref="Albums"/>.
        /// </summary>
        int TotalAlbumsCount { get; }

        /// <summary>
        /// Populates the <see cref="IAlbumCollection.Albums"/> in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0);

        /// <summary>
        /// Fires when <see cref="Albums"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;
    }
}
