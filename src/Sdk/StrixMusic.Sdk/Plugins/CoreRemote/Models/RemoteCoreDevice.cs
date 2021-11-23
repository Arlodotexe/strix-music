using OwlCore.Remoting;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICoreDevice"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    public class RemoteCoreDevice : ICoreDevice
    {
        private MemberRemote _memberRemote;

        /// <summary>
        /// Creates a new instance of a <see cref="RemoteCoreDevice"/>.
        /// </summary>
        internal RemoteCoreDevice(string sourceCoreInstanceId, string id)
        {
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId);

            Id = id;
            Name = string.Empty;

            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{nameof(RemoteCoreDevice)}.{id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="RemoteCoreDevice"/>.
        /// </summary>
        internal RemoteCoreDevice(ICoreDevice device)
        {
            SourceCore = RemoteCore.GetInstance(device.SourceCore.InstanceId);

            _memberRemote = new MemberRemote(this, $"{device.SourceCore.InstanceId}.{nameof(RemoteCoreDevice)}.{device.Id}", RemoteCoreMessageHandler.SingletonHost);

            Id = device.Id;
            Name = device.Name;
        }

        /// <inheritdoc/>
        [RemoteProperty]
        public string Id { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public string Name { get; set; }

        /// <inheritdoc/>
        public ICoreTrackCollection? PlaybackQueue { get; set; }

        /// <inheritdoc/>
        public ICoreTrack? NowPlaying { get; set; }

        /// <inheritdoc/>
        public ICore SourceCore { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public IPlayableBase? PlaybackContext { get; set; }

        /// <inheritdoc/>
        public DeviceType Type { get; set; }

        /// <inheritdoc/>
        public bool ShuffleState { get; set; }

        /// <inheritdoc/>
        public RepeatState RepeatState { get; set; }

        /// <inheritdoc/>
        public bool IsSeekAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsResumeAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsPauseAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeVolumeAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangePlaybackSpeedAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsNextAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsPreviousAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsToggleShuffleAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsToggleRepeatAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public TimeSpan Position { get; set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; set; }

        /// <inheritdoc/>
        public double Volume { get; set; }

        /// <inheritdoc/>
        public double PlaybackSpeed { get; set; }

        /// <inheritdoc/>
        public event EventHandler<ICoreTrack>? NowPlayingChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsActiveChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableBase>? PlaybackContextChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? ShuffleStateChanged;

        /// <inheritdoc/>
        public event EventHandler<RepeatState>? RepeatStateChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<double>? VolumeChanged;

        /// <inheritdoc/>
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc/>
        public Task ChangePlaybackSpeedAsync(double speed)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeVolumeAsync(double volume)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task NextAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PreviousAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ResumeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SeekAsync(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SwitchToAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ToggleRepeatAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ToggleShuffleAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            _memberRemote.Dispose();
            return default;
        }
    }
}
