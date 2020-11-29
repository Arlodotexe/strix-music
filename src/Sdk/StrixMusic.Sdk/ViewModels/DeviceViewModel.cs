using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains information about a <see cref="IImage"/>
    /// </summary>
    public class DeviceViewModel : ObservableObject, IDevice
    {
        private TrackViewModel? _nowPlaying;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
        /// </summary>
        /// <param name="device">The base <see cref="IDevice"/></param>
        public DeviceViewModel(IDevice device)
        {
            Model = device ?? throw new ArgumentNullException(nameof(device));

            if (Model.NowPlaying != null)
                _nowPlaying = new TrackViewModel(Model.NowPlaying);

            SourceCores = device.GetSourceCores().Select(MainViewModel.GetLoadedCore).ToList();

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

        private void Device_VolumeChanged(object sender, double e) => OnPropertyChanged(nameof(Volume));

        private void Device_StateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void Device_ShuffleStateChanged(object sender, bool e) => OnPropertyChanged(nameof(ShuffleState));

        private void Device_RepeatStateChanged(object sender, RepeatState e) => OnPropertyChanged(nameof(RepeatState));

        private void Device_PositionChanged(object sender, TimeSpan e) => OnPropertyChanged(nameof(Position));

        private void Device_PlaybackSpeedChanged(object sender, double e) => OnPropertyChanged(nameof(PlaybackSpeed));

        private void Device_PlaybackContextChanged(object sender, IPlayable e) => OnPropertyChanged(nameof(PlaybackContext));

        private void Device_NowPlayingChanged(object sender, ITrack e)
        {
            OnPropertyChanged(nameof(NowPlaying));
            _nowPlaying = new TrackViewModel(e);
        }

        private void Device_IsActiveChanged(object sender, bool e) => OnPropertyChanged(nameof(IsActive));

        /// <summary>
        /// The wrapped model for this <see cref="DeviceViewModel"/>.
        /// </summary>
        internal IDevice Model { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The I sources that form this member.
        /// </summary>
        public IReadOnlyList<ICoreDevice> Sources => this.GetSources();

        /// <inheritdoc />
        IReadOnlyList<ICoreDevice> ISdkMember<ICoreDevice>.Sources => Sources;

        /// <inheritdoc />
        public string Id => Model.Id;

        /// <inheritdoc />
        public string Name => Model.Name;

        /// <inheritdoc />
        public DeviceType Type => Model.Type;

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc />
        public ITrack? NowPlaying => _nowPlaying;

        /// <inheritdoc />
        public bool IsActive => Model.IsActive;

        /// <inheritdoc />
        public IPlayable? PlaybackContext => Model.PlaybackContext;

        /// <inheritdoc />
        public TimeSpan Position => Model.Position;

        /// <inheritdoc />
        public PlaybackState PlaybackState => Model.PlaybackState;

        /// <inheritdoc />
        public bool ShuffleState => Model.ShuffleState;

        /// <inheritdoc />
        public RepeatState RepeatState => Model.RepeatState;

        /// <inheritdoc />
        public double Volume => Model.Volume;

        /// <inheritdoc />
        public double PlaybackSpeed => Model.PlaybackSpeed;

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
        public event EventHandler<ITrack> NowPlayingChanged
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
