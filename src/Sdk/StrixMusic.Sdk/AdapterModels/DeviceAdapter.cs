// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Translates a <see cref="ICoreDevice"/> to a <see cref="IDevice"/>. Does not provide merging.
    /// </summary>
    public sealed class DeviceAdapter : IDevice
    {
        private readonly ICoreDevice _source;

        /// <summary>
        /// Creates a new instance of <see cref="DeviceAdapter"/>.
        /// </summary>
        /// <param name="source"></param>
        public DeviceAdapter(ICoreDevice source)
        {
            _source = source;

            Name = _source.Name;
            IsActive = _source.IsActive;
            PlaybackContext = _source.PlaybackContext;
            Position = _source.Position;
            PlaybackState = _source.PlaybackState;
            ShuffleState = _source.ShuffleState;
            RepeatState = _source.RepeatState;
            Volume = _source.Volume;
            PlaybackSpeed = _source.PlaybackSpeed;
            
            Guard.IsNotNull(_source.NowPlaying,nameof(_source.NowPlaying));

            var nowPlaying = new MergedTrack(_source.NowPlaying.IntoList(), new MergedCollectionConfig());

            NowPlaying = new PlaybackItem()
            {
                Track = nowPlaying,
            };

            if (!(_source.PlaybackQueue is null))
                PlaybackQueue = new MergedTrackCollection(_source.PlaybackQueue.IntoList(), new MergedCollectionConfig());

            AttachEvents();
        }

        private void AttachEvents()
        {
            _source.NowPlayingChanged += Source_NowPlayingChanged;
        }

        private void DetachEvents()
        {
            _source.NowPlayingChanged -= Source_NowPlayingChanged;
        }

        private void Source_NowPlayingChanged(object sender, ICoreTrack e)
        {
            var nowPlaying = new MergedTrack(e.IntoList(), new MergedCollectionConfig());

            NowPlaying = new PlaybackItem
            {
                Track = nowPlaying,
            };

            NowPlayingChanged?.Invoke(sender, NowPlaying);
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsActiveChanged
        {
            add => _source.IsActiveChanged += value;
            remove => _source.IsActiveChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IPlayableBase>? PlaybackContextChanged
        {
            add => _source.PlaybackContextChanged += value;
            remove => _source.PlaybackContextChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged
        {
            add => _source.ShuffleStateChanged += value;
            remove => _source.ShuffleStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged
        {
            add => _source.RepeatStateChanged += value;
            remove => _source.RepeatStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged
        {
            add => _source.PositionChanged += value;
            remove => _source.PositionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _source.PlaybackStateChanged += value;
            remove => _source.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged
        {
            add => _source.VolumeChanged += value;
            remove => _source.VolumeChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged
        {
            add => _source.PlaybackSpeedChanged += value;
            remove => _source.PlaybackSpeedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackItem>? NowPlayingChanged;

        /// <inheritdoc />
        public string Id => _source.Id;

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public bool IsActive { get; internal set; }

        /// <inheritdoc />
        public DeviceType Type { get; internal set; }

        /// <inheritdoc />
        public double PlaybackSpeed { get; internal set; }

        /// <inheritdoc />
        public IPlayableBase? PlaybackContext { get; internal set; }

        /// <inheritdoc />
        public double Volume { get; internal set; }

        /// <inheritdoc />
        public bool ShuffleState { get; internal set; }

        /// <inheritdoc />
        public RepeatState RepeatState { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Position { get; internal set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc />
        public bool IsSeekAsyncAvailable => _source.IsSeekAsyncAvailable;

        /// <inheritdoc />
        public bool IsResumeAsyncAvailable => _source.IsResumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _source.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeVolumeAsyncAvailable => _source.IsChangeVolumeAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangePlaybackSpeedAvailable => _source.IsChangePlaybackSpeedAvailable;

        /// <inheritdoc />
        public bool IsNextAsyncAvailable => _source.IsNextAsyncAvailable;

        /// <inheritdoc />
        public bool IsPreviousAsyncAvailable => _source.IsPreviousAsyncAvailable;

        /// <inheritdoc />
        public bool IsToggleShuffleAsyncAvailable => _source.IsToggleShuffleAsyncAvailable;

        /// <inheritdoc />
        public bool IsToggleRepeatAsyncAvailable => _source.IsToggleRepeatAsyncAvailable;

        /// <inheritdoc />
        public Task NextAsync(CancellationToken cancellationToken = default) => _source.NextAsync(cancellationToken);

        /// <inheritdoc />
        public Task PreviousAsync(CancellationToken cancellationToken = default) => _source.PreviousAsync(cancellationToken);

        /// <inheritdoc />
        public Task ToggleShuffleAsync(CancellationToken cancellationToken = default) => _source.ToggleShuffleAsync(cancellationToken);

        /// <inheritdoc />
        public Task ToggleRepeatAsync(CancellationToken cancellationToken = default) => _source.ToggleRepeatAsync(cancellationToken);

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) => _source.SeekAsync(position, cancellationToken);

        /// <inheritdoc />
        public Task SwitchToAsync(CancellationToken cancellationToken = default) => _source.SwitchToAsync(cancellationToken);

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default) => _source.ChangePlaybackSpeedAsync(speed, cancellationToken);

        /// <inheritdoc />
        public Task ResumeAsync(CancellationToken cancellationToken = default) => _source.ResumeAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseAsync(CancellationToken cancellationToken = default) => _source.PauseAsync(cancellationToken);

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default) => _source.ChangeVolumeAsync(volume, cancellationToken);

        /// <inheritdoc />
        public ICore? SourceCore => _source.SourceCore;

        /// <inheritdoc />
        public ICoreDevice? Source => _source;

        /// <inheritdoc />
        public ITrackCollection? PlaybackQueue { get; }

        /// <inheritdoc />
        public PlaybackItem? NowPlaying { get; private set; }

        /// <inheritdoc />
        public ValueTask DisposeAsync() => _source.DisposeAsync();
    }
}
