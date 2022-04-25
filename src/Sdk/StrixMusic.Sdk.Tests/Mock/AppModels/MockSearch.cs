using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockSearch : ISearch
{
    public ValueTask DisposeAsync() => default;

    public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<string>();

    public bool Equals(ICoreSearch? other) => false;

    public IReadOnlyList<ICoreSearch> Sources { get; } = new List<ICoreSearch>();

    public Task<ISearchResults> GetSearchResultsAsync(string query, CancellationToken cancellationToken = default) => Task.FromResult<ISearchResults>(new MockSearchResults());

    public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries(CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<ISearchQuery>();

    public ISearchHistory? SearchHistory { get; } = new MockSearchHistory();
    public event EventHandler? SourcesChanged;
}
