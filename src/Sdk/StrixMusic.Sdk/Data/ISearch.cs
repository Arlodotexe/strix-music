using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// Provides various search-related activities.
    /// </summary>
    public interface ISearch : ISearchBase, ICoreMember
    {
        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ICoreSearchResults"/> containing multiple.</returns>
        public Task<ISearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Gets the recently searched 
        /// </summary>
        /// <returns>The recent search queries.</returns>
        public IAsyncEnumerable<ISearchQuery> GetSearchHistory();
    }
}