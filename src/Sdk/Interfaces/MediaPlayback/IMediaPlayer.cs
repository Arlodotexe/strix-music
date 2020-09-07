using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces.MediaPlayback
{
    /// <summary>
    /// A simple media player that can play an audio track.
    /// </summary>
    /// <remarks>Only plays one track at a time, and can cache other tracks ahead of time.</remarks>
    public interface IMediaPlayer
    {
        /// <summary>
        /// Plays a preloaded track.
        /// </summary>
        /// <param name="id">The ID that was registered during preload.</param>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation.</returns>
        Task Play(string id);

        /// <summary>
        /// Plays a track.
        /// </summary>
        /// <param name="config">The source configuration for this track.</param>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation.</returns>
        Task Play(IMediaSourceConfig config);

        /// <summary>
        /// Preloads a track given a config.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation.</returns>
        Task Preload(IMediaSourceConfig config);
    }
}
