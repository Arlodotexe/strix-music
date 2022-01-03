using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A base class for playable collections.
    /// </summary>
    public interface IPlayableCollectionBase : ICollectionBase, IPlayableCollectionItem, IAsyncDisposable
    {
    }
}