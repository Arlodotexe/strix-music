namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Enumeration for sort types of artists.
    /// </summary>
    public enum ArtistSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort artists by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort artists by track number.
        /// </summary>
        TrackNumber,

        /// <summary>
        /// Sort track by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort artists by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// Sort artists by last played.
        /// </summary>
        LastPlayed,
    }
}
