// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ISearch"/>.
    /// </summary>
    public sealed class SearchViewModel : ObservableObject, ISdkViewModel, ISearch, IDelegatable<ISearch>
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
        }

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => _search.SourcesChanged += value;
            remove => _search.SourcesChanged -= value;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query, CancellationToken cancellationToken = default) => _search.GetSearchAutoCompleteAsync(query, cancellationToken);

        /// <inheritdoc />
        public bool Equals(ICoreSearch? other) => _search.Equals(other!);

        /// <inheritdoc />
        public IReadOnlyList<ICoreSearch> Sources => _search.Sources;

        /// <inheritdoc />
        public Task<ISearchResults> GetSearchResultsAsync(string query, CancellationToken cancellationToken = default) => _search.GetSearchResultsAsync(query, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries(CancellationToken cancellationToken = default) => _search.GetRecentSearchQueries(cancellationToken);

        /// <inheritdoc/>
        ISearch IDelegatable<ISearch>.Inner => _search;

        /// <inheritdoc />
        ISearchHistory? ISearch.SearchHistory => _search.SearchHistory;

        /// <summary>
        /// The view model for search history.
        /// </summary>
        public SearchHistoryViewModel? SearchHistory { get; }
    }
}
