using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Delegates search operations
    /// </summary>
    public interface ISearchBase : IAsyncDisposable
    {
        /// <summary>
        /// Given a query, return suggested completed queries.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Suggested completed queries.</returns>
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query);
    }
}