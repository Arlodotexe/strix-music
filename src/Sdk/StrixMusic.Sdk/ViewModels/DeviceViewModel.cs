using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains information about a <see cref="IImage"/>
    /// </summary>
    public sealed class DeviceViewModel : ObservableObject, ISdkViewModel, IDevice
    {
        private PlaybackItem? _nowPlaying;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="device">The <see cref="IDevice"/> to wrap around.</param>
        internal DeviceViewModel(MainViewModel root, IDevice device)
        {
            Model = device ?? throw new ArgumentNullException(nameof(device));
            Root = root;

            if (Model.NowPlaying != null)
                _nowPlaying = Model.NowPlaying;

            if (device.SourceCore != null)
                SourceCore = root.GetLoadedCore(device.SourceCore);

            if (Model.PlaybackQueue != null)
                PlaybackQueue = new TrackCollectionViewModel(root, Model.PlaybackQueue);

            ChangePlaybackSpeedAsyncCommand = new AsyncRelayCommand<double>(ChangePlaybackSpeedAsync);
            ResumeAsyncCommand = new AsyncRelayCommand(ResumeAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            TogglePauseResumeCommand = new AsyncRelayCommand(TogglePauseResume);
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

        private void Device_VolumeChanged(object sender, double e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Volume)));

        private void Device_ShuffleStateChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(ShuffleState)));

        private void Device_RepeatStateChanged(object sender, RepeatState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(RepeatState)));

        private void Device_PositionChanged(object sender, TimeSpan e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Position)));

        private void Device_PlaybackSpeedChanged(object sender, double e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackSpeed)));

        private void Device_PlaybackContextChanged(object sender, IPlayableBase e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackContext)));

        private void Device_IsActiveChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsActive)));

        private void Device_StateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() =>
        {
            OnPropertyChanged(nameof(PlaybackState));
            OnPropertyChanged(nameof(IsPlaying));
        });

        private void Device_NowPlayingChanged(object sender, PlaybackItem e) => _ = Threading.OnPrimaryThread(() =>
        {
            NowPlaying = e;
            NowPlayingChanged?.Invoke(sender, e);
        });

        /// <summary>
        /// The wrapped model for this <see cref="DeviceViewModel"/>.
        /// </summary>
        internal IDevice Model { get; set; }

        /// <inheritdoc />
        public ICore? SourceCore { get; set; }

        /// <inheritdoc />
        public ICoreDevice? Source => Model.Source;

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc />
        public string Id => Model.Id;

        /// <inheritdoc />
        public string Name => Model.Name;

        /// <inheritdoc />
        public DeviceType Type => Model.Type;

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc cref="IDevice.NowPlaying"/>
        public PlaybackItem? NowPlaying
        {
            get => _nowPlaying;
            set => SetProperty(ref _nowPlaying, value);
        }

        /// <inheritdoc />
        public bool IsActive => Model.IsActive;

        /// <inheritdoc />
        public IPlayableBase? PlaybackContext => Model.PlaybackContext;

        /// <inheritdoc />
        public TimeSpan Position => Model.Position;

        /// <inheritdoc />
        public PlaybackState PlaybackState => Model.PlaybackState;

        /// <summary>
        /// Indicates if the device is currently playing.
        /// </summary>
        public bool IsPlaying => PlaybackState == PlaybackState.Playing;

        /// <inheritdoc />
        public bool ShuffleState => Model.ShuffleState;

        /// <inheritdoc />
        public RepeatState RepeatState => Model.RepeatState;

        /// <inheritdoc />
        public double Volume => Model.Volume;

        /// <inheritdoc />
        public double PlaybackSpeed => Model.PlaybackSpeed;

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncAvailable => Model.IsToggleShuffleAsyncAvailable;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncAvailable => Model.IsToggleRepeatAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncAvailable => Model.IsChangeVolumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedAvailable => Model.IsChangePlaybackSpeedAvailable;

        /// <inheritdoc />
        public bool IsResumeAsyncAvailable => Model.IsResumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => Model.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsNextAsyncAvailable => Model.IsNextAsyncAvailable;

        /// <inheritdoc />
        public bool IsPreviousAsyncAvailable => Model.IsPreviousAsyncAvailable;

        /// <inheritdoc />
        public bool IsSeekAsyncAvailable => Model.IsSeekAsyncAvailable;

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged
        {
            add => Model.IsActiveChanged += value;

            remove => Model.IsActiveChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IPlayableBase>? PlaybackContextChanged
        {
            add => Model.PlaybackContextChanged += value;

            remove => Model.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackItem>? NowPlayingChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged
        {
            add => Model.PositionChanged += value;

            remove => Model.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => Model.PlaybackStateChanged += value;

            remove => Model.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged
        {
            add => Model.ShuffleStateChanged += value;

            remove => Model.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged
        {
            add => Model.RepeatStateChanged += value;

            remove => Model.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged
        {
            add => Model.VolumeChanged += value;

            remove => Model.VolumeChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged
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

        private Task TogglePauseResume()
        {
            return IsPlaying ? PauseAsync() : ResumeAsync();
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
        public IAsyncRelayCommand<TimeSpan> SeekAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the device.
        /// </summary>
        public IAsyncRelayCommand ResumeAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the device.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// <see cref="PauseAsyncCommand"/> runs if playing, otherwise runs <see cref="ResumeAsync"/>.
        /// </summary>
        public IAsyncRelayCommand TogglePauseResumeCommand { get; }

        /// <summary>
        /// Attempts to move back to the previous track in the queue.
        /// </summary>
        public IAsyncRelayCommand PreviousAsyncCommand { get; }

        /// <summary>
        /// Attempts to skip to the next track in the queue.
        /// </summary>
        public IAsyncRelayCommand NextAsyncCommand { get; }

        /// <summary>
        /// Attempts to change volume.
        /// </summary>
        public IAsyncRelayCommand<double> ChangeVolumeAsyncCommand { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return Model.DisposeAsync();
        }
    }
}
