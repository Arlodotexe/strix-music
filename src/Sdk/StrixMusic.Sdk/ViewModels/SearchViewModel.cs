// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A view model wrapper for <see cref="ISearch"/>.
    /// </summary>
    public sealed class SearchViewModel : ObservableObject, ISdkViewModel, ISearch
    {
        private readonly ISearch _search;

        /// <summary>
        /// Creates a new instance of <see cref="SearchViewModel"/>.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="search">The model to wrap around.</param>
        internal SearchViewModel(MainViewModel root, ISearch search)
        {
            Root = root;
            _search = search;

            if (search.SearchHistory != null)
                SearchHistory = new SearchHistoryViewModel(root, search.SearchHistory);

            SourceCores = search.SourceCores.Select(root.GetLoadedCore).ToList();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query) => _search.GetSearchAutoCompleteAsync(query);

        /// <inheritdoc />
        public bool Equals(ICoreSearch other) => _search.Equals(other);

        /// <inheritdoc />
        public IReadOnlyList<ICoreSearch> Sources => _search.Sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc />
        public Task<ISearchResults> GetSearchResultsAsync(string query) => _search.GetSearchResultsAsync(query);

        /// <inheritdoc />
        public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries() => _search.GetRecentSearchQueries();

        /// <inheritdoc />
        ISearchHistory? ISearch.SearchHistory => _search.SearchHistory;

        /// <summary>
        /// The view model for search history.
        /// </summary>
        public SearchHistoryViewModel? SearchHistory { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync() => _search.DisposeAsync();
    }
}
