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

        private AsyncLock _getTracksMutex = new AsyncLock();
        private AsyncLock _getArtistsMutex = new AsyncLock();
        private AsyncLock _getAlbumsMutex = new AsyncLock();
        private AsyncLock _getPlaylistsMutex = new AsyncLock();
        private AsyncLock _getChildrenMutex = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected ExternalCorePlayableCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        [RemoteMethod]
        private void OnPlaybackStateChanged(object sender, PlaybackState e)
        {

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

        /// <inheritdoc />?
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? TotalChildrenCountChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        [RemoteProperty]
        public ICore SourceCore { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Id { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public Uri? Url { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Name { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string? Description { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public PlaybackState PlaybackState { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public TimeSpan Duration { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? LastPlayed { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? AddedAt { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public virtual int TotalAlbumItemsCount
        {
            get => _totalAlbumItemsCount;
            internal set
            {
                _totalAlbumItemsCount = value;
                AlbumItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public virtual int TotalArtistItemsCount
        {
            get => _totalArtistItemsCount;
            internal set
            {
                _totalArtistItemsCount = value;
                ArtistItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public virtual int TotalTracksCount
        {
            get => _totalTracksCount;
            internal set
            {
                _totalTracksCount = value;
                TrackItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public virtual int TotalPlaylistItemsCount
        {
            get => _totalPlaylistItemsCount;
            internal set
            {
                _totalPlaylistItemsCount = value;
                PlaylistItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public virtual int TotalChildrenCount
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
            get => _totalChildrenCount;
            internal set
            {
                _totalChildrenCount = value;
                ImagesCountChanged?.Invoke(this, value);
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
        public bool IsPlayArtistCollectionAsyncAvailable  { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseArtistCollectionAsyncAvailable  { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayPlaylistCollectionAsyncAvailable  { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPausePlaylistCollectionAsyncAvailable  { get; set; }

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable  { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseTrackCollectionAsyncAvailable  { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeNameAsyncAvailable  { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeDescriptionAsyncAvailable  { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public bool IsChangeDurationAsyncAvailable  { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public virtual bool IsInitialized { get; set; }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddChildAvailable(int index)
        {
            return MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddChildAvailable));
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddPlaylistItemAvailable(int index)
        {
            return MemberRemote.ReceiveDataAsync<bool>(nameof(IsAddPlaylistItemAvailable));
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddAlbumItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemovePlaylistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveAlbumItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveChildAvailable(int index)
        {
            return MemberRemote.ReceiveDataAsync<bool>(nameof());
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDescriptionAsync(string? description)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeNameAsync(string name)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseAlbumCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseArtistCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlaylistCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlayableCollectionGroupAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            return Task.CompletedTask;
        }

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
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveAlbumItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemovePlaylistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveChildAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [RemoteMethod]
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            MemberRemote.ReceiveDataAsync<IEnumerable<ICoreImage>>(nameof(GetImagesAsync));
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddImageAsync(ICoreImage image, int index)
        {

        }

        /// <summary>
        /// Initializes the collection group base.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RemoteMethod]
        public virtual Task InitAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [RemoteMethod]
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
