namespace StrixMusic.Sdk.ViewModels.Helpers.Sorting
{
    /// <summary>
    /// Enumeration for sort types of tracks.
    /// </summary>
    public enum AlbumSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort albums by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort albums by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort albums by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// Sort albums by last played.
        /// </summary>
        LastPlayed,

        /// <summary>
        /// Sort albums by date published.
        /// </summary>
        DatePublished,
    }
}
