using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an artist.
    /// </summary>
    public interface IArtist : IPlayableCollectionBase
    {
        /// <summary>
        /// The Discography for the Artist.
        /// </summary>
        IReadOnlyList<IAlbum> Albums { get; }

        /// <summary>
        /// The top tracks for this artists. If unknown, returns the most listened to tracks for this user. Can be empty.
        /// </summary>
        IReadOnlyList<ITrack> TopTracks { get; }

        /// <summary>
        /// Artists that are related to this one. Can be empty.
        /// </summary>
        IReadOnlyList<IArtist> RelatedArtists { get; }

        /// <summary>
        /// The total number of songs in this collection.
        /// </summary>
        int TotalAlbumsCount { get; }

        /// <summary>
        /// The total number of songs in this collection.
        /// </summary>
        int TotalTopTracksCount { get; }

        /// <summary>
        /// The total number of songs in this collection.
        /// </summary>
        int TotalRelatedArtistsCount { get; }
    }
}
