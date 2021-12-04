using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// Wraps around a <see cref="ICorePlayableCollectionGroup"/> to host data, or receives data given some identifiers.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public abstract partial class RemoteCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup
    {
        private readonly MemberRemote _memberRemote;
        private readonly ICorePlayableCollectionGroup? _corePlayableCollection;
        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;
        private int _totalImageCount;
        private int _totalUrlCount;
        private string? _description;
        private PlaybackState _playbackState;
        private TimeSpan _duration;
        private DateTime? _lastPlayed;
        private SemaphoreSlim _getTracksMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getArtistsMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getAlbumsMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getPlaylistsMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getChildrenMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getImagesMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getUrlsMutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroupBase"/>, for receiving data.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the instance of the core this object was created in.</param>
        /// <param name="remotingId">A unique identifier for the wrapped collection that is consistent between host and clients.</param>
        protected RemoteCorePlayableCollectionGroupBase(string sourceCoreInstanceId, string remotingId)
        {
            // Properties assigned before MemberRemote is created won't be set remotely, and should be set remotely in the ctor.
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId);

            Id = remotingId;
            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{GetType().Name}.{remotingId}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroupBase"/> and wraps around a <paramref name="corePlayableCollection"/> for sending data.
        /// </summary>
        /// <param name="corePlayableCollection">The collection to wrap around and remotely interact with.</param>
        /// <param name="remotingId">A unique identifier for this playable collection instance. Used to </param>
        /// 
        protected RemoteCorePlayableCollectionGroupBase(ICorePlayableCollectionGroup corePlayableCollection)
        {
            _corePlayableCollection = corePlayableCollection;

            SourceCore = RemoteCore.GetInstance(corePlayableCollection.SourceCore.InstanceId);

            var fullRemoteId = $"{corePlayableCollection.SourceCore.InstanceId}.{GetType().Name}.{corePlayableCollection.Id}";
            _memberRemote = new MemberRemote(this, fullRemoteId, RemoteCoreMessageHandler.SingletonHost);

            // Remotely update with the actual ID.
            Id = _corePlayableCollection.Id;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ChildrenCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <inheritdoc />
        public ICore SourceCore { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Id { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc />
        [RemoteProperty]
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            set
            {
                _playbackState = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? LastPlayed
        {
            get => _lastPlayed;
            set
            {
                _lastPlayed = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? AddedAt { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalAlbumItemsCount
        {
            get => _totalAlbumItemsCount;
            set
            {
                _totalAlbumItemsCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalArtistItemsCount
        {
            get => _totalArtistItemsCount;
            set
            {
                _totalArtistItemsCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalTrackCount
        {
            get => _totalTrackCount;
            set
            {
                _totalTrackCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalPlaylistItemsCount
        {
            get => _totalPlaylistItemsCount;
            set
            {
                _totalPlaylistItemsCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalChildrenCount
        {
            get => _totalChildrenCount;
            internal set
            {
                _totalChildrenCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalImageCount
        {
            get => _totalImageCount;
            internal set
            {
                _totalImageCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalUrlCount
        {
            get => _totalUrlCount;
            internal set
            {
                _totalUrlCount = value;
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayAlbumCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseAlbumCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayArtistCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseArtistCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayPlaylistCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPausePlaylistCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseTrackCollectionAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeNameAsyncAvailable { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeDescriptionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public bool IsChangeDurationAsyncAvailable { get; set; }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddChildAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddChildAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddPlaylistItemAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddTrackAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddTrackAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddArtistItemAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddAlbumItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddAlbumItemAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddImageAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddImageAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddUrlAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddUrlAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveTrackAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveImageAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveUrlAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemovePlaylistItemAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveAlbumItemAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveArtistItemAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveChildAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveChildAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDescriptionAsync(string? description) => _memberRemote.ReceiveDataAsync<object>(nameof(ChangeDescriptionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDurationAsync(TimeSpan duration) => _memberRemote.RemoteWaitAsync(nameof(ChangeDurationAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeNameAsync(string name) => _memberRemote.RemoteWaitAsync(nameof(ChangeNameAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseAlbumCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseAlbumCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PlayAlbumCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseArtistCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PlayArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlaylistCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PausePlaylistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PlayPlaylistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseTrackCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseTrackCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseTrackCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem) => _memberRemote.RemoteWaitAsync(nameof(PlayAlbumCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync() => _memberRemote.RemoteWaitAsync(nameof(PlayPlayableCollectionGroupAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlayableCollectionGroupAsync() => _memberRemote.RemoteWaitAsync(nameof(PausePlayableCollectionGroupAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem) => _memberRemote.RemoteWaitAsync(nameof(PlayPlaylistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem) => _memberRemote.RemoteWaitAsync(nameof(PlayArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup) => _memberRemote.RemoteWaitAsync(nameof(PlayPlayableCollectionGroupAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync(ICoreTrack track) => _memberRemote.RemoteWaitAsync(nameof(PlayTrackCollectionAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            using (await Flow.EasySemaphore(_getChildrenMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICorePlayableCollectionGroup>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getPlaylistsMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICorePlaylistCollectionItem>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getAlbumsMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreAlbumCollectionItem>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getArtistsMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreArtistCollectionItem>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            using (await Flow.EasySemaphore(_getTracksMutex))
            {
                if (_memberRemote.Mode == RemotingMode.Host)
                {
                    Guard.IsNotNull(_corePlayableCollection, nameof(_corePlayableCollection));

                    await foreach (var item in _corePlayableCollection.GetTracksAsync(limit, offset))
                    {
                        Guard.IsNotNull(item, nameof(item));

                        yield return await _memberRemote.PublishDataAsync(nameof(GetTracksAsync), item);
                    }
                }

                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreTrack>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getImagesMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreImage>>(nameof(GetImagesAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getUrlsMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreUrl>>(nameof(GetUrlsAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddTrackAsync(ICoreTrack track, int index) => _memberRemote.RemoteWaitAsync(nameof(AddTrackAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index) => _memberRemote.RemoteWaitAsync(nameof(AddArtistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index) => _memberRemote.RemoteWaitAsync(nameof(AddAlbumItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index) => _memberRemote.RemoteWaitAsync(nameof(AddPlaylistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index) => _memberRemote.RemoteWaitAsync(nameof(AddChildAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddImageAsync(ICoreImage image, int index) => _memberRemote.RemoteWaitAsync(nameof(AddImageAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddUrlAsync(ICoreUrl url, int index) => _memberRemote.RemoteWaitAsync(nameof(AddUrlAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveTrackAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveTrackAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveArtistItemAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveArtistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveAlbumItemAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveAlbumItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemovePlaylistItemAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemovePlaylistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveChildAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveChildAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveImageAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveImageAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveUrlAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveUrlAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public ValueTask DisposeAsync() => new ValueTask(Task.Run(async () =>
        {
            await _memberRemote.RemoteWaitAsync(nameof(DisposeAsync));
            _memberRemote.Dispose();
        }));
    }
}
