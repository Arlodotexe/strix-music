using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Enums
{
    /// <summary>
    /// Used to identify which variant of <see cref="ITrack"/> this is.
    /// </summary>
    public enum TrackType
    {
        /// <summary>
        /// The <see cref="ITrack"/> is a standard song.
        /// </summary>
        Song,

        /// <summary>
        /// The <see cref="ITrack"/> is an episode of a podcast.
        /// </summary>
        PodcastEpisode,
    }
}
