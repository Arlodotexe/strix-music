using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// User preferences for behavior when the user requests to play a single item from a collection of collections.
    /// </summary>
    /// <example>From the library, the user requests to requests that an album is played. This decides</example>
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