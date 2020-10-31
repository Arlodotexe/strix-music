namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IPlayableCollectionGroupBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlayableCollectionGroup : IPlayableCollectionGroupBase, ICorePlaylistCollection, ICoreTrackCollection, ICoreAlbumCollection, ICoreArtistCollection, ICorePlayableCollectionGroupChildren, ICoreMember
    {
    }
}