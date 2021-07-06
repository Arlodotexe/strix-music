using System;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of playlists.
    /// </summary>
    public enum PlaylistSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort playlists by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort playlists by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort playlists by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// Sort playlists by last played.
        /// </summary>
        LastPlayed,
    }
}
