using System;

namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of tracks.
    /// </summary>
    [Flags]
    public enum TrackSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort tracks by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort tracks by track number.
        /// </summary>
        TrackNumber,

        /// <summary>
        /// Sort tracks by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort tracks by duration.
        /// </summary>
        Duration,
    }
}