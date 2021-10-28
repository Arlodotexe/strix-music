using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    internal class MockAudioPlayerService : IAudioPlayerService
    {
        public IMediaSourceConfig CurrentSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TimeSpan Position => throw new NotImplementedException();

        public PlaybackState PlaybackState => throw new NotImplementedException();

        public double Volume => throw new NotImplementedException();

        public double PlaybackSpeed => throw new NotImplementedException();

        public event EventHandler<IMediaSourceConfig> CurrentSourceChanged;
        public event EventHandler<float[]> QuantumProcessed;
        public event EventHandler<TimeSpan> PositionChanged;
        public event EventHandler<PlaybackState> PlaybackStateChanged;
        public event EventHandler<double> VolumeChanged;
        public event EventHandler<double> PlaybackSpeedChanged;

        public Task ChangePlaybackSpeedAsync(double speed)
        {
            throw new NotImplementedException();
        }

        public Task ChangeVolumeAsync(double volume)
        {
            throw new NotImplementedException();
        }

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task Play(string id)
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaSourceConfig sourceConfig)
        {
            throw new NotImplementedException();
        }

        public Task Preload(IMediaSourceConfig sourceConfig)
        {
            throw new NotImplementedException();
        }

        public Task ResumeAsync()
        {
            throw new NotImplementedException();
        }

        public Task SeekAsync(TimeSpan position)
        {
            throw new NotImplementedException();
        }
    }
}
