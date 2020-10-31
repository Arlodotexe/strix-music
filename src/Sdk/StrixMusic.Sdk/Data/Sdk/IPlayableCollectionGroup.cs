namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IPlayableCollectionGroupBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlayableCollectionGroup : IPlayableCollectionGroupBase, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection, IPlayableCollectionGroupChildren, ISdkMember
    {
    }
}