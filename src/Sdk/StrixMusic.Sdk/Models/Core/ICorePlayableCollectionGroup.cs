using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IPlayableCollectionGroupBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlayableCollectionGroup : ICorePlayableCollection, IPlayableCollectionGroupBase, ICorePlaylistCollection, ICoreTrackCollection, ICoreAlbumCollection, ICoreArtistCollection, ICorePlayableCollectionGroupChildren, ICoreMember
    {
    }
}