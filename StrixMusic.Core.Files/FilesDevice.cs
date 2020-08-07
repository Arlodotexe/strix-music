using System;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Files
{
    public class FilesDevice : IDevice
    {
        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICore SourceCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IPlayableCollectionBase PlaybackContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TimeSpan Position => throw new NotImplementedException();

        public PlaybackState State => throw new NotImplementedException();

        public bool? ShuffleState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RepeatState RepeatState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double? VolumePercent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DeviceType DeviceType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PlaybackSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Previous()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public bool Seek(long position)
        {
            throw new NotImplementedException();
        }

        public void SwitchTo()
        {
            throw new NotImplementedException();
        }

        public void ToggleRepeat()
        {
            throw new NotImplementedException();
        }

        public void ToggleShuffle()
        {
            throw new NotImplementedException();
        }
    }
}
