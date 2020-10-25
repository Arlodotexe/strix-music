namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// How songs will repeat in their context.
    /// </summary>
    public enum RepeatState
    {
        /// <summary>
        /// Neither track nor context will repeat.
        /// </summary>
        None,

        /// <summary>
        /// The current track will repeat when done playing.
        /// </summary>
        One,

        /// <summary>
        /// The current playback context (such as a Playlist) will start over when it finishes playing the last item.
        /// </summary>
        All,
    }
}
