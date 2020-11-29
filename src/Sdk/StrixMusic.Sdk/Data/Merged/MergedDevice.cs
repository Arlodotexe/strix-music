using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Translates a <see cref="ICoreDevice"/> to a <see cref="IDevice"/>.
    /// </summary>
    public class MergedDevice : IDevice
    {
        private readonly ICoreDevice _source;
        private readonly IReadOnlyList<ICoreDevice> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="MergedDevice"/>.
        /// </summary>
        /// <param name="source"></param>
        public MergedDevice(ICoreDevice source)
        {
            _source = source;
            _sources = _source.IntoList();
            
            SourceCores = _source.SourceCore.IntoList();

            Name = _source.Name;
            IsActive = _source.IsActive;
            PlaybackContext = _source.PlaybackContext;
            Position = _source.Position;
            PlaybackState = _source.PlaybackState;
            ShuffleState = _source.ShuffleState;
            RepeatState = _source.RepeatState;
            Volume = _source.Volume;
            PlaybackSpeed = _source.PlaybackSpeed;

            if (!(_source.PlaybackQueue is null))
                PlaybackQueue = new MergedTrackCollection(_source.PlaybackQueue.IntoList());

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
            NowPlaying = new MergedTrack(e.IntoList());
            NowPlayingChanged?.Invoke(sender, NowPlaying);
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged
        {
            add => _source.IsActiveChanged += value;
            remove => _source.IsActiveChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IPlayable> PlaybackContextChanged
        {
            add => _source.PlaybackContextChanged += value;
            remove => _source.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool> ShuffleStateChanged
        {
            add => _source.ShuffleStateChanged += value;
            remove => _source.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState> RepeatStateChanged
        {
            add => _source.RepeatStateChanged += value;
            remove => _source.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan> PositionChanged
        {
            add => _source.PositionChanged += value;
            remove => _source.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _source.PlaybackStateChanged += value;
            remove => _source.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double> VolumeChanged
        {
            add => _source.VolumeChanged += value;
            remove => _source.VolumeChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double> PlaybackSpeedChanged
        {
            add => _source.PlaybackSpeedChanged += value;
            remove => _source.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ITrack>? NowPlayingChanged;

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
        public IPlayable? PlaybackContext { get; internal set; }

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
        public bool IsSeekAsyncSupported => _source.IsSeekAsyncSupported;

        /// <inheritdoc />
        public bool IsResumeAsyncSupported => _source.IsResumeAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _source.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncSupported => _source.IsChangeVolumeAsyncSupported;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedSupported => _source.IsChangePlaybackSpeedSupported;

        /// <inheritdoc />
        public bool IsNextAsyncSupported => _source.IsNextAsyncSupported;

        /// <inheritdoc />
        public bool IsPreviousAsyncSupported => _source.IsPreviousAsyncSupported;

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncSupported => _source.IsToggleShuffleAsyncSupported;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncSupported => _source.IsToggleRepeatAsyncSupported;

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
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        IReadOnlyList<ICoreDevice> ISdkMember<ICoreDevice>.Sources => _sources;

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc />
        public ITrack? NowPlaying { get; private set; }
    }
}