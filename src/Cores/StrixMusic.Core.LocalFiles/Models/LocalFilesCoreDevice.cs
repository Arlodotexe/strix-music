using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc />
    public class LocalFilesCoreDevice : ICoreDevice
    {
        /// <summary>
        /// Creates a <see cref="LocalFilesCoreDevice"/> with the core instance.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="name">Name of the device</param>
        /// <param name="isActive">State of the device.</param>
        public LocalFilesCoreDevice(ICore sourceCore, string name, bool isActive)
        {
            SourceCore = sourceCore;
            Name = name;
            IsActive = isActive;
            Id = Guid.NewGuid().ToString(); // hardcoded the Id for now.
            Position = new TimeSpan(0, 0, 0); // hardcoded for now.

            IsActiveChanged?.Invoke(this, IsActive);
            PositionChanged?.Invoke(this, Position);
        }

        /// <inheritdoc cref="ICoreMember.SourceCore" />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsActive { get; }

        /// <inheritdoc />
        public ICoreTrackCollection? PlaybackQueue { get; }

        /// <inheritdoc />
        public IPlayable? PlaybackContext { get; }

        /// <inheritdoc />
        public ICoreTrack? NowPlaying { get; }

        /// <inheritdoc />
        public TimeSpan Position { get; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public bool ShuffleState { get; }

        /// <inheritdoc />
        public RepeatState RepeatState { get; }

        /// <inheritdoc />
        public double Volume { get; }

        /// <inheritdoc />
        public DeviceType Type { get; }

        /// <inheritdoc />
        public double PlaybackSpeed { get; }

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedAvailable { get; }

        /// <inheritdoc />
        public bool IsResumeAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsNextAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsPreviousAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsSeekAsyncAvailable { get; }

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ResumeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task NextAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task PreviousAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ToggleRepeatAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task SwitchToAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged;

        /// <inheritdoc />
        public event EventHandler<IPlayable>? PlaybackContextChanged;

        /// <inheritdoc />
        public event EventHandler<ICoreTrack>? NowPlayingChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged;

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged;

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged;

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged;
    }
}
