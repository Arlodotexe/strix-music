using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains a history of playable items which were selected from search results.
    /// </summary>
    public interface ISearchHistoryBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}