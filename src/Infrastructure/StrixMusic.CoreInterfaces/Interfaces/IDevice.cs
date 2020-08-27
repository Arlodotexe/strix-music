using System;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents a device that a user can connect to for playback
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// The source core which created the object.
        /// </summary>
        public ICore SourceCore { get; }

        /// <summary>
        /// Identifies the device
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The displayed name of this device
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If true, the device is currently active and playing audio.
        /// </summary>
        bool IsActive { get; }

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
        /// <remarks>If null, the player cannot be shuffled.</remarks>
        bool? ShuffleState { get; }

        /// <inheritdoc cref="Enums.RepeatState"/>
        RepeatState RepeatState { get; }

        /// <summary>
        /// The volume of the device (0-1). If null, the volume cannot be changed.
        /// </summary>
        double? VolumePercent { get; }

        /// <inheritdoc cref="Enums.DeviceType" />
        DeviceType DeviceType { get; }

        /// <summary>
        /// The rate of the playback for the current track. If the playback speed can't be changed, value is <see langword="null"/>
        /// </summary>
        double? PlaybackSpeed { get; }

        /// <summary>
        /// Attempts to change the playback speed.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ChangePlaybackSpeed(double speed);

        /// <summary>
        /// Resume the device if in the state <see cref="Enums.PlaybackState.Paused"/>.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ResumeAsync();

        /// <summary>
        /// Pauses the device if in the state <see cref="Enums.PlaybackState.Playing"/>
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task PauseAsync();

        /// <summary>
        /// Advances to the next track. If there is no next track, playback is paused.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task NextAsync();

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task PreviousAsync();

        /// <summary>
        /// Seeks the track to a given timestamp.
        /// </summary>
        /// <param name="position">Time in milliseconds to seek the song to.</param>
        /// <returns><see langword="true"/> if the <see cref="ITrack"/> was seeked to successfully, <see langword="false"/> otherwise.</returns>
        Task SeekAsync(long position);

        /// <summary>
        /// Changes the volume
        /// </summary>
        /// <param name="volume">The volume of the device.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ChangeVolumeAsync(double volume);

        /// <summary>
        /// Toggles shuffle on or off.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ToggleShuffleAsync();

        /// <summary>
        /// Asks the device to toggle to the next repeat state.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ToggleRepeatAsync();

        /// <summary>
        /// Switches to this device.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task SwitchToAsync();

        /// <summary>
        /// Fires when <see cref="IsActive"/> changes.
        /// </summary>
        event EventHandler<bool>? IsActiveChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackContext"/> changes.
        /// </summary>
        event EventHandler<IPlayableCollectionBase> PlaybackContextChanged;

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
        event EventHandler<bool?> ShuffleStateChanged;

        /// <summary>
        /// Fires when <see cref="RepeatState"/> changes.
        /// </summary>
        event EventHandler<RepeatState> RepeatStateChanged;

        /// <summary>
        /// Fires when <see cref="VolumePercent"/> changes.
        /// </summary>
        event EventHandler<double?> VolumePercentChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackSpeed"/> changes.
        /// </summary>
        event EventHandler<double> PlaybackSpeedChanged;
    }
}
