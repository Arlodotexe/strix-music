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
        /// The audio is currently playing.
        /// </summary>
        Playing,

        /// <summary>
        /// The audio is playing, but is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The audio player is loading.
        /// </summary>
        Loading,
    }
}
