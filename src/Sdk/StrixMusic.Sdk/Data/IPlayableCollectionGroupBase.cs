namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroupBase : IPlayable, IPlaylistCollectionBase, ITrackCollectionBase, IAlbumCollectionBase, IArtistCollectionBase, IPlayableCollectionGroupChildrenBase
    {
    }
}
