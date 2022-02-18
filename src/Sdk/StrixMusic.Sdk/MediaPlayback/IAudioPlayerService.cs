using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// A simple media player that can play an audio track.
    /// </summary>
    /// <remarks>Only plays one track at a time, and can cache other tracks ahead of time.</remarks>
    public interface IAudioPlayerService : IAudioPlayerBase
    {
        /// <summary>
        /// Plays a track.
        /// </summary>
        /// <param name="sourceConfig">The source configuration for this track.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Play(PlaybackItem sourceConfig);

        /// <summary>
        /// Preloads a track to be played later.
        /// </summary>
        /// <param name="sourceConfig">The track's source data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Preload(PlaybackItem sourceConfig);

        /// <summary>
        /// The currently playing media source.
        /// </summary>
        public PlaybackItem? CurrentSource { get; set; }

        /// <summary>
        /// Raised when <see cref="CurrentSource"/> is changed.
        /// </summary>
        public event EventHandler<PlaybackItem?>? CurrentSourceChanged;

        /// <summary>
        /// Raised when a quantum of data is processed. 
        /// </summary>
        public event EventHandler<float[]>? QuantumProcessed;
    }
}
