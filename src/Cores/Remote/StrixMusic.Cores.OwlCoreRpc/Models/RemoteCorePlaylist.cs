// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICoreArtist"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    public sealed class RemoteCorePlaylist : ICorePlaylist
    {
        private readonly MemberRemote _memberRemote;
        private readonly ICorePlaylist? _playlist;

        private string _name;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlaylist"/>.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The instance ID of the core that created this object.</param>
        /// <param name="id">A unique identifier for this instance.</param>
        /// <param name="name">The name of the data.</param>
        internal RemoteCorePlaylist(string sourceCoreInstanceId, string name, string id)
        {
            _name = name;
            Id = id;

            // Properties assigned before MemberRemote is created won't be set remotely.
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client); // should be set remotely by the ctor.

            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{nameof(RemoteCorePlaylist)}.{id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Wraps around and remotely relays events, property changes and method calls (with return data) from an playlist instance.
        /// </summary>
        /// <param name="corePlaylist"></param>
        internal RemoteCorePlaylist(ICorePlaylist corePlaylist)
        {
            _playlist = corePlaylist;
            _name = corePlaylist.Name;
            Id = corePlaylist.Id;
            SourceCore = RemoteCore.GetInstance(corePlaylist.SourceCore.InstanceId, RemotingMode.Host);

            _memberRemote = new MemberRemote(this, $"{corePlaylist.SourceCore.InstanceId}.{nameof(RemoteCorePlaylist)}.{corePlaylist.Id}", RemoteCoreMessageHandler.SingletonHost);
        }

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc/>  
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc/>  
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc/>  
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc/>  
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc/> 
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <inheritdoc/> 
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc/>
        public ICore SourceCore { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Name => _name;

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public DateTime? LastPlayed { get; set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; set; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; set; }

        /// <inheritdoc/>
        public ICoreUserProfile? Owner { get; set; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; set; }

        /// <inheritdoc/>
        public int TotalTrackCount { get; set; }

        /// <inheritdoc/>
        public bool IsPlayTrackCollectionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsPauseTrackCollectionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public DateTime? AddedAt { get; set; }

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public int TotalImageCount { get; set; }

        /// <inheritdoc/>
        public int TotalUrlCount { get; set; }

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync() => default;
    }
}
