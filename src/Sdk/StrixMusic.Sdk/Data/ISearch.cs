using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// Provides various search-related activities.
    /// </summary>
    public interface ISearch : ISearchBase, ISdkMember, IMerged<ICoreSearch>
    {
        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Value is <see cref="ISearchResults"/>.</returns>
        Task<ISearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Gets the items  
        /// </summary>
        /// <returns>The recent search queries.</returns>
        public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries();

        /// <summary>
        /// Contains items that the user has recently selected from the search results.
        /// </summary>
        ISearchHistory? SearchHistory { get; }
    }
}