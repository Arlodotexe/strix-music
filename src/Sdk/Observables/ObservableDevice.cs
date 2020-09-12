using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains information about a <see cref="IImage"/>
    /// </summary>
    public class ObservableDevice : ObservableObject
    {
        private readonly IDevice _device;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDevice"/> class.
        /// </summary>
        /// <param name="device">The base <see cref="IDevice"/></param>
        public ObservableDevice(IDevice device)
        {
            _device = device;

            if (_device.NowPlaying != null)
                NowPlaying = new ObservableTrack(_device.NowPlaying);

            PlaybackQueue = new ObservableCollection<ObservableTrack>(_device.PlaybackQueue.Select(x => new ObservableTrack(x)));

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
            _device.PlaybackQueueChanged += Device_PlaybackQueueChanged;
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
            _device.PlaybackQueueChanged -= Device_PlaybackQueueChanged;
        }

        private void Device_PlaybackQueueChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                PlaybackQueue.Insert(item.Index, new ObservableTrack(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                PlaybackQueue.RemoveAt(item.Index);
            }
        }

        private void Device_VolumePercentChanged(object sender, double e)
        {
            VolumePercent = e;
        }

        private void Device_StateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Device_ShuffleStateChanged(object sender, bool e)
        {
            ShuffleState = e;
        }

        private void Device_RepeatStateChanged(object sender, RepeatState e)
        {
            RepeatState = e;
        }

        private void Device_PositionChanged(object sender, TimeSpan e)
        {
            Position = e;
        }

        private void Device_PlaybackSpeedChanged(object sender, double e)
        {
            PlaybackSpeed = e;
        }

        private void Device_PlaybackContextChanged(object sender, IPlayableCollectionBase e)
        {
            PlaybackContext = e;
        }

        private void Device_NowPlayingChanged(object sender, ITrack e)
        {
            NowPlaying = new ObservableTrack(e);
        }

        private void Device_IsActiveChanged(object sender, bool e)
        {
            IsActive = e;
        }

        /// <inheritdoc cref="IDevice.SourceCore"/>
        public ICore SourceCore => _device.SourceCore;

        /// <inheritdoc cref="IDevice.Id"/>
        public string Id => _device.Id;

        /// <inheritdoc cref="IDevice.Name"/>
        public string Name => _device.Name;

        /// <inheritdoc cref="IDevice.IsActive"/>
        public bool IsActive
        {
            get => _device.IsActive;
            private set => SetProperty(() => _device.IsActive, value);
        }

        /// <inheritdoc cref="IDevice.PlaybackContext"/>
        public IPlayable PlaybackContext
        {
            get => _device.PlaybackContext;
            private set => SetProperty(() => _device.PlaybackContext, value);
        }

        /// <inheritdoc cref="IDevice.PlaybackQueue"/>
        public ObservableCollection<ObservableTrack> PlaybackQueue { get; }

        private ObservableTrack? _nowPlaying;

        /// <inheritdoc cref="IDevice.NowPlaying"/>
        public ObservableTrack? NowPlaying
        {
            get => _nowPlaying;
            private set => SetProperty(ref _nowPlaying, value);
        }

        /// <inheritdoc cref="IDevice.Position"/>
        public TimeSpan Position
        {
            get => _device.Position;
            private set => SetProperty(() => _device.Position, value);
        }

        /// <inheritdoc cref="IDevice.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _device.PlaybackState;
            private set => SetProperty(() => _device.PlaybackState, value);
        }

        /// <inheritdoc cref="IDevice.ShuffleState"/>
        public bool ShuffleState
        {
            get => _device.ShuffleState;
            private set => SetProperty(() => _device.ShuffleState, value);
        }

        /// <inheritdoc cref="IDevice.RepeatState"/>
        public RepeatState RepeatState
        {
            get => _device.RepeatState;
            private set => SetProperty(() => _device.RepeatState, value);
        }

        /// <inheritdoc cref="IDevice.SourceCore"/>
        public double VolumePercent
        {
            get => _device.VolumePercent;
            private set => SetProperty(() => _device.VolumePercent, value);
        }

        /// <inheritdoc cref="IDevice.DeviceType"/>
        public DeviceType DeviceType => _device.DeviceType;

        /// <inheritdoc cref="IDevice.PlaybackSpeed"/>
        public double? PlaybackSpeed
        {
            get => _device.PlaybackSpeed;
            private set => SetProperty(() => _device.PlaybackSpeed, value);
        }

        /// <inheritdoc cref="IDevice.IsActiveChanged"/>
        public event EventHandler<bool>? IsActiveChanged
        {
            add => _device.IsActiveChanged += value;

            remove => _device.IsActiveChanged -= value;
        }

        /// <inheritdoc cref="IDevice.PlaybackContextChanged"/>
        public event EventHandler<IPlayableCollectionBase> PlaybackContextChanged
        {
            add => _device.PlaybackContextChanged += value;

            remove => _device.PlaybackContextChanged -= value;
        }

        /// <inheritdoc cref="IDevice.NowPlayingChanged"/>
        public event EventHandler<ITrack> NowPlayingChanged
        {
            add => _device.NowPlayingChanged += value;

            remove => _device.NowPlayingChanged -= value;
        }

        /// <inheritdoc cref="IDevice.PositionChanged"/>
        public event EventHandler<TimeSpan> PositionChanged
        {
            add => _device.PositionChanged += value;

            remove => _device.PositionChanged -= value;
        }

        /// <inheritdoc cref="IDevice.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _device.PlaybackStateChanged += value;

            remove => _device.PlaybackStateChanged -= value;
        }

        /// <inheritdoc cref="IDevice.ShuffleStateChanged"/>
        public event EventHandler<bool> ShuffleStateChanged
        {
            add => _device.ShuffleStateChanged += value;

            remove => _device.ShuffleStateChanged -= value;
        }

        /// <inheritdoc cref="IDevice.RepeatStateChanged"/>
        public event EventHandler<RepeatState> RepeatStateChanged
        {
            add => _device.RepeatStateChanged += value;

            remove => _device.RepeatStateChanged -= value;
        }

        /// <inheritdoc cref="IDevice.VolumePercentChanged"/>
        public event EventHandler<double> VolumePercentChanged
        {
            add => _device.VolumePercentChanged += value;

            remove => _device.VolumePercentChanged -= value;
        }

        /// <inheritdoc cref="IDevice.PlaybackSpeedChanged"/>
        public event EventHandler<double> PlaybackSpeedChanged
        {
            add => _device.PlaybackSpeedChanged += value;

            remove => _device.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc cref="IDevice.PlaybackQueueChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>> PlaybackQueueChanged
        {
            add => _device.PlaybackQueueChanged += value;

            remove => _device.PlaybackQueueChanged -= value;
        }

        /// <inheritdoc cref="IDevice.ChangePlaybackSpeedAsync"/>
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            return _device.ChangePlaybackSpeedAsync(speed);
        }

        /// <inheritdoc cref="IDevice.ChangeVolumeAsync"/>
        public Task ChangeVolumeAsync(double volume)
        {
            return _device.ChangeVolumeAsync(volume);
        }

        /// <inheritdoc cref="IDevice.NextAsync"/>
        public Task NextAsync()
        {
            return _device.NextAsync();
        }

        /// <inheritdoc cref="IDevice.PauseAsync"/>
        public Task PauseAsync()
        {
            return _device.PauseAsync();
        }

        /// <inheritdoc cref="IDevice.PreviousAsync"/>
        public Task PreviousAsync()
        {
            return _device.PreviousAsync();
        }

        /// <inheritdoc cref="IDevice.ResumeAsync"/>
        public Task ResumeAsync()
        {
            return _device.ResumeAsync();
        }

        /// <inheritdoc cref="IDevice.SeekAsync"/>
        public Task SeekAsync(TimeSpan position)
        {
            return _device.SeekAsync(position);
        }

        /// <inheritdoc cref="IDevice.SwitchToAsync"/>
        public Task SwitchToAsync()
        {
            return _device.SwitchToAsync();
        }

        /// <inheritdoc cref="IDevice.ToggleRepeatAsync"/>
        public Task ToggleRepeatAsync()
        {
            return _device.ToggleRepeatAsync();
        }

        /// <inheritdoc cref="IDevice.ToggleShuffleAsync"/>
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
