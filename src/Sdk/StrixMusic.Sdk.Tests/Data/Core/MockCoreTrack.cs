using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Data.Core
{
    internal class MockCoreTrack : ICoreTrack
    {
        public ICoreAlbum Album => throw new NotImplementedException();

        public ICoreLyrics Lyrics => throw new NotImplementedException();

        public ICorePlayableCollectionGroup RelatedItems => throw new NotImplementedException();

        public TrackType Type => throw new NotImplementedException();

        public int? TrackNumber => throw new NotImplementedException();

        public int? DiscNumber => throw new NotImplementedException();

        public CultureInfo Language => throw new NotImplementedException();

        public bool IsExplicit => throw new NotImplementedException();

        public bool IsChangeAlbumAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeTrackNumberAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeLanguageAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeLyricsAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeIsExplicitAsyncAvailable => throw new NotImplementedException();

        public int TotalArtistItemsCount => throw new NotImplementedException();

        public bool IsPlayArtistCollectionAsyncAvailable => throw new NotImplementedException();

        public bool IsPauseArtistCollectionAsyncAvailable => throw new NotImplementedException();

        public DateTime? AddedAt => throw new NotImplementedException();

        public string Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public DateTime? LastPlayed => throw new NotImplementedException();

        public PlaybackState PlaybackState => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public bool IsChangeNameAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeDescriptionAsyncAvailable => throw new NotImplementedException();

        public bool IsChangeDurationAsyncAvailable => throw new NotImplementedException();

        public int TotalImageCount => throw new NotImplementedException();

        public int TotalUrlCount => throw new NotImplementedException();

        public int TotalGenreCount => throw new NotImplementedException();

        public ICore SourceCore => throw new NotImplementedException();

        public event EventHandler<ICoreAlbum> AlbumChanged;
        public event EventHandler<ICoreLyrics> LyricsChanged;
        public event EventHandler<int?> TrackNumberChanged;
        public event EventHandler<CultureInfo> LanguageChanged;
        public event EventHandler<bool> IsExplicitChanged;
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem> ArtistItemsChanged;
        public event EventHandler<bool> IsPlayArtistCollectionAsyncAvailableChanged;
        public event EventHandler<bool> IsPauseArtistCollectionAsyncAvailableChanged;
        public event EventHandler<int> ArtistItemsCountChanged;
        public event CollectionChangedEventHandler<ICoreImage> ImagesChanged;
        public event CollectionChangedEventHandler<ICoreUrl> UrlsChanged;
        public event EventHandler<PlaybackState> PlaybackStateChanged;
        public event EventHandler<string> NameChanged;
        public event EventHandler<string> DescriptionChanged;
        public event EventHandler<TimeSpan> DurationChanged;
        public event EventHandler<DateTime?> LastPlayedChanged;
        public event EventHandler<bool> IsChangeNameAsyncAvailableChanged;
        public event EventHandler<bool> IsChangeDescriptionAsyncAvailableChanged;
        public event EventHandler<bool> IsChangeDurationAsyncAvailableChanged;
        public event EventHandler<int> ImagesCountChanged;
        public event EventHandler<int> UrlsCountChanged;
        public event CollectionChangedEventHandler<ICoreGenre> GenresChanged;
        public event EventHandler<int> GenresCountChanged;

        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddGenreAsync(ICoreGenre genre, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddUrlAsync(ICoreUrl url, int index)
        {
            throw new NotImplementedException();
        }

        public Task ChangeAlbumAsync(ICoreAlbum albums)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDescriptionAsync(string description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLyricsAsync(ICoreLyrics lyrics)
        {
            throw new NotImplementedException();
        }

        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task PauseArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveGenreAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUrlAsync(int index)
        {
            throw new NotImplementedException();
        }
    }
}
