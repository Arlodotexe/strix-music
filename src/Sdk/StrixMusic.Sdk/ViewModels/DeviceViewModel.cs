using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains information about a <see cref="ICoreImage"/>
    /// </summary>
    public class DeviceViewModel : ObservableObject, IDeviceBase
    {
        private ICoreTrack? _nowPlaying;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
        /// </summary>
        /// <param name="device">The base <see cref="IDeviceBase"/></param>
        public DeviceViewModel(IDeviceBase device)
        {
            Model = device;

            if (Model.NowPlaying != null)
                NowPlaying = new TrackViewModel(Model.NowPlaying);

            SourceCore = Model.SourceCore != null ? MainViewModel.GetLoadedCore(Model.SourceCore) : null;

            if (Model.PlaybackQueue != null)
                PlaybackQueue = new TrackCollectionViewModel(Model.PlaybackQueue);

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
            Model.IsActiveChanged += Device_IsActiveChanged;
            Model.NowPlayingChanged += Device_NowPlayingChanged;
            Model.PlaybackContextChanged += Device_PlaybackContextChanged;
            Model.PlaybackSpeedChanged += Device_PlaybackSpeedChanged;
            Model.PositionChanged += Device_PositionChanged;
            Model.RepeatStateChanged += Device_RepeatStateChanged;
            Model.ShuffleStateChanged += Device_ShuffleStateChanged;
            Model.PlaybackStateChanged += Device_StateChanged;
            Model.VolumeChanged += Device_VolumeChanged;
        }

        private void DetachEvents()
        {
            Model.IsActiveChanged -= Device_IsActiveChanged;
            Model.NowPlayingChanged -= Device_NowPlayingChanged;
            Model.PlaybackContextChanged -= Device_PlaybackContextChanged;
            Model.PlaybackSpeedChanged -= Device_PlaybackSpeedChanged;
            Model.PositionChanged -= Device_PositionChanged;
            Model.RepeatStateChanged -= Device_RepeatStateChanged;
            Model.ShuffleStateChanged -= Device_ShuffleStateChanged;
            Model.PlaybackStateChanged -= Device_StateChanged;
            Model.VolumeChanged -= Device_VolumeChanged;
        }

        private void Device_VolumeChanged(object sender, double e) => Volume = e;

        private void Device_StateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void Device_ShuffleStateChanged(object sender, bool e) => ShuffleState = e;

        private void Device_RepeatStateChanged(object sender, RepeatState e) => RepeatState = e;

        private void Device_PositionChanged(object sender, TimeSpan e) => Position = e;

        private void Device_PlaybackSpeedChanged(object sender, double e) => PlaybackSpeed = e;

        private void Device_PlaybackContextChanged(object sender, IPlayable e) => PlaybackContext = e;

        private void Device_NowPlayingChanged(object sender, ICoreTrack e) => NowPlaying = new TrackViewModel(e);

        private void Device_IsActiveChanged(object sender, bool e) => IsActive = e;
        
        /// <summary>
        /// The wrapped model for this <see cref="DeviceViewModel"/>.
        /// </summary>
        internal IDeviceBase Model { get; set; }

        /// <inheritdoc />
        public ICore? SourceCore { get; }

        /// <inheritdoc />
        public string Id => Model.Id;

        /// <inheritdoc />
        public string Name => Model.Name;

        /// <inheritdoc />
        public DeviceType Type => Model.Type;

        /// <inheritdoc />
        public ITrackCollectionBase PlaybackQueue { get; }

        /// <inheritdoc />
        public bool IsActive
        {
            get => Model.IsActive;
            private set => SetProperty(() => Model.IsActive, value);
        }

        /// <inheritdoc />
        public IPlayable? PlaybackContext
        {
            get => Model.PlaybackContext;
            internal set => SetProperty(() => Model.PlaybackContext, value);
        }

        /// <inheritdoc />
        public ICoreTrack? NowPlaying
        {
            get => _nowPlaying;
            internal set => SetProperty(ref _nowPlaying, value);
        }

        /// <inheritdoc />
        public TimeSpan Position
        {
            get => Model.Position;
            private set => SetProperty(() => Model.Position, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => Model.PlaybackState;
            private set => SetProperty(() => Model.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool ShuffleState
        {
            get => Model.ShuffleState;
            private set => SetProperty(() => Model.ShuffleState, value);
        }

        /// <inheritdoc />
        public RepeatState RepeatState
        {
            get => Model.RepeatState;
            private set => SetProperty(() => Model.RepeatState, value);
        }

        /// <inheritdoc />
        public double Volume
        {
            get => Model.Volume;
            private set => SetProperty(() => Model.Volume, value);
        }

        /// <inheritdoc />
        public double PlaybackSpeed
        {
            get => Model.PlaybackSpeed;
            private set => SetProperty(() => Model.PlaybackSpeed, value);
        }

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncSupported => Model.IsToggleShuffleAsyncSupported;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncSupported => Model.IsToggleRepeatAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncSupported => Model.IsChangeVolumeAsyncSupported;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedSupported => Model.IsChangePlaybackSpeedSupported;

        /// <inheritdoc />
        public bool IsResumeAsyncSupported => Model.IsResumeAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => Model.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsNextAsyncSupported => Model.IsNextAsyncSupported;

        /// <inheritdoc />
        public bool IsPreviousAsyncSupported => Model.IsPreviousAsyncSupported;

        /// <inheritdoc />
        public bool IsSeekAsyncSupported => Model.IsSeekAsyncSupported;

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged
        {
            add => Model.IsActiveChanged += value;

            remove => Model.IsActiveChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IPlayable> PlaybackContextChanged
        {
            add => Model.PlaybackContextChanged += value;

            remove => Model.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ICoreTrack> NowPlayingChanged
        {
            add => Model.NowPlayingChanged += value;

            remove => Model.NowPlayingChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan> PositionChanged
        {
            add => Model.PositionChanged += value;

            remove => Model.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => Model.PlaybackStateChanged += value;

            remove => Model.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool> ShuffleStateChanged
        {
            add => Model.ShuffleStateChanged += value;

            remove => Model.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState> RepeatStateChanged
        {
            add => Model.RepeatStateChanged += value;

            remove => Model.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double> VolumeChanged
        {
            add => Model.VolumeChanged += value;

            remove => Model.VolumeChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double> PlaybackSpeedChanged
        {
            add => Model.PlaybackSpeedChanged += value;

            remove => Model.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            return Model.ChangePlaybackSpeedAsync(speed);
        }

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume)
        {
            return Model.ChangeVolumeAsync(volume);
        }

        /// <inheritdoc />
        public Task NextAsync()
        {
            return Model.NextAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return Model.PauseAsync();
        }

        /// <inheritdoc />
        public Task PreviousAsync()
        {
            return Model.PreviousAsync();
        }

        /// <inheritdoc />
        public Task ResumeAsync()
        {
            return Model.ResumeAsync();
        }

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position)
        {
            return Model.SeekAsync(position);
        }

        /// <inheritdoc />
        public Task SwitchToAsync()
        {
            return Model.SwitchToAsync();
        }

        /// <inheritdoc />
        public Task ToggleRepeatAsync()
        {
            return Model.ToggleRepeatAsync();
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync()
        {
            return Model.ToggleShuffleAsync();
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
