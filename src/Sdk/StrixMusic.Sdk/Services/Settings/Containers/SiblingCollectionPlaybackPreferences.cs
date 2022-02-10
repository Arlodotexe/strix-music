namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// User preferences for behavior when the user requests to play a single item from a collection of collections.
    /// </summary>
    /// <example>
    /// Assuming album preference is set to <see cref="SiblingCollectionPlaybackBehavior.AllCollections"/>.
    /// From an album collection (such as library), the user selects to play an album. When playback of the album ends, the next album in the collection might begin.
    /// </example>
    public sealed class SiblingCollectionPlaybackPreferences
    {
        /// <summary>
        /// Behavior for playing an ArtistCollection that has sibling ArtistCollections.
        /// </summary>
        public SiblingCollectionPlaybackBehavior Artists { get; set; }

        /// <summary>
        /// Behavior for playing an AlbumCollection that has sibling AlbumCollection.
        /// </summary>
        public SiblingCollectionPlaybackBehavior Albums { get; set; }

        /// <summary>
        /// Behavior for playing an PlaylistCollection that has sibling PlaylistCollections.
        /// </summary>
        public SiblingCollectionPlaybackBehavior Playlists { get; set; }

        /// <summary>
        /// Behavior for playing an PlayableCollectionGroup that has sibling PlayableCollectionGroup.
        /// </summary>
        public SiblingCollectionPlaybackBehavior PlayableCollectionGroups { get; set; }
    }
}