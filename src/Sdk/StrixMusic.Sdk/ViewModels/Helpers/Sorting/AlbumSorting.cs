using System;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of tracks.
    /// </summary>
    [Flags]
    public enum AlbumSorting
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unordered = 0x0,

        /// <summary>
        /// Flags sorting in descending order
        /// </summary>
        Descending = 0x1,

        /// <summary>
        /// Sort albums by name.
        /// </summary>
        Alphanumerical = 0x2,

        /// <summary>
        /// Sort albums by date added to collection.
        /// </summary>
        DateAdded = 0x4,

        /// <summary>
        /// Sort albums by duration.
        /// </summary>
        Duration = 0x8,

        /// <summary>
        /// Sort albums by last played.
        /// </summary>
        LastPlayed = 0x16,

        /// <summary>
        /// Sort albums by date published.
        /// </summary>
        DatePublished = 0x32,
    }
}
