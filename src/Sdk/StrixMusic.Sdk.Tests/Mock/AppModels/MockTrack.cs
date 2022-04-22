using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockTrack : MockPlayableCollectionGroup, ITrack
{
    private int _totalGenreCount;
    private readonly IReadOnlyList<ICoreGenreCollection> _sources = new List<ICoreGenreCollection>();
    private int? _trackNumber;
    private CultureInfo? _language;
    private bool _isExplicit;
    private bool _isChangeAlbumAsyncAvailable;
    private bool _isChangeTrackNumberAsyncAvailable;
    private bool _isChangeLanguageAsyncAvailable;
    private bool _isChangeLyricsAsyncAvailable;
    private bool _isChangeIsExplicitAsyncAvailable;
    private IAlbum? _album;
    private ILyrics? _lyrics;

    public int TotalGenreCount
    {
        get => _totalGenreCount;
        set
        {
            _totalGenreCount = value;
            GenresCountChanged?.Invoke(this, value);
        }
    }

    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalGenreCount--;
        GenresChanged?.Invoke(this, new List<CollectionChangedItem<IGenre>>(), new List<CollectionChangedItem<IGenre>> { new(new MockGenre(), index) });

        return Task.CompletedTask;
    }

    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public event EventHandler<int>? GenresCountChanged;

    public bool Equals(ICoreGenreCollection? other) => false;

    IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => _sources;

    public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IGenre>>(Enumerable.Range(0, limit).Select(_ => new MockGenre()).ToList());

    public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) 
    {
        TotalGenreCount++;
        GenresChanged?.Invoke(this, new List<CollectionChangedItem<IGenre>> { new(genre, index) }, new List<CollectionChangedItem<IGenre>>());

        return Task.CompletedTask;
    }

    public event CollectionChangedEventHandler<IGenre>? GenresChanged;
    
    public bool Equals(ICoreArtist? other) => false;

    public TrackType Type { get; }

    public int? TrackNumber
    {
        get => _trackNumber;
        set
        {
            _trackNumber = value;
            TrackNumberChanged?.Invoke(this, value);
        }
    }

    public int? DiscNumber { get; set; }

    public CultureInfo? Language
    {
        get => _language;
        set
        {
            _language = value;
            LanguageChanged?.Invoke(this, value);
        }
    }

    public bool IsExplicit
    {
        get => _isExplicit;
        set
        {
            _isExplicit = value;
            IsExplicitChanged?.Invoke(this, value);
        }
    }

    public bool IsChangeAlbumAsyncAvailable
    {
        get => _isChangeAlbumAsyncAvailable;
        set
        {
            _isChangeAlbumAsyncAvailable = value;
        }
    }

    public bool IsChangeTrackNumberAsyncAvailable
    {
        get => _isChangeTrackNumberAsyncAvailable;
        set => _isChangeTrackNumberAsyncAvailable = value;
    }

    public bool IsChangeLanguageAsyncAvailable
    {
        get => _isChangeLanguageAsyncAvailable;
        set => _isChangeLanguageAsyncAvailable = value;
    }

    public bool IsChangeLyricsAsyncAvailable
    {
        get => _isChangeLyricsAsyncAvailable;
        set => _isChangeLyricsAsyncAvailable = value;
    }

    public bool IsChangeIsExplicitAsyncAvailable
    {
        get => _isChangeIsExplicitAsyncAvailable;
        set => _isChangeIsExplicitAsyncAvailable = value;
    }

    public Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default)
    {
        TrackNumber = trackNumber;
        return Task.CompletedTask;
    }

    public Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default)
    {
        Language = language;
        return Task.CompletedTask;
    }

    public Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default) 
    {
        IsExplicit = isExplicit;
        return Task.CompletedTask;
    }

    public event EventHandler<int?>? TrackNumberChanged;
    
    public event EventHandler<CultureInfo?>? LanguageChanged;
    
    public event EventHandler<bool>? IsExplicitChanged;

    public event EventHandler<IAlbum?>? AlbumChanged;
    
    public event EventHandler<ILyrics?>? LyricsChanged;
    
    public bool Equals(ICoreTrack? other) => false;
    
    public IReadOnlyList<ICoreTrack> Sources { get; } = new List<ICoreTrack>();

    public IAlbum? Album
    {
        get => _album;
        set
        {
            _album = value;
            AlbumChanged?.Invoke(this, value);
        }
    }

    public ILyrics? Lyrics
    {
        get => _lyrics;
        set
        {
            _lyrics = value;
            LyricsChanged?.Invoke(this, value);
        }
    }

    public IPlayableCollectionGroup? RelatedItems { get; } = new MockPlayableCollectionGroup();

    public Task ChangeLyricsAsync(ILyrics? lyrics, CancellationToken cancellationToken = default)
    {
        Lyrics = lyrics;
        return Task.CompletedTask;
    }
    public Task ChangeAlbumAsync(IAlbum? album, CancellationToken cancellationToken = default)
    {
        Album = album;
        return Task.CompletedTask;
    }
}
