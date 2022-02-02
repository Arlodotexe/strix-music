using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Used to create a new playlist that can be added to the backend.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Must use instances to satisfy interface.")]
    public sealed class InitialPlaylistData : IPlaylist, IInitialData
    {
        private readonly IReadOnlyList<ICorePlaylist> _sources = new List<ICorePlaylist>();

        /// <summary>
        /// Holds any tracks that the user wants to add to the playlist on creation.
        /// </summary>
        public List<ITrack> Tracks { get; set; } = new List<ITrack>();

        /// <summary>
        /// Holds any images that the user wants to add to the playlist on creation. These should point to a file that the app has access to.
        /// </summary>
        public List<IImage> Images { get; set; } = new List<IImage>();

        /// <summary>
        /// Holds any urls that the user wants to add to the playlist on creation.
        /// </summary>
        public List<IUrl> Urls { get; set; } = new List<IUrl>();

        /// <inheritdoc />
        public List<ICore>? TargetSourceCores { get; set; } = new List<ICore>();

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

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

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc/>
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public int TotalImageCount => Images.Count;

        /// <inheritdoc />
        public int TotalTrackCount => Tracks.Count;

        /// <inheritdoc/>
        public int TotalUrlCount => Urls.Count;

        /// <inheritdoc />
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc />
        public Uri? Url { get; set; }

        /// <inheritdoc />
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc/>
        public DownloadInfo DownloadInfo => throw new NotSupportedException();

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public TimeSpan Duration { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public IUserProfile? Owner { get; set; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

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
        public Task<bool> IsRemoveTrackAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync()
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            return Task.FromResult(false);
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

        /// <inheritdoc/>
        public Task StartDownloadOperationAsync(DownloadOperation operation)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            Tracks.InsertOrAdd(index, track);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            Tracks.RemoveAt(index);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            Images.InsertOrAdd(index, image);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            Images.RemoveAt(index);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task AddUrlAsync(IUrl url, int index)
        {
            Urls.InsertOrAdd(index, url);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveUrlAsync(int index)
        {
            Urls.RemoveAt(index);

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; } = new List<ICore>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylist> IMerged<ICorePlaylist>.Sources => _sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreUrlCollection> Sources => _sources;

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return Task.FromResult<IReadOnlyList<IImage>>(Images.Skip(offset).Take(limit).ToList());
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset)
        {
            return Task.FromResult<IReadOnlyList<IUrl>>(Urls.Skip(offset).Take(limit).ToList());
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset)
        {
            return Task.FromResult<IReadOnlyList<ITrack>>(Tracks.Skip(offset).Take(limit).ToList());
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => false;

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => false;

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => false;

        /// <inheritdoc/>
        public bool Equals(ICoreUrlCollection other) => false;

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => false;

        /// <inheritdoc />
        public bool Equals(ICorePlaylist other) => false;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}