using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzDevice : IDevice
    {
        /// <summary>
        /// Creates a <see cref="MusicBrainzDevice"/> with the core instance.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="name">Name of the device</param>
        /// <param name="isActive">State of the device.</param>
        public MusicBrainzDevice(ICore sourceCore, string name, bool isActive)
        {
            SourceCore = sourceCore;
            Name = name;
            IsActive = isActive;
            Id = Guid.NewGuid().ToString(); // hardcoded the Id for now.
            PlaybackQueue = new SynchronizedObservableCollection<ITrack>();
            Position = new TimeSpan(0, 0, 0); // hardcoded for now.

            IsActiveChanged?.Invoke(this, IsActive);
            PositionChanged?.Invoke(this, Position);
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsActive { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ITrack> PlaybackQueue { get; }

        /// <inheritdoc />
        public IPlayable PlaybackContext { get; }

        /// <inheritdoc />
        public ITrack? NowPlaying { get; }

        /// <inheritdoc />
        public TimeSpan Position { get; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public bool ShuffleState { get; }

        /// <inheritdoc />
        public RepeatState RepeatState { get; }

        /// <inheritdoc />
        public double VolumePercent { get; }

        /// <inheritdoc />
        public DeviceType Type { get; }

        /// <inheritdoc />
        public double PlaybackSpeed { get; }

        /// <inheritdoc />
        public bool IsShuffleStateChangedSupported { get; }

        /// <inheritdoc />
        public bool IsRepeatStateChangedSupported { get; }

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedSupported { get; }

        /// <inheritdoc />
        public bool IsResumeAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsNextAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsPreviousAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsSeekAsyncSupported { get; }

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
        public event EventHandler<IPlayable> PlaybackContextChanged;

        /// <inheritdoc />
        public event EventHandler<ITrack> NowPlayingChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan> PositionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<bool> ShuffleStateChanged;

        /// <inheritdoc />
        public event EventHandler<RepeatState> RepeatStateChanged;

        /// <inheritdoc />
        public event EventHandler<double> VolumePercentChanged;

        /// <inheritdoc />
        public event EventHandler<double> PlaybackSpeedChanged;
    }
}
