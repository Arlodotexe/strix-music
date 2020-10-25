namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Preferred player type.
    /// </summary>
    public enum MediaPlayerType
    {
        /// <summary>
        /// No media playback supported.
        /// </summary>
        None,

        /// <summary>
        /// A standard MediaPlayerElement with no special features.
        /// </summary>
        Standard,

        /// <summary>
        /// A PlayReady-enabled MediaPlayerElement.
        /// </summary>
        PlayReady,

        /// <summary>
        /// No in-app MediaPlayerElement will be created, and all device manipulations will be directly delegated to the core.
        /// </summary>
        RemoteOnly,
    }
}