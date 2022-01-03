using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains recently played items.
    /// </summary>
    public interface IRecentlyPlayedBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}