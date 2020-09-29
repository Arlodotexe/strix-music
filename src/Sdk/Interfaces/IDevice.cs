using System;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Represents a device that a user can connect to for playback
    /// </summary>
    public interface IDevice : ICoreMember
    {
        /// <summary>
        /// Identifies the device.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The displayed name of this device.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If true, the device is currently active and playing audio.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// The current playback queue.
        /// </summary>
        /// <remarks>When <see cref="DeviceType"/> is <see cref="DeviceType.Remote" />, this will override the internal playback queue.</remarks>
        public SynchronizedObservableCollection<ITrack> PlaybackQueue { get; }

        /// <summary>
        /// The context of the currently playing track.
        /// </summary>
        IPlayable PlaybackContext { get; }

        /// <summary>
        /// The currently playing <see cref="ITrack"/>.
        /// </summary>
        ITrack? NowPlaying { get; }

        /// <summary>
        /// The amount of time that has passed since a song has started.
        /// </summary>
        TimeSpan Position { get; }

        /// <inheritdoc cref="Enums.PlaybackState"/>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// True if the player is using a shuffled track list.
        /// </summary>
        bool ShuffleState { get; }

        /// <inheritdoc cref="Enums.RepeatState"/>
        RepeatState RepeatState { get; }

        /// <summary>
        /// The volume of the device (0-1).
        /// </summary>
        double VolumePercent { get; }

        /// <inheritdoc cref="Enums.DeviceType" />
        DeviceType Type { get; }

        /// <summary>
        /// The rate of the playback for the current track.
        /// </summary>
        double PlaybackSpeed { get; }

        /// <summary>
        /// If true, <see cref="ToggleShuffleAsync"/> is supported.
        /// </summary>
        bool IsShuffleStateChangedSupported { get; }

        /// <summary>
        /// If true, <see cref="ToggleRepeatAsync"/> is supported.
        /// </summary>
        bool IsRepeatStateChangedSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeVolumeAsync(double)"/> is supported.
        /// </summary>
        bool IsChangeVolumeAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangePlaybackSpeedAsync(double)"/> is supported.
        /// </summary>
        bool IsChangePlaybackSpeedSupported { get; }

        /// <summary>
        /// If true, <see cref="ResumeAsync()"/> is supported.
        /// </summary>
        bool IsResumeAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="PauseAsync()"/> is supported.
        /// </summary>
        bool IsPauseAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="NextAsync()"/> is supported.
        /// </summary>
        bool IsNextAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="PreviousAsync()"/> is supported.
        /// </summary>
        bool IsPreviousAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="SeekAsync(TimeSpan)"/> is supported.
        /// </summary>
        bool IsSeekAsyncSupported { get; }

        /// <summary>
        /// Attempts to change the playback speed.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangePlaybackSpeedAsync(double speed);

        /// <summary>
        /// Resume the device if in the state <see cref="Enums.PlaybackState.Paused"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ResumeAsync();

        /// <summary>
        /// Pauses the device if in the state <see cref="Enums.PlaybackState.Playing"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseAsync();

        /// <summary>
        /// Advances to the next track. If there is no next track, playback is paused.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task NextAsync();

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PreviousAsync();

        /// <summary>
        /// Seeks the track to a given timestamp.
        /// </summary>
        /// <param name="position">Time to seek the song to.</param>
        /// <returns><see langword="true"/> if the <see cref="ITrack"/> was seeked to successfully, <see langword="false"/> otherwise.</returns>
        Task SeekAsync(TimeSpan position);

        /// <summary>
        /// Changes the volume
        /// </summary>
        /// <param name="volume">The volume of the device.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeVolumeAsync(double volume);

        /// <summary>
        /// Toggles shuffle on or off.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleShuffleAsync();

        /// <summary>
        /// Asks the device to toggle to the next repeat state.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleRepeatAsync();

        /// <summary>
        /// Switches to this device.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SwitchToAsync();

        /// <summary>
        /// Fires when <see cref="IsActive"/> changes.
        /// </summary>
        event EventHandler<bool>? IsActiveChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackContext"/> changes.
        /// </summary>
        event EventHandler<IPlayable> PlaybackContextChanged;

        /// <summary>
        /// Fires when <see cref="NowPlaying"/> changes.
        /// </summary>
        event EventHandler<ITrack> NowPlayingChanged;

        /// <summary>
        /// Fires when <see cref="Position"/> changes.
        /// </summary>
        event EventHandler<TimeSpan> PositionChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackState"/> changes.
        /// </summary>
        event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <summary>
        /// Fires when <see cref="ShuffleState"/> changes.
        /// </summary>
        event EventHandler<bool> ShuffleStateChanged;

        /// <summary>
        /// Fires when <see cref="RepeatState"/> changes.
        /// </summary>
        event EventHandler<RepeatState> RepeatStateChanged;

        /// <summary>
        /// Fires when <see cref="VolumePercent"/> changes.
        /// </summary>
        event EventHandler<double> VolumePercentChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackSpeed"/> changes.
        /// </summary>
        event EventHandler<double> PlaybackSpeedChanged;
    }
}
