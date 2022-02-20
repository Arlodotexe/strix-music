using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// Translates a <see cref="ICoreDevice"/> to a <see cref="IDevice"/>. Does not provide merging.
    /// </summary>
    public sealed class CoreDeviceProxy : IDevice
    {
        private readonly ICoreDevice _source;
        private readonly ISettingsService _serviceProvider;

        /// <summary>
        /// Creates a new instance of <see cref="CoreDeviceProxy"/>.
        /// </summary>
        /// <param name="source"></param>
        public CoreDeviceProxy(ICoreDevice source, ISettingsService settingsService)
        {
            _source = source;

            Name = _source.Name;
            IsActive = _source.IsActive;
            PlaybackContext = _source.PlaybackContext;
            Position = _source.Position;
            PlaybackState = _source.PlaybackState;
            ShuffleState = _source.ShuffleState;
            RepeatState = _source.RepeatState;
            Volume = _source.Volume;
            PlaybackSpeed = _source.PlaybackSpeed;

            _serviceProvider = settingsService;

            Guard.IsNotNull(_source.NowPlaying,nameof(_source.NowPlaying));

            var nowPlaying = new MergedTrack(_source.NowPlaying.IntoList(), settingsService);

            NowPlaying = new PlaybackItem()
            {
                Track = nowPlaying,
            };

            if (!(_source.PlaybackQueue is null))
                PlaybackQueue = new MergedTrackCollection(_source.PlaybackQueue.IntoList(), settingsService);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _source.NowPlayingChanged += Source_NowPlayingChanged;
        }

        private void DetachEvents()
        {
            _source.NowPlayingChanged -= Source_NowPlayingChanged;
        }

        private void Source_NowPlayingChanged(object sender, ICoreTrack e)
        {
            var nowPlaying = new MergedTrack(e.IntoList(), _serviceProvider);

            NowPlaying = new PlaybackItem()
            {
                Track = nowPlaying,
            };

            NowPlayingChanged?.Invoke(sender, NowPlaying);
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged
        {
            add => _source.IsActiveChanged += value;
            remove => _source.IsActiveChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IPlayableBase>? PlaybackContextChanged
        {
            add => _source.PlaybackContextChanged += value;
            remove => _source.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged
        {
            add => _source.ShuffleStateChanged += value;
            remove => _source.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged
        {
            add => _source.RepeatStateChanged += value;
            remove => _source.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged
        {
            add => _source.PositionChanged += value;
            remove => _source.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _source.PlaybackStateChanged += value;
            remove => _source.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged
        {
            add => _source.VolumeChanged += value;
            remove => _source.VolumeChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged
        {
            add => _source.PlaybackSpeedChanged += value;
            remove => _source.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackItem>? NowPlayingChanged;

        /// <inheritdoc />
        public string Id => _source.Id;

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public bool IsActive { get; internal set; }

        /// <inheritdoc />
        public DeviceType Type { get; internal set; }

        /// <inheritdoc />
        public double PlaybackSpeed { get; internal set; }

        /// <inheritdoc />
        public IPlayableBase? PlaybackContext { get; internal set; }

        /// <inheritdoc />
        public double Volume { get; internal set; }

        /// <inheritdoc />
        public bool ShuffleState { get; internal set; }

        /// <inheritdoc />
        public RepeatState RepeatState { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Position { get; internal set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc />
        public bool IsSeekAsyncAvailable => _source.IsSeekAsyncAvailable;

        /// <inheritdoc />
        public bool IsResumeAsyncAvailable => _source.IsResumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _source.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncAvailable => _source.IsChangeVolumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedAvailable => _source.IsChangePlaybackSpeedAvailable;

        /// <inheritdoc />
        public bool IsNextAsyncAvailable => _source.IsNextAsyncAvailable;

        /// <inheritdoc />
        public bool IsPreviousAsyncAvailable => _source.IsPreviousAsyncAvailable;

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncAvailable => _source.IsToggleShuffleAsyncAvailable;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncAvailable => _source.IsToggleRepeatAsyncAvailable;

        /// <inheritdoc />
        public Task NextAsync() => _source.NextAsync();

        /// <inheritdoc />
        public Task PreviousAsync() => _source.PreviousAsync();

        /// <inheritdoc />
        public Task ToggleShuffleAsync() => _source.ToggleShuffleAsync();

        /// <inheritdoc />
        public Task ToggleRepeatAsync() => _source.ToggleRepeatAsync();

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position) => _source.SeekAsync(position);

        /// <inheritdoc />
        public Task SwitchToAsync() => _source.SwitchToAsync();

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed) => _source.ChangePlaybackSpeedAsync(speed);

        /// <inheritdoc />
        public Task ResumeAsync() => _source.ResumeAsync();

        /// <inheritdoc />
        public Task PauseAsync() => _source.PauseAsync();

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume) => _source.ChangeVolumeAsync(volume);

        /// <inheritdoc />
        public ICore? SourceCore => _source.SourceCore;

        /// <inheritdoc />
        public ICoreDevice? Source => _source;

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc />
        public PlaybackItem? NowPlaying { get; private set; }

        /// <inheritdoc />
        public ValueTask DisposeAsync() => _source.DisposeAsync();
    }
}