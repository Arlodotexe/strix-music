using OwlCore.Events;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.Core.Items
{
    public class MockCoreTrack : ICoreTrack
    {
        public MockCoreTrack(ICore sourceCore, string id, string name)
        {
            Id = id;
            Name = name;
            SourceCore = sourceCore;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public ICoreAlbum? Album { get; set; }

        public ICoreLyrics? Lyrics { get; set; }

        public ICorePlayableCollectionGroup? RelatedItems { get; set; }

        public TrackType Type { get; set; }

        public int? TrackNumber { get; set; }

        public int? DiscNumber { get; set; }

        public CultureInfo? Language { get; set; }

        public bool IsExplicit { get; set; }

        public bool IsChangeAlbumAsyncAvailable { get; set; }

        public bool IsChangeTrackNumberAsyncAvailable { get; set; }

        public bool IsChangeLanguageAsyncAvailable { get; set; }

        public bool IsChangeLyricsAsyncAvailable { get; set; }

        public bool IsChangeIsExplicitAsyncAvailable { get; set; }

        public int TotalArtistItemsCount { get; set; }

        public bool IsPlayArtistCollectionAsyncAvailable { get; set; }

        public bool IsPauseArtistCollectionAsyncAvailable { get; set; }

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

        public ICore SourceCore { get; set; }

        public event EventHandler<ICoreAlbum?>? AlbumChanged;

        public event EventHandler<ICoreLyrics?>? LyricsChanged;

        public event EventHandler<int?>? TrackNumberChanged;

        public event EventHandler<CultureInfo?>? LanguageChanged;

        public event EventHandler<bool>? IsExplicitChanged;

        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        public event EventHandler<int>? ArtistItemsCountChanged;

        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        public event EventHandler<string>? NameChanged;

        public event EventHandler<string?>? DescriptionChanged;

        public event EventHandler<TimeSpan>? DurationChanged;

        public event EventHandler<DateTime?>? LastPlayedChanged;

        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        public event EventHandler<int>? ImagesCountChanged;

        public event EventHandler<int>? UrlsCountChanged;

        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

        public event EventHandler<int>? GenresCountChanged;

        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default)
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

        public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeAlbumAsync(ICoreAlbum? albums, CancellationToken cancellationToken = default)
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

        public Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLyricsAsync(ICoreLyrics? lyrics, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
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

        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
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

        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default)
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

        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
