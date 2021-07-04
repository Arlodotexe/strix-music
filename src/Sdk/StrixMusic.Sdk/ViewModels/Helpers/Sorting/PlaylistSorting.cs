using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of playlists.
    /// </summary>
    public enum PlaylistSorting
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unordered,

        /// <summary>
        /// Sort albums by ascending order (A-Z).
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort albums in descending order (Z-A).
        /// </summary>
        Descending,

        /// <summary>
        /// Sort track by index.
        /// </summary>
        AddedAt,

        /// <summary>
        /// Sort albums by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// Sort albums by lastPlayed.
        /// </summary>
        LastPlayed,
    }
}
