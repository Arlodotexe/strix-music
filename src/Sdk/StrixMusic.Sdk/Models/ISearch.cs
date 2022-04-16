// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Provides various search-related activities.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ISearch : ISearchBase, ISdkMember, IMerged<ICoreSearch>
    {
        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A task representing the async operation. Value is <see cref="ISearchResults"/>.</returns>
        Task<ISearchResults> GetSearchResultsAsync(string query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the recent search queries.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>The recent search queries.</returns>
        public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries(CancellationToken cancellationToken = default);

        /// <summary>
        /// Contains items that the user has recently selected from the search results.
        /// </summary>
        ISearchHistory? SearchHistory { get; }
    }
}
