namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Current playback state.
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        /// The item is not playing, paused or queued to be played.
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
        /// The audio is not playing, but is queued to be played.
        /// </summary>
        Queued,
    }
}
