using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains information about a <see cref="IImage"/>
    /// </summary>
    public class ObservableDevice : ObservableObject, IDevice
    {
        private readonly IDevice _device;

        private ITrack? _nowPlaying;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDevice"/> class.
        /// </summary>
        /// <param name="device">The base <see cref="IDevice"/></param>
        public ObservableDevice(IDevice device)
        {
            _device = device;

            if (_device.NowPlaying != null)
                NowPlaying = new ObservableTrack(_device.NowPlaying);

            SourceCore = MainViewModel.GetLoadedCore(_device.SourceCore);

            PlaybackQueue = new ObservableCollection<ITrack>(_device.PlaybackQueue.Select(x => new ObservableTrack(x)));

            ChangePlaybackSpeedAsyncCommand = new AsyncRelayCommand<double>(ChangePlaybackSpeedAsync);
            ResumeAsyncCommand = new AsyncRelayCommand(ResumeAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            NextAsyncCommand = new AsyncRelayCommand(NextAsync);
            PreviousAsyncCommand = new AsyncRelayCommand(PreviousAsync);
            SeekAsyncCommand = new AsyncRelayCommand<TimeSpan>(SeekAsync);
            ChangeVolumeAsyncCommand = new AsyncRelayCommand<double>(ChangeVolumeAsync);
            ToggleShuffleCommandAsync = new AsyncRelayCommand(ToggleShuffleAsync);
            ToggleRepeatCommandAsync = new AsyncRelayCommand(ToggleRepeatAsync);
            SwitchToAsyncCommand = new AsyncRelayCommand(SwitchToAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _device.IsActiveChanged += Device_IsActiveChanged;
            _device.NowPlayingChanged += Device_NowPlayingChanged;
            _device.PlaybackContextChanged += Device_PlaybackContextChanged;
            _device.PlaybackSpeedChanged += Device_PlaybackSpeedChanged;
            _device.PositionChanged += Device_PositionChanged;
            _device.RepeatStateChanged += Device_RepeatStateChanged;
            _device.ShuffleStateChanged += Device_ShuffleStateChanged;
            _device.PlaybackStateChanged += Device_StateChanged;
            _device.VolumePercentChanged += Device_VolumePercentChanged;
        }

        private void DetachEvents()
        {
            _device.IsActiveChanged -= Device_IsActiveChanged;
            _device.NowPlayingChanged -= Device_NowPlayingChanged;
            _device.PlaybackContextChanged -= Device_PlaybackContextChanged;
            _device.PlaybackSpeedChanged -= Device_PlaybackSpeedChanged;
            _device.PositionChanged -= Device_PositionChanged;
            _device.RepeatStateChanged -= Device_RepeatStateChanged;
            _device.ShuffleStateChanged -= Device_ShuffleStateChanged;
            _device.PlaybackStateChanged -= Device_StateChanged;
            _device.VolumePercentChanged -= Device_VolumePercentChanged;
        }

        private void Device_VolumePercentChanged(object sender, double e)
        {
            VolumePercent = e;
        }

        private void Device_StateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void Device_ShuffleStateChanged(object sender, bool e) => ShuffleState = e;

        private void Device_RepeatStateChanged(object sender, RepeatState e) => RepeatState = e;

        private void Device_PositionChanged(object sender, TimeSpan e) => Position = e;

        private void Device_PlaybackSpeedChanged(object sender, double e) => PlaybackSpeed = e;

        private void Device_PlaybackContextChanged(object sender, IPlayableCollectionBase e) => PlaybackContext = e;

        private void Device_NowPlayingChanged(object sender, ITrack e) => NowPlaying = new ObservableTrack(e);

        private void Device_IsActiveChanged(object sender, bool e) => IsActive = e;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _device.Id;

        /// <inheritdoc />
        public string Name => _device.Name;

        /// <inheritdoc />
        public DeviceType DeviceType => _device.DeviceType;

        /// <inheritdoc />
        public ObservableCollection<ITrack> PlaybackQueue { get; }

        /// <inheritdoc />
        public bool IsActive
        {
            get => _device.IsActive;
            private set => SetProperty(() => _device.IsActive, value);
        }

        /// <inheritdoc />
        public IPlayable PlaybackContext
        {
            get => _device.PlaybackContext;
            private set => SetProperty(() => _device.PlaybackContext, value);
        }

        /// <inheritdoc />
        public ITrack? NowPlaying
        {
            get => _nowPlaying;
            private set => SetProperty(ref _nowPlaying, value);
        }

        /// <inheritdoc />
        public TimeSpan Position
        {
            get => _device.Position;
            private set => SetProperty(() => _device.Position, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _device.PlaybackState;
            private set => SetProperty(() => _device.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool ShuffleState
        {
            get => _device.ShuffleState;
            private set => SetProperty(() => _device.ShuffleState, value);
        }

        /// <inheritdoc />
        public RepeatState RepeatState
        {
            get => _device.RepeatState;
            private set => SetProperty(() => _device.RepeatState, value);
        }

        /// <inheritdoc />
        public double VolumePercent
        {
            get => _device.VolumePercent;
            private set => SetProperty(() => _device.VolumePercent, value);
        }

        /// <inheritdoc />
        public double PlaybackSpeed
        {
            get => _device.PlaybackSpeed;
            private set => SetProperty(() => _device.PlaybackSpeed, value);
        }

        /// <inheritdoc />
        public bool IsShuffleStateChangedSupported => _device.IsShuffleStateChangedSupported;

        /// <inheritdoc />
        public bool IsRepeatStateChangedSupported => _device.IsRepeatStateChangedSupported;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncSupported => _device.IsChangeVolumeAsyncSupported;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedSupported => _device.IsChangePlaybackSpeedSupported;

        /// <inheritdoc />
        public bool IsResumeAsyncSupported => _device.IsResumeAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _device.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsNextAsyncSupported => _device.IsNextAsyncSupported;

        /// <inheritdoc />
        public bool IsPreviousAsyncSupported => _device.IsPreviousAsyncSupported;

        /// <inheritdoc />
        public bool IsSeekAsyncSupported => _device.IsSeekAsyncSupported;

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged
        {
            add => _device.IsActiveChanged += value;

            remove => _device.IsActiveChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IPlayableCollectionBase> PlaybackContextChanged
        {
            add => _device.PlaybackContextChanged += value;

            remove => _device.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ITrack> NowPlayingChanged
        {
            add => _device.NowPlayingChanged += value;

            remove => _device.NowPlayingChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan> PositionChanged
        {
            add => _device.PositionChanged += value;

            remove => _device.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _device.PlaybackStateChanged += value;

            remove => _device.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool> ShuffleStateChanged
        {
            add => _device.ShuffleStateChanged += value;

            remove => _device.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState> RepeatStateChanged
        {
            add => _device.RepeatStateChanged += value;

            remove => _device.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double> VolumePercentChanged
        {
            add => _device.VolumePercentChanged += value;

            remove => _device.VolumePercentChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double> PlaybackSpeedChanged
        {
            add => _device.PlaybackSpeedChanged += value;

            remove => _device.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            return _device.ChangePlaybackSpeedAsync(speed);
        }

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume)
        {
            return _device.ChangeVolumeAsync(volume);
        }

        /// <inheritdoc />
        public Task NextAsync()
        {
            return _device.NextAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return _device.PauseAsync();
        }

        /// <inheritdoc />
        public Task PreviousAsync()
        {
            return _device.PreviousAsync();
        }

        /// <inheritdoc />
        public Task ResumeAsync()
        {
            return _device.ResumeAsync();
        }

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position)
        {
            return _device.SeekAsync(position);
        }

        /// <inheritdoc />
        public Task SwitchToAsync()
        {
            return _device.SwitchToAsync();
        }

        /// <inheritdoc />
        public Task ToggleRepeatAsync()
        {
            return _device.ToggleRepeatAsync();
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync()
        {
            return _device.ToggleShuffleAsync();
        }

        /// <summary>
        /// Attempts to change playback speed.
        /// </summary>
        public IAsyncRelayCommand<double> ChangePlaybackSpeedAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the shuffle state for this device.
        /// </summary>
        public IAsyncRelayCommand ToggleShuffleCommandAsync { get; }

        /// <summary>
        /// Attempts to toggle the repeat state for this device.
        /// </summary>
        public IAsyncRelayCommand ToggleRepeatCommandAsync { get; }

        /// <summary>
        /// Attempts to switch playback to this device.
        /// </summary>
        public IAsyncRelayCommand SwitchToAsyncCommand { get; }

        /// <summary>
        /// Attempts to seek the currently playing track on the device. Does not alter playback state.
        /// </summary>
        public IAsyncRelayCommand SeekAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the device.
        /// </summary>
        public IAsyncRelayCommand ResumeAsyncCommand { get; }

        /// <summary>
        /// Attempts to move back to the previous track in the queue.
        /// </summary>
        public IAsyncRelayCommand PreviousAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the device.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to skip to the next track in the queue.
        /// </summary>
        public IAsyncRelayCommand NextAsyncCommand { get; }

        /// <summary>
        /// Attempts to change volume.
        /// </summary>
        public IAsyncRelayCommand<double> ChangeVolumeAsyncCommand { get; }
    }
}
