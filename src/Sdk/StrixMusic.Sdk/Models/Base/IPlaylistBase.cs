using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylistBase : IPlayableCollectionItem, ITrackCollectionBase, IPlaylistCollectionItemBase, IAsyncDisposable
    {
    }
}