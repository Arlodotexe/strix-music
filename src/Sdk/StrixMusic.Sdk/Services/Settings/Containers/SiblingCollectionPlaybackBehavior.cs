namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// The behavior when the users clicks an collection to play from a collection.
    /// </summary>
    /// <example>From the library, the user requests that an album is played from a list of other albums.</example>
    public enum SiblingCollectionPlaybackBehavior
    {
        /// <summary>
        /// The selected collection plays. When completed, the next collection plays.
        /// </summary>
        AllCollections,

        /// <summary>
        /// The selected collection will play. When completed, playback ends.
        /// </summary>
        OnlySelectedCollection,
    }
}