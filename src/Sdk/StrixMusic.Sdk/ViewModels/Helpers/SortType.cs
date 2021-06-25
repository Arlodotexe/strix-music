namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Enumeration for sort types.
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        UnOrdered,

        /// <summary>
        /// Sort tracks by ascending order(A-Z).
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort tracks in descending order(Z-A)
        /// </summary>
        Descending,

        /// <summary>
        /// Sort tracks by track number.
        /// </summary>
        TrackNumber,

        /// <summary>
        /// Sort track by index.
        /// </summary>
        AddedAt,

        /// <summary>
        /// Sort tracks by duration.
        /// </summary>
        Duration
    }
}