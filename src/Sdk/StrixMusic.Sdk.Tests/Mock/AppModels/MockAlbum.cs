using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockAlbum : MockPlayableCollectionGroup, IAlbum
{
    private readonly List<IGenre> _genres = new();
    private int _totalGenreCount;
    private DateTime? _datePublished;
    private bool _isChangeDatePublishedAsyncAvailable;

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
        var removedItem = _genres[index];
        _genres.RemoveAt(index);

        TotalGenreCount = _genres.Count;
        GenresChanged?.Invoke(this, new List<CollectionChangedItem<IGenre>>(), new List<CollectionChangedItem<IGenre>> { new(removedItem, index) });

        return Task.CompletedTask;
    }

    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public event EventHandler<int>? GenresCountChanged;

    public bool Equals(ICoreGenreCollection? other) => false;

    IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources { get; } = new List<ICoreGenreCollection>();

    public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _genres.ToAsyncEnumerable();

    public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) 
    {
        _genres.Insert(index, genre);

        TotalGenreCount = _genres.Count;
        GenresChanged?.Invoke(this, new List<CollectionChangedItem<IGenre>> { new(genre, index) }, new List<CollectionChangedItem<IGenre>>());

        return Task.CompletedTask;
    }

    public event CollectionChangedEventHandler<IGenre>? GenresChanged;

    public DateTime? DatePublished
    {
        get => _datePublished;
        set
        {
            _datePublished = value;
            DatePublishedChanged?.Invoke(this, value);
        }
    }

    public bool IsChangeDatePublishedAsyncAvailable
    {
        get => _isChangeDatePublishedAsyncAvailable;
        set
        {
            _isChangeDatePublishedAsyncAvailable = value;
            IsChangeDatePublishedAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    public Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default)
    {
        DatePublished = datePublished;
        return Task.CompletedTask;
    }

    public event EventHandler<DateTime?>? DatePublishedChanged;
    
    public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;
    
    public bool Equals(ICoreAlbum? other) => throw new NotImplementedException();

    IReadOnlyList<ICoreAlbum> IMerged<ICoreAlbum>.Sources{ get; } = new List<ICoreAlbum>();

    public IPlayableCollectionGroup? RelatedItems { get; } = new MockPlayableCollectionGroup();
}
