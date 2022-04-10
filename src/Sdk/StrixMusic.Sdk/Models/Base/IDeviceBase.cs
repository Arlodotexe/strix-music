// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A device that controls playback of an audio player.
    /// </summary>
    public interface IDeviceBase : IAudioPlayerBase, IAsyncDisposable
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
        IPlayableBase? PlaybackContext { get; }

        /// <inheritdoc cref="DeviceType" />
        DeviceType Type { get; }

        /// <summary>
        /// True if the player is using a shuffled track list.
        /// </summary>
        bool ShuffleState { get; }

        /// <inheritdoc cref="RepeatState"/>
        RepeatState RepeatState { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.SeekAsync(TimeSpan, CancellationToken)"/> is supported.
        /// </summary>
        bool IsSeekAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.ResumeAsync(CancellationToken)"/> is supported.
        /// </summary>
        bool IsResumeAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.PauseAsync(CancellationToken)"/> is supported.
        /// </summary>
        bool IsPauseAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.ChangeVolumeAsync(double, CancellationToken)"/> is supported.
        /// </summary>
        bool IsChangeVolumeAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="IAudioPlayerBase.ChangePlaybackSpeedAsync(double, CancellationToken)"/> is supported.
        /// </summary>
        bool IsChangePlaybackSpeedAvailable { get; }

        /// <summary>
        /// If true, <see cref="NextAsync(CancellationToken)"/> is supported.
        /// </summary>
        bool IsNextAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PreviousAsync(CancellationToken)"/> is supported.
        /// </summary>
        bool IsPreviousAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ToggleShuffleAsync"/> is supported.
        /// </summary>
        bool IsToggleShuffleAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ToggleRepeatAsync"/> is supported.
        /// </summary>
        bool IsToggleRepeatAsyncAvailable { get; }

        /// <summary>
        /// Advances to the next track. If there is no next track, playback is paused.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task NextAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PreviousAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Toggles shuffle on or off.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleShuffleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asks the device to toggle to the next repeat state.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleRepeatAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Switches to this device.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SwitchToAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when <see cref="IsActive"/> changes.
        /// </summary>
        event EventHandler<bool>? IsActiveChanged;

        /// <summary>
        /// Fires when <see cref="PlaybackContext"/> changes.
        /// </summary>
        event EventHandler<IPlayableBase>? PlaybackContextChanged;

        /// <summary>
        /// Fires when <see cref="ShuffleState"/> changes.
        /// </summary>
        event EventHandler<bool>? ShuffleStateChanged;

        /// <summary>
        /// Fires when <see cref="RepeatState"/> changes.
        /// </summary>
        event EventHandler<RepeatState>? RepeatStateChanged;
    }
}
