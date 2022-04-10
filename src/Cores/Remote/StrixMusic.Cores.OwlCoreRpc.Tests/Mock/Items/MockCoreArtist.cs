using OwlCore.Events;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Items
{
    public class MockCoreArtist : ICoreArtist
    {
        public MockCoreArtist(ICore sourceCore, string id, string name)
        {
            SourceCore = sourceCore;
            Id = id;
            Name = name;
        }

        public ICore SourceCore { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public ICorePlayableCollectionGroup? RelatedItems { get; set; }

        public int TotalAlbumItemsCount { get; set; }

        public bool IsPlayAlbumCollectionAsyncAvailable { get; set; }

        public bool IsPauseAlbumCollectionAsyncAvailable { get; set; }

        public int TotalTrackCount { get; set; }

        public bool IsPlayTrackCollectionAsyncAvailable { get; set; }

        public bool IsPauseTrackCollectionAsyncAvailable { get; set; }

        public DateTime? AddedAt { get; set; }

        public string? Description { get; set; }

        public DateTime? LastPlayed { get; set; }

        public PlaybackState PlaybackState { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsChangeNameAsyncAvailable { get; set; }

        public bool IsChangeDescriptionAsyncAvailable { get; set; }

        public bool IsChangeDurationAsyncAvailable { get; set; }

        public int TotalImageCount { get; set; }

        public int TotalUrlCount { get; set; }

        public int TotalGenreCount { get; set; }

        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;
        public event EventHandler<int>? AlbumItemsCountChanged;
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;
        public event EventHandler<int>? TracksCountChanged;
        public event EventHandler<PlaybackState>? PlaybackStateChanged;
        public event EventHandler<string>? NameChanged;
        public event EventHandler<string?>? DescriptionChanged;
        public event EventHandler<TimeSpan>? DurationChanged;
        public event EventHandler<DateTime?>? LastPlayedChanged;
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
        public event EventHandler<int>? ImagesCountChanged;
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;
        public event EventHandler<int>? UrlsCountChanged;
        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;
        public event EventHandler<int>? GenresCountChanged;

        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddGenreAsync(ICoreGenre genre, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield return MockCoreItemFactory.CreateAlbum(SourceCore);
        }

        public IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return AsyncEnumerable.Empty<ICoreGenre>();
        }

        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield return MockCoreItemFactory.CreateTrack(SourceCore);
        }

        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
