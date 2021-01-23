using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// Playlist data that was created in the UI and should be added as a new item in the backend.
    /// </summary>
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
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public int TotalTracksCount => _playlistData.TotalTracksCount;

        /// <inheritdoc />
        public int TotalImageCount => _playlistData.TotalImageCount;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _playlistData.Genres;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _playlistData.Id;

        /// <inheritdoc />
        public Uri? Url { get; }

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
        public bool IsPlayAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable { get; }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable { get; }

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
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
        public Task PlayAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseAsync()
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
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            throw new NotSupportedException();
        }
    }
}