namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Used to identify which variant of <see cref="ICoreTrack"/> this is.
    /// </summary>
    public enum TrackType
    {
        /// <summary>
        /// The <see cref="ICoreTrack"/> is a standard song.
        /// </summary>
        Song,

        /// <summary>
        /// The <see cref="ICoreTrack"/> is an episode of a podcast.
        /// </summary>
        PodcastEpisode,
    }
}
