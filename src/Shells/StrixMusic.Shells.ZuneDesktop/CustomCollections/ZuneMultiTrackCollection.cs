using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// Creates a new instance for <see cref="ITrackCollection"/>.
        /// </summary>
        /// <param name="collection"></param>
        public ZuneMultiTrackCollection()
        {
        }

        /// <inheritdoc />
        public int TotalTrackCount => throw new NotImplementedException();

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => throw new NotImplementedException();

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => throw new NotImplementedException();

        /// <inheritdoc />
        public DateTime? AddedAt => throw new NotImplementedException();

        /// <inheritdoc />
        public string Id => throw new NotImplementedException();

        /// <inheritdoc />
        public string Name => throw new NotImplementedException();

        /// <inheritdoc />
        public string? Description => throw new NotImplementedException();

        /// <inheritdoc />
        public DateTime? LastPlayed => throw new NotImplementedException();

        /// <inheritdoc />
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc />
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => throw new NotImplementedException();

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => throw new NotImplementedException();

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => throw new NotImplementedException();

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalImageCount => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<ICoreImageCollection> Sources => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalUrlCount => throw new NotImplementedException();

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => throw new NotImplementedException();

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => throw new NotImplementedException();

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
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

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
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
