namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of tracks.
    /// </summary>
    public enum TrackSorting
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
        /// Sort tracks by name.
        /// </summary>
        Alphanumerical = 0x2,

        /// <summary>
        /// Sort tracks by track number.
        /// </summary>
        TrackNumber = 0x4,

        /// <summary>
        /// Sort tracks by date added to collection.
        /// </summary>
        DateAdded = 0x8,

        /// <summary>
        /// Sort tracks by duration.
        /// </summary>
        Duration,
    }
}