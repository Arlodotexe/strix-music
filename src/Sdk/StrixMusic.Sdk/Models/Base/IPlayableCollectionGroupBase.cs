using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroupBase : IPlaylistCollectionBase, ITrackCollectionBase, IAlbumCollectionBase, IArtistCollectionBase, ICollectionItemBase, IPlayableCollectionGroupChildrenBase, IAsyncDisposable
    {
    }
}
