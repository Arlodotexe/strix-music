using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A collection of artists.
    /// </summary>
    public interface IArtistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The artists in the library.
        /// </summary>
        IReadOnlyList<IArtist> Artists { get; }

        /// <summary>
        /// Populates a set of <see cref="Artists"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0);

        /// <summary>
        /// The total number of available <see cref="Artists"/>.
        /// </summary>
        int TotalArtistsCount { get; }

        /// <summary>
        /// Fires when <see cref="Artists"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;
    }
}
