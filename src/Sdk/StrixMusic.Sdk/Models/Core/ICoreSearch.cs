﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A core's implementation of various search-related activities.
    /// </summary>
    public interface ICoreSearch : ISearchBase, ICoreMember
    {
        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ICoreSearchResults"/> containing multiple.</returns>
        public Task<ICoreSearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Gets the recently searched 
        /// </summary>
        /// <returns>The recent search queries.</returns>
        public IAsyncEnumerable<ICoreSearchQuery> GetRecentSearchQueries();

        /// <summary>
        /// Contains items that the user has recently selected from the search results.
        /// </summary>
        ICoreSearchHistory? SearchHistory { get; }
    }
}