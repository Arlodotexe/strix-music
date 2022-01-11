using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Playlist data that was created in the UI and should be added as a new item in the backend.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Must use instances to satisfy interface.")]
    public class InitialCorePlaylistData : ICorePlaylist, ICoreInitialData
    {
        private readonly InitialPlaylistData _playlistData;

        /// <summary>
        /// Create a new instance of <see cref="InitialCorePlaylistData"/>.
        /// </summary>
        /// <param name="playlistData"></param>
        /// <param name="sourceCore"></param>
        public InitialCorePlaylistData(InitialPlaylistData playlistData, ICore sourceCore)
        {
            _playlistData = playlistData;
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public int TotalTrackCount => _playlistData.TotalTrackCount;

        /// <inheritdoc />
        public int TotalImageCount => _playlistData.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _playlistData.TotalUrlCount;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _playlistData.Id;

        /// <inheritdoc />
        public string Name => _playlistData.Name;

        /// <inheritdoc />
        public string? Description => _playlistData.Description;

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public TimeSpan Duration { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; } = DateTime.Now;

        /// <inheritdoc />
        public ICoreUserProfile? Owner { get; }

        /// <inheritdoc />
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable { get; }

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddUrlAsync(ICoreUrl url, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
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
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ICoreTrack track)
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