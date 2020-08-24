using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an artist.
    /// </summary>
    public interface IArtist : IAlbumCollection, ITrackCollection
    {
        /// <summary>
        /// The artists in the library.
        /// </summary>
        IReadOnlyList<IArtist> RelatedArtists { get; }

        /// <summary>
        /// Populates a set of <see cref="IArtist.Albums"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateRelatedArtistsAsync(int limit, int offset = 0);

        /// <summary>
        /// The total number of available <see cref="RelatedArtists"/>.
        /// </summary>
        int TotalRelatedArtistsCount { get; }

        /// <summary>
        /// Fires when <see cref="RelatedArtists"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IArtist>>? RelatedArtistsChanged;
    }
}
