namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroupBase : IPlaylistCollectionBase, ITrackCollectionBase, IAlbumCollectionBase, IArtistCollectionBase, ICollectionItemBase, IPlayableCollectionGroupChildrenBase
    {
    }
}
