using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A device that controls playback of an audio player.
    /// </summary>
    public interface IDeviceBase : IAudioPlayerBase
    {
        /// <summary>
        /// A unique identifier for the player.
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
        /// The context of the currently playing track.
        /// </summary>
        IPlayable? PlaybackContext { get; }

        /// <inheritdoc cref="DeviceType" />
        DeviceType Type { get; }

        /// <summary>
        /// True if the player is using a shuffled track list.
        /// </summary>
        bool ShuffleState { get; }

        /// <inheritdoc cref="RepeatState"/>
        RepeatState RepeatState { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.SeekAsync(TimeSpan)"/> is supported.
        /// </summary>
        bool IsSeekAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.ResumeAsync()"/> is supported.
        /// </summary>
        bool IsResumeAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.PauseAsync()"/> is supported.
        /// </summary>
        bool IsPauseAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.ChangeVolumeAsync(double)"/> is supported.
        /// </summary>
        bool IsChangeVolumeAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.ChangePlaybackSpeedAsync(double)"/> is supported.
        /// </summary>
        bool IsChangePlaybackSpeedSupported { get; }

        /// <summary>
        /// If true, <see cref="NextAsync()"/> is supported.
        /// </summary>
        bool IsNextAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="PreviousAsync()"/> is supported.
        /// </summary>
        bool IsPreviousAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ToggleShuffleAsync"/> is supported.
        /// </summary>
        bool IsToggleShuffleAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ToggleRepeatAsync"/> is supported.
        /// </summary>
        bool IsToggleRepeatAsyncSupported { get; }

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
        /// Fires when <see cref="ShuffleState"/> changes.
        /// </summary>
        event EventHandler<bool> ShuffleStateChanged;

        /// <summary>
        /// Fires when <see cref="RepeatState"/> changes.
        /// </summary>
        event EventHandler<RepeatState> RepeatStateChanged;
    }
}