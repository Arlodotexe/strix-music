using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// An item that belongs in an <see cref="IAlbumCollectionBase"/> or <see cref="IAlbumBase"/>.
    /// </summary>
    public interface IAlbumCollectionItemBase : ICollectionItemBase, IPlayableCollectionItem, IAsyncDisposable
    {
    }
}
