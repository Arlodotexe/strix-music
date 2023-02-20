using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using SoundCloud.Api.Entities;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Cores.SoundCloud.Models;

public sealed class SoundCloudTrack : ICoreTrack
{
    private Track _track;

    public SoundCloudTrack(ICore sourceCore, Track track)
    {
        SourceCore = sourceCore;
        _track = track;
    }

    public ICoreAlbum? Album => null;

    public ICoreLyrics? Lyrics => null;

    public ICorePlayableCollectionGroup? RelatedItems => throw new NotImplementedException();

    public TrackType Type { get; } = TrackType.Song;

    public int? TrackNumber => null;

    public int? DiscNumber => null;

    public CultureInfo? Language => null;

    public bool IsExplicit => false;

    public bool IsChangeAlbumAsyncAvailable => false;

    public bool IsChangeTrackNumberAsyncAvailable => false;

    public bool IsChangeLanguageAsyncAvailable => false;

    public bool IsChangeLyricsAsyncAvailable => false;

    public bool IsChangeIsExplicitAsyncAvailable => false;

    public int TotalArtistItemsCount => throw new NotImplementedException();

    public bool IsPlayArtistCollectionAsyncAvailable => false;

    public bool IsPauseArtistCollectionAsyncAvailable => false;

    public DateTime? AddedAt { get; }

    public string Id => _track.Id.ToString();

    public string Name => _track.Title;

    public string? Description => _track.Description;

    public DateTime? LastPlayed { get; }

    public PlaybackState PlaybackState { get; }

    public TimeSpan Duration { get; }

    public bool IsChangeNameAsyncAvailable => false;

    public bool IsChangeDescriptionAsyncAvailable => false;

    public bool IsChangeDurationAsyncAvailable => false;

    public int TotalImageCount => 1;

    public int TotalUrlCount => 4;

    public int TotalGenreCount => _track.TagList.Count;

    public ICore SourceCore { get; }

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

    public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var tag in _track.TagList)
            if (tag is not null)
                yield return new SoundCloudGenre(SourceCore, tag);
    }

    public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        yield return new SoundCloudImage(SourceCore, _track.ArtworkUrl);
    }

    public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        List<SoundCloudUrl> urls = new(TotalUrlCount)
        {
            new(SourceCore, "Permalink", _track.PermalinkUrl, UrlType.MusicStreaming),
            new(SourceCore, "Purchase", _track.PurchaseUrl, UrlType.PurchaseForDownload),
            new(SourceCore, "Download", _track.DownloadUrl, UrlType.MusicStreaming),
            new(SourceCore, "Video", new(_track.VideoUrl), UrlType.Unspecified)
        };

        for (int i = offset; i < urls.Count && i - offset < limit; i++)
            yield return urls[i];
    }

    public IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task AddGenreAsync(ICoreGenre genre, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeAlbumAsync(ICoreAlbum? albums, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeLyricsAsync(ICoreLyrics? lyrics, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
