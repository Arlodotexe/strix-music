using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;
using OwlCore.Events;
using OwlCore.Provisos;
using OwlCore.Remoting;
using OwlCore.Remoting.Attributes;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.External.Models
{
    /// <inheritdoc cref="ICorePlayableCollectionGroup"/>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public abstract class ExternalCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup, IAsyncInit
    {
        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTracksCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;
        private int _totalImageCount;

        private Uri? _url;
        private string _name;
        private string? _description;
        private PlaybackState _playbackState;
        private TimeSpan _duration;
        private DateTime? _lastPlayed;

        private bool _isChangeDurationAsyncAvailable;
        private bool _isChangeDescriptionAsyncAvailable;
        private bool _isChangeNameAsyncAvailable;
        private bool _isPauseTrackCollectionAsyncAvailable;
        private bool _isPlayTrackCollectionAsyncAvailable;
        private bool _isPausePlaylistCollectionAsyncAvailable;
        private bool _isPlayPlaylistCollectionAsyncAvailable;
        private bool _isPauseArtistCollectionAsyncAvailable;
        private bool _isPlayArtistCollectionAsyncAvailable;
        private bool _isPauseAlbumCollectionAsyncAvailable;
        private bool _isPlayAlbumCollectionAsyncAvailable;

        private AsyncLock _getTracksMutex = new AsyncLock();
        private AsyncLock _getArtistsMutex = new AsyncLock();
        private AsyncLock _getAlbumsMutex = new AsyncLock();
        private AsyncLock _getPlaylistsMutex = new AsyncLock();
        private AsyncLock _getChildrenMutex = new AsyncLock();
        private AsyncLock _getImagesMutex = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        /// <param name="name">
        /// The name of the implementing collection. 
        /// Predefined for some implementations (<see cref="ExternalCoreSearchResults"/>, <see cref="ExternalCoreLibrary"/>, etc.) so data is properly merged in the UI.
        /// </param>
        protected ExternalCorePlayableCollectionGroupBase(ICore sourceCore, string name)
        {
            // Properties assigned before MemberRemote is created won't be set remotely.
            SourceCore = sourceCore; // should be set remotely by the ctor.
            _name = name;

            MemberRemote = new MemberRemote(this, $"{sourceCore.InstanceId}.{GetType().Name}.{name}");

            Id = MemberRemote.Id;
        }

        [RemoteMethod]
        private void OnTrackItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnPlaylistItemsChanged(IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> removedItems)
        {
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnAlbumItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnArtistItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnChildItemsChanged(IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> removedItems)
        {
            ChildItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnImagesChanged(IReadOnlyList<CollectionChangedItem<ICoreImage>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <summary>
        /// The <see cref="MemberRemote"/> used to communicate changes for the derived class.
        /// </summary>
        public MemberRemote MemberRemote { get; set; }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

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
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TotalChildrenCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

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
        public ICore SourceCore { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Id { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public Uri? Url
        {
            get => _url;
            set
            {
                _url = value;
                UrlChanged?.Invoke(this, value);
            }
        }

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
        public int TotalTracksCount
        {
            get => _totalTracksCount;
            set
            {
                _totalTracksCount = value;
                TrackItemsCountChanged?.Invoke(this, value);
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
                TotalChildrenCountChanged?.Invoke(this, value);
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
        [RemoteProperty]
        public bool IsInitialized { get; set; }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddChildAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddChildAvailable));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddPlaylistItemAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddPlaylistItemAvailable));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddTrackAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddTrackAvailable));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddArtistItemAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddArtistItemAvailable));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddAlbumItemAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddAlbumItemAvailable));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddImageAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddImageAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveTrackAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveTrackAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveImageAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveImageAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemovePlaylistItemAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsRemovePlaylistItemAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveAlbumItemAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveArtistItemAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveArtistItemAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveChildAvailable(int index) => MemberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveChildAvailable));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDescriptionAsync(string? description) => MemberRemote.ReceiveDataAsync<object>(nameof(ChangeDescriptionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDurationAsync(TimeSpan duration) => MemberRemote.RemoteWaitAsync(nameof(ChangeDurationAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeNameAsync(string name) => MemberRemote.RemoteWaitAsync(nameof(ChangeNameAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseAlbumCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PauseAlbumCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PlayAlbumCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseArtistCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PauseArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PlayArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlaylistCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PausePlaylistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PlayPlaylistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseTrackCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PauseTrackCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync() => MemberRemote.RemoteWaitAsync(nameof(PauseTrackCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem) => MemberRemote.RemoteWaitAsync(nameof(PlayAlbumCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync() => MemberRemote.RemoteWaitAsync(nameof(PlayPlayableCollectionGroupAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlayableCollectionGroupAsync() => MemberRemote.RemoteWaitAsync(nameof(PausePlayableCollectionGroupAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem) => MemberRemote.RemoteWaitAsync(nameof(PlayPlaylistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem) => MemberRemote.RemoteWaitAsync(nameof(PlayArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup) => MemberRemote.RemoteWaitAsync(nameof(PlayPlayableCollectionGroupAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync(ICoreTrack track) => MemberRemote.RemoteWaitAsync(nameof(PlayTrackCollectionAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            using (await _getChildrenMutex.LockAsync())
            {
                var res = await MemberRemote.ReceiveDataAsync<IEnumerable<ICorePlayableCollectionGroup>>(nameof(GetTracksAsync));

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
            using (await _getPlaylistsMutex.LockAsync())
            {
                var res = await MemberRemote.ReceiveDataAsync<IEnumerable<ICorePlaylistCollectionItem>>(nameof(GetTracksAsync));

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
            using (await _getAlbumsMutex.LockAsync())
            {
                var res = await MemberRemote.ReceiveDataAsync<IEnumerable<ICoreAlbumCollectionItem>>(nameof(GetTracksAsync));

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
            using (await _getArtistsMutex.LockAsync())
            {
                var res = await MemberRemote.ReceiveDataAsync<IEnumerable<ICoreArtistCollectionItem>>(nameof(GetTracksAsync));

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
            using (await _getTracksMutex.LockAsync())
            {
                var res = await MemberRemote.ReceiveDataAsync<IEnumerable<ICoreTrack>>(nameof(GetTracksAsync));

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
            using (await _getImagesMutex.LockAsync())
            {
                var res = await MemberRemote.ReceiveDataAsync<IEnumerable<ICoreImage>>(nameof(GetImagesAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddTrackAsync(ICoreTrack track, int index) => MemberRemote.RemoteWaitAsync(nameof(AddTrackAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index) => MemberRemote.RemoteWaitAsync(nameof(AddArtistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index) => MemberRemote.RemoteWaitAsync(nameof(AddAlbumItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index) => MemberRemote.RemoteWaitAsync(nameof(AddPlaylistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index) => MemberRemote.RemoteWaitAsync(nameof(AddChildAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveTrackAsync(int index) => MemberRemote.RemoteWaitAsync(nameof(RemoveTrackAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveArtistItemAsync(int index) => MemberRemote.RemoteWaitAsync(nameof(RemoveArtistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveAlbumItemAsync(int index) => MemberRemote.RemoteWaitAsync(nameof(RemoveAlbumItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemovePlaylistItemAsync(int index) => MemberRemote.RemoteWaitAsync(nameof(RemovePlaylistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveChildAsync(int index) => MemberRemote.RemoteWaitAsync(nameof(RemoveChildAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveImageAsync(int index) => MemberRemote.RemoteWaitAsync(nameof(RemoveImageAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddImageAsync(ICoreImage image, int index) => MemberRemote.RemoteWaitAsync(nameof(AddImageAsync));

        /// <summary>
        /// Initializes the collection group base.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RemoteMethod]
        public Task InitAsync() => MemberRemote.RemoteWaitAsync(nameof(InitAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public async ValueTask DisposeAsync()
        {
            await MemberRemote.RemoteWaitAsync(nameof(DisposeAsync));
            MemberRemote.Dispose();
        }
    }
}
