using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.MediaPlayback
{
    /// <inheritdoc />
    public sealed class MediaPlayerElementAudioService : IAudioPlayerService
    {
        private readonly MediaPlayerElement _player;
        private readonly Dictionary<string, PlaybackItem> _preloadedSources;
        private readonly SynchronizationContext _synchronizationContext;
        private PlaybackItem? _currentSource;
        private PlaybackState _playbackState;

        /// <summary>
        /// Creates a new instance of <see cref="MediaPlayerElementAudioService"/>.
        /// </summary>
        /// <param name="player">The <see cref="MediaPlayerElement"/> to wrap around.</param>
        public MediaPlayerElementAudioService(MediaPlayerElement player)
        {
            _synchronizationContext = SynchronizationContext.Current ?? new();
            Guard.IsNotNull(player.MediaPlayer, nameof(player.MediaPlayer));

            _player = player;
            _preloadedSources = new Dictionary<string, PlaybackItem>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            _player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSessionOnPositionChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackRateChanged += PlaybackSessionOnPlaybackRateChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSessionOnPlaybackStateChanged;
            _player.MediaPlayer.VolumeChanged += MediaPlayerOnVolumeChanged;
            _player.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            _player.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private void DetachEvents()
        {
            _player.MediaPlayer.PlaybackSession.PositionChanged -= PlaybackSessionOnPositionChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackRateChanged -= PlaybackSessionOnPlaybackRateChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSessionOnPlaybackStateChanged;
            _player.MediaPlayer.VolumeChanged -= MediaPlayerOnVolumeChanged;
            _player.MediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
            _player.MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
        }

        private void PlaybackSessionOnPlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            PlaybackState = sender.PlaybackState switch
            {
                MediaPlaybackState.Playing => PlaybackState.Playing,
                MediaPlaybackState.Buffering => PlaybackState.Loading,
                MediaPlaybackState.Opening => PlaybackState.Loading,
                MediaPlaybackState.Paused => PlaybackState.Paused,
                MediaPlaybackState.None => PlaybackState.None,
                _ => PlaybackState.None,
            };
        }

        private void PlaybackSessionOnPositionChanged(MediaPlaybackSession sender, object args)
        {
            PositionChanged?.Invoke(this, sender.Position);
        }

        private void PlaybackSessionOnPlaybackRateChanged(MediaPlaybackSession sender, object args)
        {
            PlaybackSpeedChanged?.Invoke(this, sender.PlaybackRate);
        }

        private void MediaPlayerOnVolumeChanged(MediaPlayer sender, object args)
        {
            VolumeChanged?.Invoke(this, sender.Volume);
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            // Since the player itself can't be queued, we use this as a sentinel value for advancing the queue.
            PlaybackStateChanged?.Invoke(this, PlaybackState.Loaded);
        }

        private void MediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine(args.ErrorMessage);
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged;

        /// <inheritdoc />
        public event EventHandler<float[]>? QuantumProcessed;

        /// <inheritdoc/>
        public event EventHandler<PlaybackItem?>? CurrentSourceChanged;

        /// <inheritdoc />
        public PlaybackItem? CurrentSource
        {
            get => _currentSource;
            set
            {
                _currentSource = value;
                CurrentSourceChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        public TimeSpan Position => _player.MediaPlayer.PlaybackSession.Position;

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            private set
            {
                _playbackState = value;
                PlaybackStateChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        public double Volume => _player.MediaPlayer.Volume;

        /// <inheritdoc />
        public double PlaybackSpeed => _player.MediaPlayer.PlaybackSession.PlaybackRate;

        /// <inheritdoc />
        public async Task Play(PlaybackItem playbackItem, CancellationToken cancellationToken = default)
        {
            var sourceConfig = playbackItem.MediaConfig;

            Guard.IsNotNull(sourceConfig, nameof(sourceConfig));

            await _synchronizationContext.PostAsync(async () =>
            {
                if (sourceConfig.MediaSourceUri != null)
                {
                    if (sourceConfig.MediaSourceUri.IsFile)
                    {
                        var file = await StorageFile.GetFileFromPathAsync(sourceConfig.MediaSourceUri.LocalPath);

                        var source = MediaSource.CreateFromStorageFile(file);
                        _player.MediaPlayer.Source = source;
                    }
                    else
                    {
                        var source = MediaSource.CreateFromUri(sourceConfig.MediaSourceUri);
                        _player.MediaPlayer.Source = source;
                    }
                }
                else if (sourceConfig.FileStreamSource != null)
                {
                    var source = MediaSource.CreateFromStream(sourceConfig.FileStreamSource.AsRandomAccessStream(), sourceConfig.FileStreamContentType);
                    _player.MediaPlayer.Source = source;
                }

                CurrentSource = playbackItem;

                _player.MediaPlayer.Play();
            });
        }

        /// <inheritdoc />
        public Task Preload(PlaybackItem playbackItem, CancellationToken cancellationToken = default)
        {
            var sourceConfig = playbackItem.MediaConfig;

            Guard.IsNotNull(sourceConfig, nameof(sourceConfig));

            // TODO: preload the track
            _preloadedSources.Add(sourceConfig.Id, playbackItem);

            return Task.CompletedTask;
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PauseAsync(CancellationToken cancellationToken = default) => _synchronizationContext.PostAsync(async () => _player.MediaPlayer.Pause());

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task ResumeAsync(CancellationToken cancellationToken = default) => _synchronizationContext.PostAsync(async () => _player.MediaPlayer.Play());

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default) => _synchronizationContext.PostAsync(async () => _player.MediaPlayer.Volume = volume);

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default) => _synchronizationContext.PostAsync(async () => _player.MediaPlayer.PlaybackSession.PlaybackRate = speed);

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) => _synchronizationContext.PostAsync(async () => _player.MediaPlayer.PlaybackSession.Position = position);
    }
}
