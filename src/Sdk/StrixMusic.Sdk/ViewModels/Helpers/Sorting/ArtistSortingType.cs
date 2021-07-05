using System;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of artists.
    /// </summary>
    [Flags]
    public enum ArtistSortingType
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
        /// Sort artists by name.
        /// </summary>
        Alphanumerical = 0x2,

        /// <summary>
        /// Sort artists by track number.
        /// </summary>
        TrackNumber = 0x4,

        /// <summary>
        /// Sort track by date added to collection.
        /// </summary>
        DateAdded = 0x8,

        /// <summary>
        /// Sort artists by duration.
        /// </summary>
        Duration = 0x16,

        /// <summary>
        /// Sort artists by last played.
        /// </summary>
        LastPlayed = 0x32,
    }
}
