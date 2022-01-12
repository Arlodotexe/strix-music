using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A collection of artibrary songs that the user can edit, rearrange and play back.
    /// </summary>
    public interface IPlaylistBase : IPlayableCollectionItem, ITrackCollectionBase, IPlaylistCollectionItemBase, IAsyncDisposable
    {
    }
}