using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IPlayableCollectionGroupBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlayableCollectionGroup : IPlayableCollectionGroupBase, ICorePlaylistCollection, ICoreTrackCollection, ICoreAlbumCollection, ICoreArtistCollection, ICorePlayableCollectionGroupChildren, ICoreMember
    {
    }
}