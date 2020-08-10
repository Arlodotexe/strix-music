using System;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Files.Models
{
    /// <inheritdoc/>
    public class FilesDevice : IDevice
    {
        /// <inheritdoc/>
        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public ICore SourceCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public IPlayableCollectionBase PlaybackContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public TimeSpan Position => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState State => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool? ShuffleState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public RepeatState RepeatState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public double? VolumePercent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public DeviceType DeviceType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public double PlaybackSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public ITrack? NowPlaying { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public event EventHandler<bool>? IsActiveChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionBase>? PlaybackContextChanged;

        /// <inheritdoc/>
        public event EventHandler<ITrack>? NowPlayingChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? StateChanged;

        /// <inheritdoc/>
        public event EventHandler<bool?>? ShuffleStateChanged;

        /// <inheritdoc/>
        public event EventHandler<RepeatState>? RepeatStateChanged;

        /// <inheritdoc/>
        public event EventHandler<double?>? VolumePercentChanged;

        /// <inheritdoc/>
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc/>
        public Task ChangeVolumeAsync(double volume)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task NextAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public Task PreviousAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ResumeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SeekAsync(long position)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SwitchToAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ToggleRepeatAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ToggleShuffleAsync()
        {
            throw new NotImplementedException();
        }
    }
}
