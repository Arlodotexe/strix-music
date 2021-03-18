using System;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylistBase : IPlayableCollectionItem, ITrackCollectionBase, IGenreCollectionBase, IPlaylistCollectionItemBase, IAsyncDisposable
    {
    }
}