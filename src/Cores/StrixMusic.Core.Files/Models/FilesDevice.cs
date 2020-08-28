using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
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
        public PlaybackState PlaybackState => throw new NotImplementedException();

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
        double? IDevice.PlaybackSpeed => throw new NotImplementedException();

        /// <inheritdoc/>
        IPlayable IDevice.PlaybackContext => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> PlaybackQueue => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<bool>? IsActiveChanged
        {
            add
            {
                IsActiveChanged += value;
            }

            remove
            {
                IsActiveChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionBase>? PlaybackContextChanged
        {
            add
            {
                PlaybackContextChanged += value;
            }

            remove
            {
                PlaybackContextChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<ITrack>? NowPlayingChanged
        {
            add
            {
                NowPlayingChanged += value;
            }

            remove
            {
                NowPlayingChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? PositionChanged
        {
            add
            {
                PositionChanged += value;
            }

            remove
            {
                PositionChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                PlaybackStateChanged += value;
            }

            remove
            {
                PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<bool?>? ShuffleStateChanged
        {
            add
            {
                ShuffleStateChanged += value;
            }

            remove
            {
                ShuffleStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<RepeatState>? RepeatStateChanged
        {
            add
            {
                RepeatStateChanged += value;
            }

            remove
            {
                RepeatStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<double?>? VolumePercentChanged;

        /// <inheritdoc/>
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? PlaybackQueueChanged;

        /// <inheritdoc/>
        public Task ChangePlaybackSpeed(double speed)
        {
            throw new NotImplementedException();
        }

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
