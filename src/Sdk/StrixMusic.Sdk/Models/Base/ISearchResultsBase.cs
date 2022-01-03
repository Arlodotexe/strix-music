using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Relevant items requested with a query from a core.
    /// </summary>
    public interface ISearchResultsBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}