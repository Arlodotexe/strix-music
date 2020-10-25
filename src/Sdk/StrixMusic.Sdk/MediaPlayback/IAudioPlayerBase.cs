using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Basic properties and methods for using and manipulating an audio player.
    /// </summary>
    /// <remarks>Play is purposely missing from this interface, and the signature should be defined in a derived type.</remarks>
    public interface IAudioPlayerBase
    {
        /// <summary>
        /// The amount of time that has passed since a song has started.
        /// </summary>
        TimeSpan Position { get; }

        /// <inheritdoc cref="PlaybackState"/>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// The volume of the device (0-1).
        /// </summary>
        double Volume { get; }

        /// <summary>
        /// The rate of the playback for the current track.
        /// </summary>
        double PlaybackSpeed { get; }

        /// <summary>
        /// Seeks the track to a given timestamp.
        /// </summary>
        /// <param name="position">Time to seek the song to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SeekAsync(TimeSpan position);

        /// <summary>
        /// Attempts to change the playback speed.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangePlaybackSpeedAsync(double speed);

        /// <summary>
        /// Resume the device if in the state <see cref="MediaPlayback.PlaybackState.Paused"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ResumeAsync();

        /// <summary>
        /// Pauses the device if in the state <see cref="MediaPlayback.PlaybackState.Playing"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseAsync();

        /// <summary>
        /// Changes the volume
        /// </summary>
        /// <param name="volume">The volume of the device.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeVolumeAsync(double volume);

        /// <summary>
        /// Fires when <see cref="Position"/> changes.
        /// </summary>
        event EventHandler<TimeSpan> PositionChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackState"/> changes.
        /// </summary>
        event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <summary>
        /// Fires when <see cref="Volume"/> changes.
        /// </summary>
        event EventHandler<double> VolumeChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackSpeed"/> changes.
        /// </summary>
        event EventHandler<double> PlaybackSpeedChanged;
    }
}
