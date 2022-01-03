namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Current playback state.
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        /// The item has no playback known state.
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
        /// The item has been loaded and playback can begin without wait.
        /// </summary>
        Loaded,

        /// <summary>
        /// The audio player is loading.
        /// </summary>
        Loading,
    }
}
