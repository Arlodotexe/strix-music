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

        private string _name = string.Empty;
        private string? _description;

        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;
        private int _totalImageCount;
        private int _totalUrlCount;

        private bool _isChangeNameAsyncAvailable;
        private bool _isChangeDescriptionAsyncAvailable;
        private bool _isChangeDurationAsyncAvailable;
        private bool _isPlayAlbumCollectionAsyncAvailable;
        private bool _isPauseAlbumCollectionAsyncAvailable;
        private bool _isPlayArtistCollectionAsyncAvailable;
        private bool _isPauseArtistCollectionAsyncAvailable;
        private bool _isPlayPlaylistCollectionAsyncAvailable;
        private bool _isPausePlaylistCollectionAsyncAvailable;
        private bool _isPlayTrackCollectionAsyncAvailable;
        private bool _isPauseTrackCollectionAsyncAvailable;

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
        protected RemoteCorePlayableCollectionGroupBase(string sourceCoreInstanceId, string id)
        {
            // These properties are set remotely in the other ctor
            SourceCoreInstanceId = sourceCoreInstanceId;
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client);
            Id = id;

            _memberRemote = new MemberRemote(this, $"{SourceCoreInstanceId}.{GetType().Name}.{Id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroupBase"/> and wraps around a <paramref name="corePlayableCollection"/> for sending data.
        /// </summary>
        /// <param name="corePlayableCollection">The collection to wrap around and remotely interact with.</param>
        protected RemoteCorePlayableCollectionGroupBase(ICorePlayableCollectionGroup corePlayableCollection)
        {
            _corePlayableCollection = corePlayableCollection;

            SourceCoreInstanceId = corePlayableCollection.SourceCore.InstanceId;
            SourceCore = RemoteCore.GetInstance(SourceCoreInstanceId, RemotingMode.Host);
            Id = corePlayableCollection.Id;

            var fullRemoteId = $"{SourceCoreInstanceId}.{GetType().Name}.{Id}";
            _memberRemote = new MemberRemote(this, fullRemoteId, RemoteCoreMessageHandler.SingletonHost);

            Name = corePlayableCollection.Name;
            Description = corePlayableCollection.Description;
            PlaybackState = corePlayableCollection.PlaybackState;
            Duration = corePlayableCollection.Duration;
            LastPlayed = corePlayableCollection.LastPlayed;
            AddedAt = corePlayableCollection.AddedAt;

            TotalTrackCount = corePlayableCollection.TotalTrackCount;
            TotalArtistItemsCount = corePlayableCollection.TotalArtistItemsCount;
            TotalAlbumItemsCount = corePlayableCollection.TotalAlbumItemsCount;
            TotalPlaylistItemsCount = corePlayableCollection.TotalPlaylistItemsCount;
            TotalChildrenCount = corePlayableCollection.TotalChildrenCount;
            TotalImageCount = corePlayableCollection.TotalImageCount;
            TotalUrlCount = corePlayableCollection.TotalUrlCount;

            IsChangeNameAsyncAvailable = corePlayableCollection.IsChangeNameAsyncAvailable;
            IsChangeDescriptionAsyncAvailable = corePlayableCollection.IsChangeDescriptionAsyncAvailable;
            IsChangeDurationAsyncAvailable = corePlayableCollection.IsChangeDurationAsyncAvailable;

            IsPlayAlbumCollectionAsyncAvailable = corePlayableCollection.IsPlayAlbumCollectionAsyncAvailable;
            IsPauseAlbumCollectionAsyncAvailable = corePlayableCollection.IsPauseAlbumCollectionAsyncAvailable;
            IsPlayArtistCollectionAsyncAvailable = corePlayableCollection.IsPlayArtistCollectionAsyncAvailable;
            IsPauseArtistCollectionAsyncAvailable = corePlayableCollection.IsPauseArtistCollectionAsyncAvailable;
            IsPlayPlaylistCollectionAsyncAvailable = corePlayableCollection.IsPlayPlaylistCollectionAsyncAvailable;
            IsPausePlaylistCollectionAsyncAvailable = corePlayableCollection.IsPausePlaylistCollectionAsyncAvailable;
            IsPlayTrackCollectionAsyncAvailable = corePlayableCollection.IsPlayTrackCollectionAsyncAvailable;
            IsPauseTrackCollectionAsyncAvailable = corePlayableCollection.IsPauseTrackCollectionAsyncAvailable;

            AttachEvents(corePlayableCollection);
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

        /// <summary>
        /// The instance ID of the <see cref="SourceCore" />
        /// </summary>
        public string SourceCoreInstanceId { get; set; }

        /// <inheritdoc />
        public ICore SourceCore { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                DescriptionChanged?.Invoke(this, value);
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
                PlaybackStateChanged?.Invoke(this, value);
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
                DurationChanged?.Invoke(this, value);
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
                LastPlayedChanged?.Invoke(this, value);
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
                AlbumItemsCountChanged?.Invoke(this, value);
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
                ArtistItemsCountChanged?.Invoke(this, value);
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
                TracksCountChanged?.Invoke(this, value);
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
                PlaylistItemsCountChanged?.Invoke(this, value);
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
                ChildrenCountChanged?.Invoke(this, value);
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
                ImagesCountChanged?.Invoke(this, value);
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
                UrlsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayAlbumCollectionAsyncAvailable
        {
            get => _isPlayAlbumCollectionAsyncAvailable;
            set
            {
                _isPlayAlbumCollectionAsyncAvailable = value;
                IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseAlbumCollectionAsyncAvailable
        {
            get => _isPauseAlbumCollectionAsyncAvailable;
            set
            {
                _isPauseAlbumCollectionAsyncAvailable = value;
                IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayArtistCollectionAsyncAvailable
        {
            get => _isPlayArtistCollectionAsyncAvailable;
            set
            {
                _isPlayArtistCollectionAsyncAvailable = value;
                IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseArtistCollectionAsyncAvailable
        {
            get => _isPauseArtistCollectionAsyncAvailable;
            set
            {
                _isPauseArtistCollectionAsyncAvailable = value;
                IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayPlaylistCollectionAsyncAvailable
        {
            get => _isPlayPlaylistCollectionAsyncAvailable;
            set
            {
                _isPlayPlaylistCollectionAsyncAvailable = value;
                IsPlayPlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPausePlaylistCollectionAsyncAvailable
        {
            get => _isPausePlaylistCollectionAsyncAvailable;
            set
            {
                _isPausePlaylistCollectionAsyncAvailable = value;
                IsPausePlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayTrackCollectionAsyncAvailable
        {
            get => _isPlayTrackCollectionAsyncAvailable;
            set
            {
                _isPlayTrackCollectionAsyncAvailable = value;
                IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseTrackCollectionAsyncAvailable
        {
            get => _isPauseTrackCollectionAsyncAvailable;
            set
            {
                _isPauseTrackCollectionAsyncAvailable = value;
                IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeNameAsyncAvailable
        {
            get => _isChangeNameAsyncAvailable;
            set
            {
                _isChangeNameAsyncAvailable = value;
                IsChangeNameAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeDescriptionAsyncAvailable
        {
            get => _isChangeDescriptionAsyncAvailable;
            set
            {
                _isChangeDescriptionAsyncAvailable = value;
                IsChangeDescriptionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc/>
        [RemoteProperty]
        public bool IsChangeDurationAsyncAvailable
        {
            get => _isChangeDurationAsyncAvailable;
            set
            {
                _isChangeDurationAsyncAvailable = value;
                IsChangeDurationAsyncAvailableChanged?.Invoke(this, value);
            }
        }

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
        public Task ChangeDescriptionAsync(string? description) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(ChangeDescriptionAsync)}.{description}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_corePlayableCollection, nameof(_corePlayableCollection));

                await _corePlayableCollection.ChangeDescriptionAsync(description);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDurationAsync(TimeSpan duration) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(ChangeDurationAsync)}.{duration}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_corePlayableCollection, nameof(_corePlayableCollection));

                await _corePlayableCollection.ChangeDurationAsync(duration);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeNameAsync(string name) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(ChangeNameAsync)}.{name}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_corePlayableCollection, nameof(_corePlayableCollection));

                await _corePlayableCollection.ChangeNameAsync(name);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

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

                if (_memberRemote.Mode == RemotingMode.Client)
                {
                    var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreTrack>>(nameof(GetTracksAsync));

                    if (res is null)
                        yield break;

                    foreach (var item in res)
                        yield return item;
                }
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
