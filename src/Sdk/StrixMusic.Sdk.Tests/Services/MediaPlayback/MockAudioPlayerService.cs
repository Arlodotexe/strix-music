using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    internal class MockAudioPlayerService : IAudioPlayerService
    {
        public IMediaSourceConfig? CurrentSource { get; set; } = new MockMediaSourceConfig();

        public TimeSpan Position => TimeSpan.FromSeconds(10);

        public PlaybackState PlaybackState => PlaybackState.None;

        public double Volume => 100;

        public double PlaybackSpeed => 1;

        public event EventHandler<IMediaSourceConfig?>? CurrentSourceChanged;
        public event EventHandler<float[]>? QuantumProcessed;
        public event EventHandler<TimeSpan>? PositionChanged;
        public event EventHandler<PlaybackState>? PlaybackStateChanged;
        public event EventHandler<double>? VolumeChanged;
        public event EventHandler<double>? PlaybackSpeedChanged;

        public MockAudioPlayerService()
        {
            
        }

        public Task ChangePlaybackSpeedAsync(double speed)
        {
            return Task.CompletedTask;
        }

        public Task ChangeVolumeAsync(double volume)
        {
            return Task.CompletedTask;
        }

        public Task PauseAsync()
        {
            return Task.CompletedTask;
        }

        public Task Play(IMediaSourceConfig sourceConfig)
        {
            return Task.CompletedTask;
        }

        public Task Preload(IMediaSourceConfig sourceConfig)
        {
            return Task.CompletedTask;
        }

        public Task ResumeAsync()
        {
            return Task.CompletedTask;
        }

        public Task SeekAsync(TimeSpan position)
        {
            return Task.CompletedTask;
        }
    }
}
