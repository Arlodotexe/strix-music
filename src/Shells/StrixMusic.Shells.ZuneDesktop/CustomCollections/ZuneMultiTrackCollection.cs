using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.ZuneDesktop.CustomCollections
{
    /// <summary>
    /// Special collection that can host tracks from more than <see cref="ITrackCollectionViewModel"/>
    /// </summary>
    public class ZuneMultiTrackCollection : ITrackCollection
    {
        private readonly List<ITrack> _tracks = new();
        private int _totalTrackCount = 0;

        /// <summary>
        /// Creates a new instance for <see cref="ITrackCollection"/>.
        /// </summary>
        /// <param name="sources"></param>
        public ZuneMultiTrackCollection(IEnumerable<ICoreLibrary> sources)
        {
            StoredSources = sources.ToList();
            Guard.HasSizeGreaterThan(StoredSources, 0, nameof(StoredSources));

            PreferredSource = StoredSources[0];
        }

        /// <summary>
        /// The source items that were merged to create this <see cref="ZuneMultiTrackCollection"/>
        /// </summary>
        public List<ICoreLibrary> StoredSources { get; }

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected ICorePlayableCollectionGroup PreferredSource { get; }

        /// <inheritdoc />
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
        public bool IsPlayTrackCollectionAsyncAvailable => PreferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => PreferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public DateTime? AddedAt => PreferredSource.AddedAt;

        /// <inheritdoc />
        public string Id => PreferredSource.Id;

        /// <inheritdoc />
        public string Name => PreferredSource.Name;

        /// <inheritdoc />
        public string? Description => PreferredSource.Description;

        /// <inheritdoc />
        public DateTime? LastPlayed => PreferredSource.LastPlayed;

        /// <inheritdoc />
        public PlaybackState PlaybackState => PreferredSource.PlaybackState;

        /// <inheritdoc />
        public TimeSpan Duration => PreferredSource.Duration;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => PreferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => PreferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => PreferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => throw new NotSupportedException();

        /// <inheritdoc />
        public int TotalImageCount => PreferredSource.TotalImageCount;

        /// <inheritdoc />
        public IReadOnlyList<ICoreImageCollection> Sources => StoredSources;

        /// <inheritdoc />
        public int TotalUrlCount => PreferredSource.TotalUrlCount;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => StoredSources;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

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
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<DownloadInfo>? DownloadInfoChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event EventHandler? SourcesChanged;

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default)
        {
            _tracks.Insert(index, track);

            TotalTrackCount = _tracks.Count;
            TracksChanged?.Invoke(this, new List<CollectionChangedItem<ITrack>> { new(track, index) }, new List<CollectionChangedItem<ITrack>>());

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public ValueTask DisposeAsync() => throw new NotImplementedException();

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => throw new NotImplementedException();

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => throw new NotImplementedException();

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => throw new NotImplementedException();

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _tracks.ToAsyncEnumerable();

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => PreferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => PreferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => PreferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task<bool> RemoveTrackAsync(ITrack track)
        {
            TotalTrackCount = TotalTrackCount - 1;
            TracksCountChanged?.Invoke(this, TotalTrackCount);

            return Task.FromResult(_tracks.Remove(track));
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => PreferredSource.RemoveUrlAsync(index);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
