using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// An item that belongs in an <see cref="IArtistCollectionBase"/>.
    /// </summary>
    public interface IArtistCollectionItemBase : IPlayableCollectionItem, IAsyncDisposable
    {
    }
}
