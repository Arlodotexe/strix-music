using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of artists.
    /// </summary>
    public enum ArtistSorting
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unordered,

        /// <summary>
        /// sort artists by ascending order (A-Z).
        /// </summary>
        Ascending,

        /// <summary>
        /// sort artists in descending order (Z-A).
        /// </summary>
        Descending,

        /// <summary>
        /// sort artists by track number.
        /// </summary>
        TrackNumber,

        /// <summary>
        /// Sort track by index.
        /// </summary>
        AddedAt,

        /// <summary>
        /// sort artists by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// sort artists by lastPlayed.
        /// </summary>
        LastPlayed,
    }
}
