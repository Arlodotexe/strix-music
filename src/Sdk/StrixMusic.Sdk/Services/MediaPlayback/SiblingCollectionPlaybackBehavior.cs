namespace StrixMusic.Sdk.Services.MediaPlayback
{
    /// <summary>
    /// The behavior when the users clicks an collection to play from a collection.
    /// </summary>
    /// <example>From the library, the user requests to requests that an album is played. This decides</example>
    public enum SiblingCollectionPlaybackBehavior
    {
        /// <summary>
        /// The selected collection will play. When completed, playback ends.
        /// </summary>
        OnlySelectedCollection,

        /// <summary>
        /// The selected collection plays. When completed, the next collection plays.
        /// </summary>
        AllCollections,
    }
}