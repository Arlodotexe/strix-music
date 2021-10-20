﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;
using OwlCore.Events;
using OwlCore.Remoting;
using OwlCore.Remoting.Attributes;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICorePlayableCollectionGroup"/>
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public abstract class RemoteCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup
    {
        private readonly MemberRemote _memberRemote;

        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;
        private int _totalImageCount;
        private int _totalUrlCount;

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
        private AsyncLock _getUrlsMutex = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the instance of the core this object was created in.</param>
        /// <param name="name">
        /// The name of the implementing collection. 
        /// Predefined for some implementations (<see cref="RemoteCoreSearchResults"/>, <see cref="RemoteCoreLibrary"/>, etc.) so data is properly merged in the UI.
        /// </param>
        protected RemoteCorePlayableCollectionGroupBase(string sourceCoreInstanceId, string name)
        {
            // Properties assigned before MemberRemote is created won't be set remotely, and should be set remotely in the ctor.
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId);
            _name = name;

            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{GetType().Name}.{name}");

            Id = _memberRemote.Id;
        }

        [RemoteMethod]
        private void OnTrackItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
        {
            TracksChanged?.Invoke(this, addedItems, removedItems);
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

        [RemoteMethod]
        private void OnUrlsChanged(IReadOnlyList<CollectionChangedItem<ICoreUrl>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
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
            using (await _getChildrenMutex.LockAsync())
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
            using (await _getPlaylistsMutex.LockAsync())
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
            using (await _getAlbumsMutex.LockAsync())
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
            using (await _getArtistsMutex.LockAsync())
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
            using (await _getTracksMutex.LockAsync())
            {
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
            using (await _getImagesMutex.LockAsync())
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
            using (await _getUrlsMutex.LockAsync())
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
        public async ValueTask DisposeAsync()
        {
            await _memberRemote.RemoteWaitAsync(nameof(DisposeAsync));
            _memberRemote.Dispose();
        }
    }
}