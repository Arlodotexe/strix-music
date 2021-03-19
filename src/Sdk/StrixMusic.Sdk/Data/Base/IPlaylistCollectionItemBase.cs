using System;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// An item that belongs in an <see cref="IPlaylistCollectionBase"/>.
    /// </summary>
    public interface IPlaylistCollectionItemBase : IPlayableCollectionItem, IAsyncDisposable
    {
    }
}