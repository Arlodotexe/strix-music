using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;

// ReSharper disable once CheckNamespace
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
namespace StrixMusic.Shared.Services
{
    /// <inheritdoc />
    public class AudioPlayerService : IAudioPlayerService
    {
        private readonly MediaPlayerElement _player;
        private readonly Dictionary<string, IMediaSourceConfig> _preloadedSources;

        /// <summary>
        /// Creates a new instance of <see cref="AudioPlayerService"/>.
        /// </summary>
        /// <param name="player">The <see cref="MediaPlayerElement"/> to wrap around.</param>
        public AudioPlayerService(MediaPlayerElement player)
        {
            if (player.MediaPlayer is null)
                throw new ArgumentException(@"MediaPlayer is not present", nameof(player));

            _player = player;
            _preloadedSources = new Dictionary<string, IMediaSourceConfig>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            _player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSessionOnPositionChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackRateChanged += PlaybackSessionOnPlaybackRateChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSessionOnPlaybackStateChanged;
            _player.MediaPlayer.VolumeChanged += MediaPlayerOnVolumeChanged;
        }

        private void DetachEvents()
        {
            _player.MediaPlayer.PlaybackSession.PositionChanged -= PlaybackSessionOnPositionChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackRateChanged -= PlaybackSessionOnPlaybackRateChanged;
            _player.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSessionOnPlaybackStateChanged;
            _player.MediaPlayer.VolumeChanged -= MediaPlayerOnVolumeChanged;
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

            PlaybackStateChanged?.Invoke(this, PlaybackState);
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

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged;

        /// <inheritdoc />
        public IMediaSourceConfig? CurrentSource { get; set; }

        /// <inheritdoc />
        public TimeSpan Position => _player.MediaPlayer.PlaybackSession.Position;

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; private set; }

        /// <inheritdoc />
        public double Volume => _player.MediaPlayer.Volume;

        /// <inheritdoc />
        public double PlaybackSpeed => _player.MediaPlayer.PlaybackSession.PlaybackRate;

        /// <inheritdoc />
        public Task Play(string id)
        {
            var source = _preloadedSources[id];

            // TODO use preloaded
            return Play(source);
        }

        /// <inheritdoc />
        public Task Play(IMediaSourceConfig sourceConfig)
        {
            if (sourceConfig.MediaSourceUri != null)
            {
                _player.MediaPlayer.SetUriSource(sourceConfig.MediaSourceUri);
            }
            else if (sourceConfig.FileStreamSource != null)
            {
                _player.MediaPlayer.SetStreamSource(sourceConfig.FileStreamSource.AsRandomAccessStream());
            }

            _player.MediaPlayer.Play();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Preload(IMediaSourceConfig sourceConfig)
        {
            // TODO: preload the track
            _preloadedSources.Add(sourceConfig.Id, sourceConfig);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            _player.MediaPlayer.Pause();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ResumeAsync()
        {
            _player.MediaPlayer.Play();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume)
        {
            _player.MediaPlayer.Volume = volume;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            _player.MediaPlayer.PlaybackSession.PlaybackRate = speed;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position)
        {
            _player.MediaPlayer.PlaybackSession.Position = position;

            return Task.CompletedTask;
        }
    }
}