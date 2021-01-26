namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Current playback state.
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        /// The item is not playing, paused or loading.
        /// </summary>
        None,

        /// <summary>
        /// Playback was initiated but failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The audio is currently playing.
        /// </summary>
        Playing,

        /// <summary>
        /// The audio is playing, but is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The item was queued by user to play next.
        /// </summary>
        Queued,

        /// <summary>
        /// The audio player is loading.
        /// </summary>
        Loading,
    }
}
