using System;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of playlists.
    /// </summary>
    [Flags]
    public enum PlaylistSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unordered = 0,

        /// <summary>
        /// Flags sorting in descending order.
        /// </summary>
        Descending = 0x1,

        /// <summary>
        /// Sort playlists by name.
        /// </summary>
        Alphanumerical = 0x2,

        /// <summary>
        /// Sort playlists by date added to collection.
        /// </summary>
        DateAdded = 0x4,

        /// <summary>
        /// Sort playlists by duration.
        /// </summary>
        Duration = 0x8,

        /// <summary>
        /// Sort playlists by last played.
        /// </summary>
        LastPlayed = 0x16,
    }
}
