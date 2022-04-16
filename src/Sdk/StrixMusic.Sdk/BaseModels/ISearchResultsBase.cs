// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// Relevant items requested with a query from a core.
    /// </summary>
    public interface ISearchResultsBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}
