// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Models.Base
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