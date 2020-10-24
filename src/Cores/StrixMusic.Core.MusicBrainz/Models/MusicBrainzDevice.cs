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
        private string _name;
        private double _volume;
        private double _speed;
        private TimeSpan _position;
        private string _id;
        private RepeatState _repeatState;
        private bool _suffleState;
        private PlaybackState _playBackState;
        private ITrack? _nowPlaying;
        private IPlayable _playBackContext;
        private bool _isActive;

        /// <summary>
        /// Creates a <see cref="MusicBrainzDevice"/> with the core instance.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="name">Name of the device</param>
        /// <param name="isActive">State of the device.</param>
        public MusicBrainzDevice(ICore sourceCore, string name, bool isActive)
        {
            SourceCore = sourceCore;
            _name = name;
            _isActive = isActive;
            _id = Guid.NewGuid().ToString(); // hardcoded the Id for now.
            PlaybackQueue = new SynchronizedObservableCollection<ITrack>();
            _position = new TimeSpan(0, 0, 0); // hardcoded for now.

            IsActiveChanged?.Invoke(this, _isActive);
            PositionChanged?.Invoke(this, _position);
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id { get => _id; }

        /// <inheritdoc />
        public string Name { get => _name; }

        /// <inheritdoc />
        public bool IsActive { get => _isActive; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ITrack> PlaybackQueue { get; }

        /// <inheritdoc />
        public IPlayable PlaybackContext { get => _playBackContext; }

        /// <inheritdoc />
        public ITrack? NowPlaying { get => _nowPlaying; }

        /// <inheritdoc />
        public TimeSpan Position { get => _position; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get => _playBackState; }

        /// <inheritdoc />
        public bool ShuffleState { get => _suffleState; }

        /// <inheritdoc />
        public RepeatState RepeatState { get => _repeatState; }

        /// <inheritdoc />
        public double VolumePercent { get => _volume; }

        /// <inheritdoc />
        public DeviceType Type { get; }

        /// <inheritdoc />
        public double PlaybackSpeed { get => _speed; }

        /// <inheritdoc />
        public bool IsShuffleStateChangedSupported { get; } = true;

        /// <inheritdoc />
        public bool IsRepeatStateChangedSupported { get; } = true;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncSupported { get; } = true;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedSupported { get; } = true;

        /// <inheritdoc />
        public bool IsResumeAsyncSupported { get; } = true;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported { get; } = true;

        /// <inheritdoc />
        public bool IsNextAsyncSupported { get; } = true;

        /// <inheritdoc />
        public bool IsPreviousAsyncSupported { get; } = true;

        /// <inheritdoc />
        public bool IsSeekAsyncSupported { get; } = true;

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            _speed = speed;
            PlaybackSpeedChanged?.Invoke(this, speed);

            return Task.CompletedTask;
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
        public async Task SeekAsync(TimeSpan position)
        {
            await Task.Delay(500);
            _position = position;
            PositionChanged?.Invoke(this, _position);
        }

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume)
        {
            _volume = volume;
            VolumePercentChanged?.Invoke(this, volume);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync()
        {
            _suffleState = !_suffleState;
            ShuffleStateChanged(this, _suffleState);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ToggleRepeatAsync()
        {
            switch (_repeatState)
            {
                case RepeatState.All:
                    _repeatState = RepeatState.None;
                    break;
                case RepeatState.None:
                    _repeatState = RepeatState.One;
                    break;
                case RepeatState.One:
                    _repeatState = RepeatState.All;
                    break;
                default:
                    _repeatState = RepeatState.None;
                    break;
            }

            RepeatStateChanged(this, _repeatState);

            return Task.CompletedTask;
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
