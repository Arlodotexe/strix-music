using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlayableCollectionGroup : ICorePlayableCollection, IPlayableCollectionGroupBase, ICorePlaylistCollection, ICoreTrackCollection, ICoreAlbumCollection, ICoreArtistCollection, ICorePlayableCollectionGroupChildren, ICoreMember
    {
    }
}