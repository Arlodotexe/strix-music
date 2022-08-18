// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="IDevice"/>.
    /// </summary>
    public sealed class DeviceViewModel : ObservableObject, ISdkViewModel, IDevice
    {
        private readonly SynchronizationContext _syncContext;
        private readonly IDevice _model;
        private PlaybackItem? _nowPlaying;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
        /// </summary>
        /// <param name="device">The <see cref="IDevice"/> to wrap around.</param>
        public DeviceViewModel(IDevice device)
        {
            _syncContext = SynchronizationContext.Current;

            _model = device;

            if (_model.NowPlaying != null)
                _nowPlaying = _model.NowPlaying;

            if (device.SourceCore != null)
                SourceCore = new CoreViewModel(device.SourceCore);

            if (_model.PlaybackQueue != null)
                PlaybackQueue = new TrackCollectionViewModel(_model.PlaybackQueue);

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
            _model.IsActiveChanged += Device_IsActiveChanged;
            _model.NowPlayingChanged += Device_NowPlayingChanged;
            _model.PlaybackContextChanged += Device_PlaybackContextChanged;
            _model.PlaybackSpeedChanged += Device_PlaybackSpeedChanged;
            _model.PositionChanged += Device_PositionChanged;
            _model.RepeatStateChanged += Device_RepeatStateChanged;
            _model.ShuffleStateChanged += Device_ShuffleStateChanged;
            _model.PlaybackStateChanged += Device_StateChanged;
            _model.VolumeChanged += Device_VolumeChanged;
        }

        private void DetachEvents()
        {
            _model.IsActiveChanged -= Device_IsActiveChanged;
            _model.NowPlayingChanged -= Device_NowPlayingChanged;
            _model.PlaybackContextChanged -= Device_PlaybackContextChanged;
            _model.PlaybackSpeedChanged -= Device_PlaybackSpeedChanged;
            _model.PositionChanged -= Device_PositionChanged;
            _model.RepeatStateChanged -= Device_RepeatStateChanged;
            _model.ShuffleStateChanged -= Device_ShuffleStateChanged;
            _model.PlaybackStateChanged -= Device_StateChanged;
            _model.VolumeChanged -= Device_VolumeChanged;
        }

        private void Device_VolumeChanged(object sender, double e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Volume)), null);

        private void Device_ShuffleStateChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(ShuffleState)), null);

        private void Device_RepeatStateChanged(object sender, RepeatState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(RepeatState)), null);

        private void Device_PositionChanged(object sender, TimeSpan e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Position)), null);

        private void Device_PlaybackSpeedChanged(object sender, double e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackSpeed)), null);

        private void Device_PlaybackContextChanged(object sender, IPlayableBase? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackContext)), null);

        private void Device_IsActiveChanged(object sender, bool e)
        {
            _syncContext.Post(_ => OnPropertyChanged(nameof(IsActive)), null);
            IsActiveChanged?.Invoke(this, e);
        }

        private void Device_StateChanged(object sender, PlaybackState e) => _syncContext.Post(_ =>
        {
            OnPropertyChanged(nameof(PlaybackState));
            OnPropertyChanged(nameof(IsPlaying));
        }, null);

        private void Device_NowPlayingChanged(object sender, PlaybackItem e) => _syncContext.Post(_ =>
        {
            Guard.IsNotNull(e.Track, nameof(e.Track));

            NowPlaying = e with
            {
                Track = new TrackViewModel(e.Track)
            };

            NowPlayingChanged?.Invoke(sender, e);
        }, null);

        /// <inheritdoc />
        public ICore? SourceCore { get; set; }

        /// <inheritdoc />
        public ICoreDevice? Source => _model.Source;

        /// <inheritdoc />
        public string Id => _model.Id;

        /// <inheritdoc />
        public string Name => _model.Name;

        /// <inheritdoc />
        public DeviceType Type => _model.Type;

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc cref="IDevice.NowPlaying"/>
        public PlaybackItem? NowPlaying
        {
            get => _nowPlaying;
            set => SetProperty(ref _nowPlaying, value);
        }

        /// <inheritdoc />
        public bool IsActive => _model.IsActive;

        /// <inheritdoc />
        public IPlayableBase? PlaybackContext => _model.PlaybackContext;

        /// <inheritdoc />
        public TimeSpan Position => _model.Position;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _model.PlaybackState;

        /// <summary>
        /// Indicates if the device is currently playing.
        /// </summary>
        public bool IsPlaying => PlaybackState == PlaybackState.Playing;

        /// <inheritdoc />
        public bool ShuffleState => _model.ShuffleState;

        /// <inheritdoc />
        public RepeatState RepeatState => _model.RepeatState;

        /// <inheritdoc />
        public double Volume => _model.Volume;

        /// <inheritdoc />
        public double PlaybackSpeed => _model.PlaybackSpeed;

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncAvailable => _model.IsToggleShuffleAsyncAvailable;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncAvailable => _model.IsToggleRepeatAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncAvailable => _model.IsChangeVolumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedAvailable => _model.IsChangePlaybackSpeedAvailable;

        /// <inheritdoc />
        public bool IsResumeAsyncAvailable => _model.IsResumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _model.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsNextAsyncAvailable => _model.IsNextAsyncAvailable;

        /// <inheritdoc />
        public bool IsPreviousAsyncAvailable => _model.IsPreviousAsyncAvailable;

        /// <inheritdoc />
        public bool IsSeekAsyncAvailable => _model.IsSeekAsyncAvailable;

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged;

        /// <inheritdoc />
        public event EventHandler<IPlayableBase?>? PlaybackContextChanged
        {
            add => _model.PlaybackContextChanged += value;
            remove => _model.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackItem>? NowPlayingChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged
        {
            add => _model.PositionChanged += value;
            remove => _model.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _model.PlaybackStateChanged += value;
            remove => _model.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged
        {
            add => _model.ShuffleStateChanged += value;
            remove => _model.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged
        {
            add => _model.RepeatStateChanged += value;
            remove => _model.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged
        {
            add => _model.VolumeChanged += value;
            remove => _model.VolumeChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged
        {
            add => _model.PlaybackSpeedChanged += value;
            remove => _model.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default) => _model.ChangePlaybackSpeedAsync(speed, cancellationToken);

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default) => _model.ChangeVolumeAsync(volume, cancellationToken);

        /// <inheritdoc />
        public Task NextAsync(CancellationToken cancellationToken = default) => _model.NextAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseAsync(CancellationToken cancellationToken = default) => _model.PauseAsync(cancellationToken);

        /// <inheritdoc />
        public Task PreviousAsync(CancellationToken cancellationToken = default) => _model.PreviousAsync(cancellationToken);

        /// <inheritdoc />
        public Task ResumeAsync(CancellationToken cancellationToken = default) => _model.ResumeAsync(cancellationToken);

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) => _model.SeekAsync(position, cancellationToken);

        /// <inheritdoc />
        public Task SwitchToAsync(CancellationToken cancellationToken = default) => _model.SwitchToAsync(cancellationToken);

        /// <inheritdoc />
        public Task ToggleRepeatAsync(CancellationToken cancellationToken = default) => _model.ToggleRepeatAsync(cancellationToken);

        /// <inheritdoc />
        public Task ToggleShuffleAsync(CancellationToken cancellationToken = default) => _model.ToggleShuffleAsync(cancellationToken);

        private Task TogglePauseResume(CancellationToken cancellationToken = default) => IsPlaying ? PauseAsync(cancellationToken) : ResumeAsync(cancellationToken);

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
    }
}
