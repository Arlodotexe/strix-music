using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Provisos;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Files.Models
{
    /// <inheritdoc cref="ICorePlayableCollectionGroup"/>
    public abstract class FilesCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup, IAsyncInit
    {
        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected FilesCorePlayableCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <summary>
        /// Initializes the collection group base.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task InitAsync(CancellationToken cancellationToken = default)
        {
            IsInitialized = true;
            return Task.CompletedTask;
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

        /// <inheritdoc />?
        public virtual event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />?
        public virtual event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />?
        public virtual event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />?
        public virtual event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />?
        public virtual event EventHandler<int>? ChildrenCountChanged;

        /// <inheritdoc />
        public virtual event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public virtual event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <inheritdoc />
        public ICore SourceCore { get; private set; }

        /// <inheritdoc />
        public abstract string Id { get; protected set; }

        /// <inheritdoc />
        public abstract string Name { get; protected set; }

        /// <inheritdoc />
        public abstract string? Description { get; protected set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc />
        public TimeSpan Duration => TimeSpan.Zero;

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
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
        public virtual int TotalTrackCount
        {
            get => _totalTrackCount;
            internal set
            {
                _totalTrackCount = value;
                TracksCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
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
        public virtual int TotalChildrenCount
        {
            get => _totalChildrenCount;
            internal set
            {
                _totalChildrenCount = value;
                ChildrenCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        public int TotalImageCount { get; } = 0;

        /// <inheritdoc />
        public int TotalUrlCount { get; } = 0;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPlayPlaylistCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPausePlaylistCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => true;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => true;

        /// <inheritdoc/>
        public virtual bool IsInitialized { get; protected set; }

        /// <inheritdoc/>
        public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
