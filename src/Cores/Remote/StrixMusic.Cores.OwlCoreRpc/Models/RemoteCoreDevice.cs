// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OwlCore.Remoting;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICoreDevice"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    public sealed class RemoteCoreDevice : ICoreDevice
    {
        private MemberRemote _memberRemote;

        /// <summary>
        /// Creates a new instance of a <see cref="RemoteCoreDevice"/>.
        /// </summary>
        [JsonConstructor]
        public RemoteCoreDevice(string sourceCoreInstanceId, string id)
        {
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client);

            Id = id;
            Name = string.Empty;

            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{nameof(RemoteCoreDevice)}.{id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="RemoteCoreDevice"/>.
        /// </summary>
        internal RemoteCoreDevice(ICoreDevice device)
        {
            SourceCore = RemoteCore.GetInstance(device.SourceCore.InstanceId, RemotingMode.Host);

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
        public event EventHandler<IPlayableBase?>? PlaybackContextChanged;

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
        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task NextAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task PauseAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task PreviousAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task ResumeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task SwitchToAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task ToggleRepeatAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task ToggleShuffleAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync() => new ValueTask(Task.Run(() =>
        {
            _memberRemote.Dispose();
            return Task.CompletedTask;
        }));
    }
}
