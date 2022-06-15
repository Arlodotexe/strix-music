using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    internal class MockAudioPlayerService : IAudioPlayerService
    {
        public PlaybackItem? CurrentSource { get; set; } = new PlaybackItem();

        public TimeSpan Position => TimeSpan.FromSeconds(10);

        public PlaybackState PlaybackState => PlaybackState.None;

        public double Volume => 100;

        public double PlaybackSpeed => 1;

        public event EventHandler<PlaybackItem?>? CurrentSourceChanged;
        public event EventHandler<float[]>? QuantumProcessed;
        public event EventHandler<TimeSpan>? PositionChanged;
        public event EventHandler<PlaybackState>? PlaybackStateChanged;
        public event EventHandler<double>? VolumeChanged;
        public event EventHandler<double>? PlaybackSpeedChanged;

        public MockAudioPlayerService()
        {
            
        }

        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task PauseAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Play(PlaybackItem sourceConfig, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Preload(PlaybackItem sourceConfig, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task ResumeAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
