using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Cores.LocalFiles.Models
{
    /// <inheritdoc cref="ICorePlayableCollectionGroup"/>
    public abstract class LocalFilesCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup, IAsyncInit
    {
        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected LocalFilesCorePlayableCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <summary>
        /// Initializes the collection group base.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task InitAsync()
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
        public Task<bool> IsAddChildAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddUrlAsync(ICoreUrl url, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveChildAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
