using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
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
        /// Populates the <see cref="IArtist.Albums"/> in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateAlbumsAsync(int limit, int offset = 0);

        /// <summary>
        /// Fires when <see cref="Albums"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;
    }
}
