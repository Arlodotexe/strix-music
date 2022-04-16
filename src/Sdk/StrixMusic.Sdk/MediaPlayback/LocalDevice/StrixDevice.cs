// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.MediaPlayback.LocalDevice
{
    /// <summary>
    /// The default playback device for the app.
    /// </summary>
    public sealed class StrixDevice : IDevice
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
            _playbackHandler.RepeatStateChanged += PlaybackHandler_RepeatStateChanged;
            _playbackHandler.ShuffleStateChanged += PlaybackHandler_ShuffleStateChanged;
            _playbackHandler.PlaybackSpeedChanged += PlaybackHandler_PlaybackSpeedChanged;
            _playbackHandler.PlaybackStateChanged += PlaybackHandler_PlaybackStateChanged;
            _playbackHandler.PositionChanged += PlaybackHandler_PositionChanged;
            _playbackHandler.VolumeChanged += PlaybackHandler_VolumeChanged;
            _playbackHandler.CurrentItemChanged += PlaybackHandler_CurrentItemChanged;
        }

        private void DetachEvents()
        {
            _playbackHandler.RepeatStateChanged -= PlaybackHandler_RepeatStateChanged;
            _playbackHandler.ShuffleStateChanged -= PlaybackHandler_ShuffleStateChanged;
            _playbackHandler.PlaybackSpeedChanged -= PlaybackHandler_PlaybackSpeedChanged;
            _playbackHandler.PlaybackStateChanged -= PlaybackHandler_PlaybackStateChanged;
            _playbackHandler.PositionChanged -= PlaybackHandler_PositionChanged;
            _playbackHandler.VolumeChanged -= PlaybackHandler_VolumeChanged;
            _playbackHandler.CurrentItemChanged -= PlaybackHandler_CurrentItemChanged;
        }

        private void PlaybackHandler_CurrentItemChanged(object sender, PlaybackItem? e)
        {
            Guard.IsNotNull(e, nameof(e));
            Guard.IsNotNull(PlaybackContext, nameof(PlaybackContext));

            SetPlaybackData(PlaybackContext, e);
        }

        private void PlaybackHandler_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackStateChanged?.Invoke(sender, e);

        private void PlaybackHandler_PositionChanged(object sender, TimeSpan e) => PositionChanged?.Invoke(sender, e);

        private void PlaybackHandler_VolumeChanged(object sender, double e) => VolumeChanged?.Invoke(this, e);

        private void PlaybackHandler_PlaybackSpeedChanged(object sender, double e) => PlaybackSpeedChanged?.Invoke(sender, e);

        private void PlaybackHandler_RepeatStateChanged(object sender, RepeatState e) => RepeatStateChanged?.Invoke(sender, e);

        private void PlaybackHandler_ShuffleStateChanged(object sender, bool e) => ShuffleStateChanged?.Invoke(sender, e);

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged;

        /// <inheritdoc />
        public event EventHandler<IPlayableBase>? PlaybackContextChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackItem>? NowPlayingChanged;

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
        public Task ResumeAsync(CancellationToken cancellationToken = default) => _playbackHandler.ResumeAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseAsync(CancellationToken cancellationToken = default) => _playbackHandler.PauseAsync(cancellationToken);

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) => _playbackHandler.SeekAsync(position, cancellationToken);

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default) => _playbackHandler.ChangePlaybackSpeedAsync(speed, cancellationToken);

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default) => _playbackHandler.ChangeVolumeAsync(volume, cancellationToken);

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
        public PlaybackItem? NowPlaying { get; private set; }

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
        public Task NextAsync(CancellationToken cancellationToken = default) => _playbackHandler.NextAsync(cancellationToken);

        /// <inheritdoc />
        public Task PreviousAsync(CancellationToken cancellationToken = default) => _playbackHandler.PreviousAsync(cancellationToken);

        /// <inheritdoc />
        public Task SwitchToAsync(CancellationToken cancellationToken = default)
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
        internal void SetPlaybackData(IPlayableBase playbackContext, PlaybackItem nowPlaying)
        {
            PlaybackContext = playbackContext;
            PlaybackContextChanged?.Invoke(this, playbackContext);

            NowPlaying = nowPlaying;
            NowPlayingChanged?.Invoke(this, nowPlaying);
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync(CancellationToken cancellationToken = default) => _playbackHandler.ToggleShuffleAsync(cancellationToken);

        /// <inheritdoc />
        public Task ToggleRepeatAsync(CancellationToken cancellationToken = default) => _playbackHandler.ToggleRepeatAsync(cancellationToken);

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return default;
        }
    }
}
