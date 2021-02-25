using System;
using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.MediaPlayback.LocalDevice
{
    /// <summary>
    /// The default playback device for the app.
    /// </summary>
    public class StrixDevice : IDevice
    {
        private readonly IPlaybackHandlerService _playbackHandler;

        /// <summary>
        /// Creates a new instance of <see cref="StrixDevice"/>.
        /// </summary>
        public StrixDevice(IPlaybackHandlerService playbackHandler)
        {
            _playbackHandler = playbackHandler;
            PlaybackContext = null;

            // TODO: Implement StrixPlaybackQueueCollection
            //PlaybackQueue = new StrixPlaybackQueueCollection();
            AttachEvents();
        }

        private void AttachEvents()
        {
            _playbackHandler.RepeatStateChanged += RepeatStateChanged;
            _playbackHandler.ShuffleStateChanged += ShuffleStateChanged;
            _playbackHandler.PlaybackSpeedChanged += PlaybackSpeedChanged;
            _playbackHandler.PlaybackStateChanged += PlaybackStateChanged;
            _playbackHandler.PositionChanged += PositionChanged;
            _playbackHandler.VolumeChanged += VolumeChanged;
        }

        private void DetachEvents()
        {
            _playbackHandler.RepeatStateChanged -= RepeatStateChanged;
            _playbackHandler.ShuffleStateChanged -= ShuffleStateChanged;
            _playbackHandler.PlaybackSpeedChanged -= PlaybackSpeedChanged;
            _playbackHandler.PlaybackStateChanged -= PlaybackStateChanged;
            _playbackHandler.PositionChanged -= PositionChanged;
            _playbackHandler.VolumeChanged -= VolumeChanged;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged;

        /// <inheritdoc />
        public event EventHandler<IPlayableBase>? PlaybackContextChanged;

        /// <inheritdoc />
        public event EventHandler<ITrack>? NowPlayingChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged;

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc />
        public TimeSpan Position => _playbackHandler.Position;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _playbackHandler.PlaybackState;

        /// <inheritdoc />
        public double Volume => _playbackHandler.Volume;

        /// <inheritdoc />
        public double PlaybackSpeed => _playbackHandler.PlaybackSpeed;

        /// <inheritdoc />
        public Task ResumeAsync() => _playbackHandler.ResumeAsync();

        /// <inheritdoc />
        public Task PauseAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position) => _playbackHandler.SeekAsync(position);

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed) => _playbackHandler.ChangePlaybackSpeedAsync(speed);

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume) => _playbackHandler.ChangeVolumeAsync(volume);

        /// <inheritdoc />
        public string Id => "609EBD5A-EBA1-4DDE-9828-C72B096D35DF";

        /// <inheritdoc />
        public string Name => "This Device";

        /// <inheritdoc />
        public bool IsActive { get; private set; }

        /// <inheritdoc />
        public ICore? SourceCore { get; }

        /// <inheritdoc />
        public ICoreDevice? Source { get; }

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc />
        public IPlayableBase? PlaybackContext { get; private set; }

        /// <inheritdoc />
        public ITrack? NowPlaying { get; private set; }

        /// <inheritdoc />
        public DeviceType Type => DeviceType.Local;

        /// <inheritdoc />
        public bool ShuffleState => _playbackHandler.ShuffleState;

        /// <inheritdoc />
        public RepeatState RepeatState => _playbackHandler.RepeatState;

        /// <inheritdoc />
        public bool IsSeekAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsResumeAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedAvailable => true;

        /// <inheritdoc />
        public bool IsNextAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsPreviousAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncAvailable => true;

        /// <inheritdoc />
        public Task NextAsync() => _playbackHandler.NextAsync();

        /// <inheritdoc />
        public Task PreviousAsync() => _playbackHandler.PreviousAsync();

        /// <inheritdoc />
        public Task SwitchToAsync()
        {
            IsActive = true;
            IsActiveChanged?.Invoke(this, IsActive);

            // Maybe preload some things?
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets playback data for this device.
        /// </summary>
        /// <param name="playbackContext">The playback context.</param>
        /// <param name="nowPlaying">The track that is playing.</param>
        internal void SetPlaybackData(IPlayableBase playbackContext, ITrack nowPlaying)
        {
            PlaybackContext = playbackContext;
            PlaybackContextChanged?.Invoke(this, playbackContext);

            NowPlaying = nowPlaying;
            NowPlayingChanged?.Invoke(this, nowPlaying);
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync() => _playbackHandler.ToggleShuffleAsync();

        /// <inheritdoc />
        public Task ToggleRepeatAsync() => _playbackHandler.ToggleRepeatAsync();
    }
}