using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public abstract class MusicBrainzCollectionGroupBase : ICorePlayableCollectionGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected MusicBrainzCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

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
        public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public abstract string Id { get; protected set; }

        /// <inheritdoc />
        public abstract Uri? Url { get; protected set; }

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
        public abstract int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalTracksCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; } = 0;

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc/>
        public Task<bool> IsAddChildSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddPlaylistItemSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumItemSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveChildSupported(int index)
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
        public Task PauseAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayAsync()
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
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield break;
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }
    }
}
