using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A view model wrapper for <see cref="ISearch"/>.
    /// </summary>
    public class SearchViewModel : ObservableObject, ISearch
    {
        private readonly ISearch _search;

        /// <summary>
        /// Creates a new instance of <see cref="SearchViewModel"/>.
        /// </summary>
        /// <param name="search">The model to wrap around.</param>
        public SearchViewModel(ISearch search)
        {
            _search = search;

            if (search.SearchHistory != null)
                SearchHistory = new SearchHistoryViewModel(search.SearchHistory);

            SourceCores = search.SourceCores.Select(MainViewModel.GetLoadedCore).ToList();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query)
        {
            return _search.GetSearchAutoCompleteAsync(query);
        }

        /// <inheritdoc />
        public bool Equals(ICoreSearch other)
        {
            return _search.Equals(other);
        }

        /// <inheritdoc />
        public IReadOnlyList<ICoreSearch> Sources => _search.Sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            return _search.GetSearchResultsAsync(query);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries()
        {
            return _search.GetRecentSearchQueries();
        }

        /// <inheritdoc />
        ISearchHistory? ISearch.SearchHistory => _search.SearchHistory;

        /// <summary>
        /// The view model for search history.
        /// </summary>
        public SearchHistoryViewModel? SearchHistory { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return _search.DisposeAsync();
        }
    }
}