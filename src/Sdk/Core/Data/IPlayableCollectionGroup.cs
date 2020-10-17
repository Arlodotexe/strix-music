namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Represents multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroup : IPlayable, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection, IPlayableCollectionGroupChildren
    {
    }
}
